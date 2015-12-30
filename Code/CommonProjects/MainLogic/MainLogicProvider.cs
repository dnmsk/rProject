using System;
using System.Xml;
using CommonUtils.Core.Config;
using CommonUtils.Core.Logger;
using CommonUtils.ExtendedTypes;
using CommonUtils.WatchfulSloths;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using IDEV.Hydra.DAO.DbConfig.InternalLogger;
using MainLogic.Providers;

namespace MainLogic {
    public class MainLogicProvider : Singleton<MainLogicProvider> {
        public static readonly  IWatchfulSloth WatchfulSloth = new WatchfulSloth();
        public readonly UserProvider UserProvider = new UserProvider();
        public readonly AccountProvider AccountProvider = new AccountProvider();

        static MainLogicProvider() {
            RegisterDB();
        }

        static void RegisterDB() {
            DatabaseActions.SetLoggerBuilder(s => {
                var daoLogger = LoggerManager.GetLogger("DAO");
                return new LoggerObj(
                    (s1, exception) => {
                        daoLogger.Info(s1 + exception);
                    },
                    (s1, objects) => {
                        daoLogger.Info(s1, objects);
                    },
                    exception => daoLogger.Error(exception),
                    (s1, objects) => daoLogger.Error(s1, objects),
                    (s1, objects) => {
                        daoLogger.Debug(s1, objects);
                    },
                    false
                );
            });
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
