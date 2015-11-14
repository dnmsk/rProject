using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using CommonUtils.ExtendedTypes;

namespace MainLogic.WebFiles {
    public static class SiteConfiguration {
        public static string ProductionHostName { get; private set; }
        public static string YandexMetrikaID { get; private set; }
        public static string GoogleAnalyticsID { get; private set; }
        public static string MailruID { get; private set; }
        public static bool NeedRunTask { get; private set; }

        static SiteConfiguration() {
            ProductionHostName = GetConfigurationProperty("ProductionHostName");
            YandexMetrikaID = GetConfigurationProperty("YandexMetrikaID");
            GoogleAnalyticsID = GetConfigurationProperty("GoogleAnalyticsID");
            MailruID = GetConfigurationProperty("MailruID");
            NeedRunTask = GetConfigurationProperty<bool>("RunTask");
        }
        
        public static string GetConfigurationProperty(string configurationProperty) {
            return GetConfigurationProperty<string>(configurationProperty);
        }
        
        private static readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();
        private static readonly Dictionary<string, object> _propertyCache = new Dictionary<string, object>(); 

        public static T GetConfigurationProperty<T>(string configurationProperty) {
            object obj;
            if (_propertyCache.TryGetValue(configurationProperty, out obj)) {
                return (T) obj;
            }
            var appSetting = ConfigurationManager.AppSettings[configurationProperty];
            if (appSetting.IsNullOrEmpty()) {
                var defalutObj = default(T);
                _propertyCache[configurationProperty] = defalutObj;
                return defalutObj;
            }
            if (typeof (T).IsEquivalentTo(typeof (string)) && appSetting[0] != '"') {
                appSetting = "\"" + appSetting + "\"";
                appSetting = appSetting.Replace("\\", "\\\\");
            }
            var deserializedObj = _javaScriptSerializer.Deserialize<T>(appSetting);
            _propertyCache[configurationProperty] = deserializedObj;
            return deserializedObj;
        }

        public static void ModifyConfigurationProperty(string key, string value) {
            ModifyConfigurationProperty(new Dictionary<string, string> {
                {key, value}
            });
        }
        public static void ModifyConfigurationProperty(Dictionary<string, string> pars) {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (AppSettingsSection)configuration.GetSection("appSettings");
            foreach (var par in pars) {
                if (_propertyCache.ContainsKey(par.Key)) {
                    _propertyCache.Remove(par.Key);
                }
                var keyValueConfigurationElement = section.Settings[par.Key];
                if(keyValueConfigurationElement == null) {
                    keyValueConfigurationElement = new KeyValueConfigurationElement(par.Key, string.Empty);
                    section.Settings.Add(keyValueConfigurationElement);
                }
                keyValueConfigurationElement.Value = par.Value;
            }
            configuration.Save();
        }
    }
}