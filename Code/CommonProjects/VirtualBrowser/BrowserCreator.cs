using System.Threading;
using Awesomium.Core;

namespace VirtualBrowser {
    public static class BrowserCreator {
        private static readonly Thread _updater;
        static BrowserCreator() {
            _updater = new Thread(() => {
                WebCore.Initialize(new WebConfig {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0"
                }, true);
                WebCore.ResourceInterceptor = new CustomResourceInterceptor();
                WebCore.Run();
            });
            _updater.Start();
        }

        public static Browser BuildBrowser(BrowserSettings browserSettings) {
            return new Browser(browserSettings);
        }
    }
}