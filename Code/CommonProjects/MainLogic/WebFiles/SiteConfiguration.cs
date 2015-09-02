using System.Configuration;
using System.Web.Configuration;
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

        public static string GetConfigurationProperty(string configurationProperty) {
            return ConfigurationManager.AppSettings[configurationProperty];
        }

        public static void ModifyConfigurationProperty(string key, string value) {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (AppSettingsSection)configuration.GetSection("appSettings");
            var keyValueConfigurationElement = section.Settings[key];
            if(keyValueConfigurationElement == null) {
                keyValueConfigurationElement = new KeyValueConfigurationElement(key, string.Empty);
                section.Settings.Add(keyValueConfigurationElement);
            }
            keyValueConfigurationElement.Value = value;
            configuration.Save();
        }
    }
}