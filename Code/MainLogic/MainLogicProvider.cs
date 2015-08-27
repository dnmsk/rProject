using System;
using System.Xml;
using CommonUtils.Core.Config;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using MainLogic.Providers;

namespace MainLogic {
    public class MainLogicProvider : Singleton<MainLogicProvider> {
        public static readonly  IWatchfulSloth WatchfulSloth = new WatchfulSloth(1000);
        public readonly UserProvider UserProvider = new UserProvider();
        public readonly AccountProvider AccountProvider = new AccountProvider();

        static MainLogicProvider() {
            RegisterDB();
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
