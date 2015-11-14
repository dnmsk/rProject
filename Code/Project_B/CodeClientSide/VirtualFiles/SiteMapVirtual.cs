using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using CommonUtils.WatchfulSloths.KangooCache;
using MainLogic;
using MainLogic.WebFiles;
using Project_B.CodeServerSide.DataProvider;
using Project_B.CodeServerSide.DataProvider.DataHelper;

namespace Project_B.CodeClientSide.VirtualFiles {
    public class SiteMapVirtual : IVirtualFile {
        private readonly MultipleKangooCache<bool, string> _sitemapContent = new MultipleKangooCache<bool, string>(MainLogicProvider.WatchfulSloth,
            b => {
                b[true] = BuildFileContent();
            }, TimeSpan.FromHours(12));

        public string VirtualPath => "~/sitemap.xml";

        public string GetContent() {
            return _sitemapContent[true];
        }

        private static string BuildFileContent() {
            var pages = ProjectProvider.Instance.GetSiteMapItems();
            var doc = new XmlDocument();
            doc.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
            var rootElem = doc.CreateElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            var langsWithSlash = LanguageTypeHelper.Instance.GetIsoNames().Select(iso => "http://" + SiteConfiguration.ProductionHostName + "/" + iso).ToArray();
            foreach (var page in pages) {
                foreach (var lang in langsWithSlash) {
                    var url = doc.CreateElement("url");
                    var location = doc.CreateElement("loc");
                    location.InnerText = lang + page.Location;
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