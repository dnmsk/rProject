using System.Configuration;

namespace Project_R.Code {
    public static class SiteConfiguration {
        public static string ProductionHostName { get; private set; }
        public static string YandexMetrikaID { get; private set; }
        public static string GoogleAnalyticsID { get; private set; }

        static SiteConfiguration() {
            ProductionHostName = ConfigurationManager.AppSettings["ProductionHostName"];
            YandexMetrikaID = ConfigurationManager.AppSettings["YandexMetrikaID"];
            GoogleAnalyticsID = ConfigurationManager.AppSettings["GoogleAnalyticsID"];
        }
    }
}