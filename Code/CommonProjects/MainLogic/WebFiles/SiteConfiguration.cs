using System.Configuration;
using CommonUtils.Code;

namespace MainLogic.WebFiles {
    public static class SiteConfiguration {
        public static string ProductionHostName { get; private set; }
        public static string YandexMetrikaID { get; private set; }
        public static string GoogleAnalyticsID { get; private set; }
        public static string MailruID { get; private set; }
        public static bool NeedRunTask { get; private set; }

        static SiteConfiguration() {
            ProductionHostName = ConfigurationManager.AppSettings["ProductionHostName"];
            YandexMetrikaID = ConfigurationManager.AppSettings["YandexMetrikaID"];
            GoogleAnalyticsID = ConfigurationManager.AppSettings["GoogleAnalyticsID"];
            MailruID = ConfigurationManager.AppSettings["MailruID"];
            var runTaskProperty = ConfigurationManager.AppSettings["RunTask"];
            if (runTaskProperty != null) {
                NeedRunTask = StringParser.ToBool(runTaskProperty.ToLower());
            }
        }
    }
}