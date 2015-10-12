﻿using System;
using System.Xml;
using CommonUtils.Core.Config;
using CommonUtils.ExtendedTypes;
using Project_B.Code.Enums;

namespace Project_B.Code.BrokerProvider.Configuration {
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
                    if (!Enum.TryParse(brokerName, out brokerType) || brokerType == BrokerType.Unknown) {
                        continue;
                    }
                    newConfiguration[brokerType] = new BrokerConfiguration(brokerNode);
                }
                BrokerConfiguration = newConfiguration;
            });
        }
    }
}