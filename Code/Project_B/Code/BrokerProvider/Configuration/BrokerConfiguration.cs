using System;
using System.Linq;
using System.Xml;
using CommonUtils.Core.Logger;
using Project_B.Code.BrokerProvider.HtmlDataExtractor;

namespace Project_B.Code.BrokerProvider.Configuration {
    public class BrokerConfiguration {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof (BrokerConfiguration).FullName);

        public SimpleConfiguration<SectionName, XPathQuery> XPath;
        public SimpleConfiguration<SectionName, string> StringSimple;
        public SimpleConfiguration<SectionName, string[]> StringArray;
        public SimpleConfiguration<SectionName, SimpleConfiguration<string, string>> CompetitionConfiguration;

        public BrokerConfiguration(XmlNode configNode) {
            BuildXPathMap(configNode.SelectNodes(".//XPathNode"));
            BuildStringSimpleMap(configNode.SelectNodes(".//StringSimple"));
            BuildStringArrayMap(configNode.SelectNodes(".//ArrayString"));
            BuildCompetitionConfigurationMap(configNode.SelectNodes(".//MapStrings"));
        }

        private void BuildCompetitionConfigurationMap(XmlNodeList selectNodes) {
            CompetitionConfiguration = new SimpleConfiguration<SectionName, SimpleConfiguration<string, string>>();
            BuildItem(selectNodes, CompetitionConfiguration, node => {
                var item = new SimpleConfiguration<string, string>();
                BuildItemMap(node.SelectNodes(".//Item"), item, nodeItem => nodeItem.InnerText);
                return item;
            });
        }

        private void BuildXPathMap(XmlNodeList xPathNodeList) {
            XPath = new SimpleConfiguration<SectionName, XPathQuery>();
            BuildItem(xPathNodeList, XPath, node => new XPathQuery(node.InnerText));
        }

        private void BuildStringSimpleMap(XmlNodeList xPathNodeList) {
            StringSimple = new SimpleConfiguration<SectionName, string>();
            BuildItem(xPathNodeList, StringSimple, node => node.InnerText);
        }

        private void BuildStringArrayMap(XmlNodeList xPathNodeList) {
            StringArray = new SimpleConfiguration<SectionName, string[]>();
            BuildItem(xPathNodeList, StringArray, node => (from XmlNode subNode in node.SelectNodes(".//Item") select subNode.InnerText).ToArray());
        }

        private static void BuildItem<E, T>(XmlNodeList xPathNodeList, SimpleConfiguration<E, T> targetMap, Func<XmlNode, T> nodeToValueFunc) where E : struct {
            foreach (XmlNode xmlNode in xPathNodeList) {
                var key = xmlNode.Attributes["Key"].InnerText;
                E sectionName;
                if (!Enum.TryParse<E>(key, out sectionName) || sectionName.Equals(default(E))) {
                    _logger.Error("section key={0} contains={1} sectionName={3}", key, xmlNode.InnerXml, sectionName);
                    continue;
                }
                targetMap[sectionName] = nodeToValueFunc(xmlNode);
            }
        }
        private static void BuildItemMap(XmlNodeList xPathNodeList, SimpleConfiguration<string, string> targetMap, Func<XmlNode, string> nodeToValueFunc) {
            foreach (XmlNode xmlNode in xPathNodeList) {
                var key = xmlNode.Attributes["Key"].InnerText;
                targetMap[key] = nodeToValueFunc(xmlNode);
            }
        }
    }
}