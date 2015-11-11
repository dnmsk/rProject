using System;
using System.CodeDom;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using AutoPublication.Code;
using CommonUtils.Code;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.WatchfulSloths.SlothMoveRules;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using Spywords_Project.Code.Algorithms;

namespace Sandbox {
    class Program {
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(Program).FullName);
        static void Main(string[] args) {
            try {
                RegisterDB();
                var algo = new CollectSearchEngineDomainsFromPhrase();
                while (true) {
                    Thread.Sleep(100);
                }
                /**/
                //Console.WriteLine(start.ElapsedMilliseconds);
                //Console.ReadLine();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                _logger.Error(ex);
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
