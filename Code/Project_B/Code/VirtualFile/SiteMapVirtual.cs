using System;
using System.Globalization;
using System.Xml;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic.WebFiles;
using Project_B.Code.DataProvider;

namespace Project_B.Code.VirtualFile {
    public class SiteMapVirtual : IVirtualFile {
        private readonly ThriftyKangooSimpleCache<object> _sitemapContent = new ThriftyKangooSimpleCache<object>(BuildFileContent, string.Empty, new TimeSpan(12, 0, 0));

        public string VirtualPath { get { return "~/Sitemap.xml"; } }

        public SiteMapVirtual() {
            var warmContent = GetContent();
        }

        public string GetContent() {
            return (string)_sitemapContent.Object();
        }

        private static string BuildFileContent() {
            var pages = MainProvider.Instance.GetSiteMapItems();
            var doc = new XmlDocument();
            doc.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
            var rootElem = doc.CreateElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            foreach (var page in pages) {
                var url = doc.CreateElement("url");
                var location = doc.CreateElement("loc");
                location.InnerText = page.Location;
                url.AppendChild(location);

                var lastmod = doc.CreateElement("lastmod");
                lastmod.InnerText = page.LastMod.ToString("yyyy-MM-dd");
                url.AppendChild(lastmod);

                var changefreq = doc.CreateElement("changefreq");
                changefreq.InnerText = page.ChangeFreq.ToString().ToLower();
                url.AppendChild(changefreq);

                var priority = doc.CreateElement("priority");
                priority.InnerText = page.Priority.ToString(CultureInfo.InvariantCulture);
                url.AppendChild(priority);

                rootElem.AppendChild(url);
            }
            doc.AppendChild(rootElem);
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                doc.InnerXml
                    .Replace("urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"",
                             "urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\"")
                    .Replace("<url xmlns=\"\">", "<url>");
        }
    }
}