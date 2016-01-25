using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor {
    public class HtmlBlockHelper {
        private readonly HtmlDocument _htmlDoc;
        public HtmlBlockHelper(string html) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            _htmlDoc = htmlDocument;
        }

        public HtmlNode GetCurrentNode() {
            return _htmlDoc.DocumentNode;
        }
        
        public List<HtmlNode> ExtractBlock(XPathQuery xPathQuery) {
            return ExtractBlock(_htmlDoc.DocumentNode, xPathQuery);
        }
        
        public static List<HtmlNode> ExtractBlock(HtmlNode htmlNode, XPathQuery xPathQuery) {
            return NodesToList(htmlNode.SelectNodes(xPathQuery.GetXPathQuery()));
        }
        
        private static List<HtmlNode> NodesToList(HtmlNodeCollection nodes) {
            return (nodes != null && nodes.Any()) ? nodes.ToList() : new List<HtmlNode>();
        }
    }
}