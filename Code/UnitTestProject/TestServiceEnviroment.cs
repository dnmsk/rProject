using CommonUtils.Core.Config;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.DbConfig;
using NUnit.Framework;

namespace UnitTestProject {
    [SetUpFixture]
    public class TestServiceEnviroment {
        [SetUp]
        public void SetUp() {
            ConfigHelper.TestMode = true;
            var target = ConfigHelper.LocalConfigPath;
            DatabaseActions.AddConnectionOption("Master", new ConnectOptions("Server=localhost;Port=5432;Database=rproject_db;User ID=postgres;Password=Ingate2009;", 10, 2, DbServerType.PostgreSql));
        }
    }
}