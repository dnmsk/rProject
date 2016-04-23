using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using Project_B.CodeServerSide;
using Project_B.CodeServerSide.Enums;

namespace Sandbox {
    class Program {
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(Program).FullName);
        private static readonly ConcurrentQueue<Action> _actionsToExecute = new ConcurrentQueue<Action>();
        
        static void Main(string[] args) {
            try {
                var bp = BookPage.Instance.GetBrokerProvider(BrokerType.RedBlue);
                while (true) {
                    var data = bp.LoadLive(SportType.All, LanguageType.English);
                    Thread.Sleep(5000);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
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
