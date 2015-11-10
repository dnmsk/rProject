﻿namespace Project_B.CodeServerSide.BrokerProvider.HtmlDataExtractor {
    public class XPathQuery {
        private readonly string _xPath;

        public XPathQuery(string xPath) {
            _xPath = xPath;
        }

        public string GetXPathQuery() {
            return _xPath;
        }
    }
}