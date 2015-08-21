using System;
using System.Xml;
using CommonUtils;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;

namespace Sandbox {
    class Program {
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(Program).FullName);
        static void Main(string[] args) {
            try {
                RegisterDB();
                EntityClassCreator.Create(
                    DatabaseActions.GetAdapter(TargetDB.MASTER),
                    new[] {
   /*                     "guest",
                        "guestreferrer",
                        "GuestExistsBrowser",
                        "GuestTechInfo",
                        "UtmGuestReferrer",
                        "utmsubdomainrule",*/
                        "guestactionlog"
                    },
                    TargetDB.MASTER,
                    "e:\\"
                );
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                _logger.Error(ex);
            }
            Console.WriteLine("Success");
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
