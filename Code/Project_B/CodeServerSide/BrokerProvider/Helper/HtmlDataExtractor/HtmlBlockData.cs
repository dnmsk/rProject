using HtmlAgilityPack;

namespace Project_B.CodeServerSide.BrokerProvider.Helper.HtmlDataExtractor {
    public class HtmlBlockData {
        public HtmlNode Node { get; }

        public HtmlBlockData(HtmlNode node) {
            Node = node;
        }
    }
}