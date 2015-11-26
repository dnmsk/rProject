using System;
using System.Xml;
using CommonUtils.Core.Config;
using CommonUtils.ExtendedTypes;
using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.Configuration {
    public class ConfigurationContainer : Singleton<ConfigurationContainer> {
        private const string _brokerConfigurationFileName = "Broker.xml";

        public SimpleConfiguration<BrokerType, BrokerConfiguration> BrokerConfiguration = new SimpleConfiguration<BrokerType, BrokerConfiguration>();

        public ConfigurationContainer() {
            ConfigHelper.RegisterConfigTarget(_brokerConfigurationFileName, xml => {
                var xmlDoc = (XmlDocument)xml;
                var newConfiguration = new SimpleConfiguration<BrokerType, BrokerConfiguration>();
                foreach (XmlNode brokerNode in xmlDoc.SelectNodes(".//Broker")) {
                    var brokerName = brokerNode.Attributes["Name"].InnerText;
                    BrokerType brokerType;
                    if (!Enum.TryParse(brokerName, out brokerType)) {
                        continue;
                    }
                    newConfiguration[brokerType] = new BrokerConfiguration(brokerNode);
                }
                BrokerConfiguration = newConfiguration;
            });
        }
    }
}