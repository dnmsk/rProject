using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor.Extractors;

namespace Sandbox {
    class Trash {
        public static void DefaultExtractor() {

            try {
               // RegisterDB();
               // var algo = new CollectDomainInfoSpywords();

                /*
                var dataResult = BookPage.Instance.GetBrokerProvider(BrokerType.ASport)
                                   .LoadRegular(SportType.All, LanguageType.English);
                */
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
                  //  Thread.Sleep(1000);
                }

                //Console.WriteLine(start.ElapsedMilliseconds);
                //Console.ReadLine();
            } catch (Exception ex) {
                Console.WriteLine(ex);
               // _logger.Error(ex);
                Console.ReadLine();
            }
            Console.WriteLine("Success");
            //Console.ReadLine();
        }
    }
}
