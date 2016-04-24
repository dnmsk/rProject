using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using CommonUtils.WatchfulSloths.WatchfulThreads;
using IDEV.Hydra.DAO;
using MainLogic;
using Project_B.CodeServerSide.BrokerProvider.Interfaces;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.DataProvider.Transport;
using Project_B.CodeServerSide.Entity;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Algorithm {
    public class BrokerAlgoLauncher {
        private BrokerRequestScheduleTransport[] _schedule = new BrokerRequestScheduleTransport[0];

        public BrokerAlgoLauncher() {
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(InitSchedule, TimeSpan.FromMinutes(10), null));
            InitSchedule();
            MainLogicProvider.WatchfulSloth.SetMove(new SlothMoveByTimeSingle<object>(LaunchScheduled, TimeSpan.FromSeconds(15), null));
        }

        private object InitSchedule() {
            var newSchedule = new List<BrokerRequestScheduleTransport>();
            foreach (var brokerSchedule in SystemBrokerRequestSchedule.DataSource.AsList()) {
                newSchedule.Add(new BrokerRequestScheduleTransport {
                    Languagetype = brokerSchedule.Languagetype,
                    BrokerID = brokerSchedule.BrokerID,
                    Behaviormode = brokerSchedule.Behaviormode,
                    SportType = brokerSchedule.SportType,
                    Pasthistorydays = brokerSchedule.Pasthistorydays,
                    RepeatSchedule = new Dictionary<RunTaskMode, TimeSpan> {
                        {RunTaskMode.RunRegularOddsTask, TimeSpan.FromSeconds(brokerSchedule.Betrepeat) },
                        {RunTaskMode.RunLiveOddsTask, TimeSpan.FromSeconds(brokerSchedule.Livebetrepeat) },
                        {RunTaskMode.RunTodayHistoryTask, TimeSpan.FromSeconds(brokerSchedule.Historyrepeat) },
                        {RunTaskMode.RunPastDateHistoryTask, TimeSpan.FromSeconds(brokerSchedule.Pasthistoryrepeat) },
                    },
                    LastTaskRun = new Dictionary<RunTaskMode, DateTime>()
                });
            }
            _schedule = newSchedule.ToArray();
            return null;
        }

        private object LaunchScheduled() {
            var schedule = _schedule;
            var utcNow = DateTime.UtcNow;
            foreach (var brokerSchedule in schedule) {
                foreach (var brokerTask in brokerSchedule.RepeatSchedule) {
                    if ((int) brokerTask.Value.TotalSeconds <= 9) {
                        continue; //idiot-safety mode
                    }
                    DateTime lastRunDateTime;
                    if (!brokerSchedule.LastTaskRun.TryGetValue(brokerTask.Key, out lastRunDateTime)) {
                        var brokerLastRequest = SystemBrokerRequest.DataSource
                            .WhereEquals(SystemBrokerRequest.Fields.BrokerID, (short)brokerSchedule.BrokerID)
                            .WhereEquals(SystemBrokerRequest.Fields.Languagetype, (short)brokerSchedule.Languagetype)
                            .WhereEquals(SystemBrokerRequest.Fields.Taskmodetype, (short)brokerTask.Key)
                            .Sort(SystemBrokerRequest.Fields.Datelaunchedutc, SortDirection.Desc)
                            .First(SystemBrokerRequest.Fields.Datelaunchedutc);
                        lastRunDateTime = brokerLastRequest?.Datelaunchedutc ?? DateTime.MinValue;
                        brokerSchedule.LastTaskRun[brokerTask.Key] = lastRunDateTime;
                    }
                    if (utcNow - lastRunDateTime > brokerTask.Value) {
                        SystemBrokerRequest systemBrokerRequestToLaunch;
                        switch (brokerTask.Key) {
                            case RunTaskMode.RunLiveOddsTask:
                            case RunTaskMode.RunRegularOddsTask:
                            case RunTaskMode.RunTodayHistoryTask:
                                systemBrokerRequestToLaunch = new SystemBrokerRequest {
                                    BrokerID = brokerSchedule.BrokerID,
                                    Languagetype = brokerSchedule.Languagetype,
                                    AlgoDateutc = utcNow,
                                    Datecreatedutc = utcNow,
                                    Taskmodetype = brokerTask.Key
                                };
                                RunTask(brokerSchedule, systemBrokerRequestToLaunch, utcNow);
                                break;
                            case RunTaskMode.RunPastDateHistoryTask:
                                for (var i = brokerSchedule.Pasthistorydays; i > 0; i--) {
                                    var algoDate = utcNow.Date.AddDays(-i);
                                    systemBrokerRequestToLaunch = new SystemBrokerRequest {
                                        BrokerID = brokerSchedule.BrokerID,
                                        Languagetype = brokerSchedule.Languagetype,
                                        AlgoDateutc = algoDate,
                                        Datecreatedutc = utcNow,
                                        Taskmodetype = brokerTask.Key
                                    };
                                    RunTask(brokerSchedule, systemBrokerRequestToLaunch, utcNow);
                                }
                                break;
                        }
                    }
                }
            }
            return null;
        }

        private void RunTask(BrokerRequestScheduleTransport brokerSchedule, SystemBrokerRequest launchedRequest, DateTime utcNow) {
            launchedRequest.Datelaunchedutc = utcNow;
            launchedRequest.Save();
            brokerSchedule.LastTaskRun[launchedRequest.Taskmodetype] = utcNow;
            TaskRunner.Instance.AddAction(() => {
                Func<BrokerData> dataGetter = null;
                var additionalStat = ProcessStatType.Default;
                switch (launchedRequest.Taskmodetype) {
                    case RunTaskMode.RunTodayHistoryTask:
                    case RunTaskMode.RunPastDateHistoryTask:
                        dataGetter = () => GetBroker(brokerSchedule.BrokerID).LoadResult(launchedRequest.AlgoDateutc.Date, brokerSchedule.SportType, brokerSchedule.Languagetype);
                        additionalStat = ProcessStatType.Result;
                        break;
                    case RunTaskMode.RunLiveOddsTask:
                        dataGetter = () => GetBroker(brokerSchedule.BrokerID).LoadLive(brokerSchedule.SportType, brokerSchedule.Languagetype);
                        additionalStat = ProcessStatType.Bet;
                        break;
                    case RunTaskMode.RunRegularOddsTask:
                        dataGetter = () => GetBroker(brokerSchedule.BrokerID).LoadRegular(brokerSchedule.SportType, brokerSchedule.Languagetype);
                        additionalStat = ProcessStatType.Bet;
                        break;
                }
                if (dataGetter != null) {
                    var sw = Stopwatch.StartNew();
                    var data = dataGetter();
                    var stat = ProjectProvider.Instance.BetProvider.SaveBrokerState(data, brokerSchedule.Behaviormode, launchedRequest.Taskmodetype);
                    sw.Stop();
                    launchedRequest.Duratation = (int) sw.ElapsedMilliseconds;
                    launchedRequest.Counttotalrawci = stat[ProcessStatType.CompetitionItemFromRaw].TotalCount;
                    launchedRequest.Countnewrawci = stat[ProcessStatType.CompetitionItemFromRaw].CreateRawCount;
                    launchedRequest.Countnewci = stat[ProcessStatType.CompetitionItemFromRaw].CreateOriginalCount;
                    launchedRequest.Counttotalentity = stat[additionalStat].TotalCount;
                    launchedRequest.Countnewentity = stat[additionalStat].CreateOriginalCount;
                    launchedRequest.Save();
                }
            });
        }
        
        private IBrokerBase GetBroker(BrokerType brokerType) => BookPage.Instance.GetBrokerProvider(brokerType);
    }
}