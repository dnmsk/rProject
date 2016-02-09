using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using AutoPublication.Code;
using CommonUtils.Code;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using IDEV.Hydra.DAO.DbFunctions;
using IDEV.Hydra.DAO.Filters;
using MainLogic.WebFiles;
using MainLogic.Wrapper;
using Project_B.CodeClientSide.Helper;
using Project_B.CodeServerSide;
using Project_B.CodeServerSide.Algorithm;
using Project_B.CodeServerSide.BrokerProvider;
using Project_B.CodeServerSide.Data;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.DataProvider.DataHelper;
using Project_B.CodeServerSide.DataProvider.DataHelper.ProcessData;
using Project_B.CodeServerSide.Entity.BrokerEntity.RawEntity;
using Project_B.CodeServerSide.Entity.Interface;
using Project_B.CodeServerSide.Enums;
using Spywords_Project.Code;
using Spywords_Project.Code.Algorithms;

namespace Sandbox {
    class Program {
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(Program).FullName);
        private static readonly ConcurrentQueue<Action> _actionsToExecute = new ConcurrentQueue<Action>();
        
        static void Main(string[] args) {
            try {
                RegisterDB();

                var dataResult = BookPage.Instance.GetBrokerProvider(BrokerType.ASport)
                                   .LoadRegular(SportType.All, LanguageType.English);
                /*
                var betProvider = new BetProvider();
                var lastRunTime = DateTime.MinValue;
                var delay = TimeSpan.FromSeconds(40);
                for (DateTime date = new DateTime(2015, 11, 24); date > new DateTime(2015, 01, 01); date = date.AddDays(-1)) {
                    while (DateTime.Now - lastRunTime < delay) {
                        Thread.Sleep(1000);
                    }
                    lastRunTime = DateTime.Now;
                    var dataResult = BookPage.Instance.GetBrokerProvider(BrokerType.LightBlue)
                                       .LoadResult(date, SportType.All, LanguageType.English);
                    var stat = betProvider.SaveBrokerState(dataResult, GatherBehaviorMode.TryDetectAll, RunTaskMode.RunPastDateHistoryTask);
                    Console.WriteLine("{0}: {1} RawCompetitionItem = {2}", DateTime.Now, date.ToString("dd-MM-yyyy"), stat[ProcessStatType.CompetitionItemFromRaw].TotalCount);
                }
                //*/
                //var types = BrokerSettingsHelper.Instance.GetBrokerTypes;
                Console.WriteLine("End: " + DateTime.Now);
                while (true) {
                    Thread.Sleep(1000);
                }

                //Console.WriteLine(start.ElapsedMilliseconds);
                //Console.ReadLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                _logger.Error(ex);
                Console.ReadLine();
            }
            Console.WriteLine("Success");
            //Console.ReadLine();
        }
        
        static void RegisterDB() {
            ConfigHelper.RegisterConfigTarget("Application.xml", xml => {
                var xmlDoc = (XmlDocument)xml;
                var container = xmlDoc.GetElementsByTagName("ConnectionStrings");
                if (container.Count != 1) {
                    return;
                }
                foreach (XmlNode connectionItem in container[0].ChildNodes) {
                    DatabaseActions.AddConnectionOption(
                        connectionItem.Attributes["name"].Value,
                        new ConnectOptions(
                            connectionItem.Attributes["connectionString"].Value,
                            uint.Parse(connectionItem.Attributes["maxConnCount"].Value),
                            uint.Parse(connectionItem.Attributes["maxRetries"].Value),
                            (DbServerType)Enum.Parse(typeof(DbServerType), connectionItem.Attributes["dbServerType"].Value))
                    );
                }
            });
        }
    }
}
