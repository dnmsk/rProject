using System;
using System.Reflection;
using System.Xml;
using CommonUtils.Core.Config;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using NUnit.Framework;
using UnitTestProject.FactoryEntities;

namespace UnitTestProject {
    [SetUpFixture]
    public class TestServiceEnviroment {
        [SetUp]
        public void SetUp() {
            Factory.Init(Assembly.GetExecutingAssembly());

            ConfigHelper.TestMode = true;
            ConfigHelper.RegisterConfigTarget("Application.xml", xml => {
                var xmlDoc = (XmlDocument) xml;
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
                            (DbServerType) Enum.Parse(typeof(DbServerType), connectionItem.Attributes["dbServerType"].Value))
                    );
                }
            });
        }
    }
}