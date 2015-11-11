using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace Spywords_Project.Code {
    public class HtmlBlockHelper {
        private readonly HtmlDocument _htmlDoc;
        public HtmlBlockHelper(string html) {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            _htmlDoc = htmlDocument;
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