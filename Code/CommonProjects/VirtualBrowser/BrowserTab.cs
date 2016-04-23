using System;
using System.Threading;
using Awesomium.Core;

namespace VirtualBrowser {
    public class BrowserTab : IDisposable {
        private readonly WebSession _session;
        private WebView _webView;
        internal BrowserTab(BrowserSettings browserSettings, WebSession session) {
            _session = session;
            WebCore.DoWork(() => {
                _webView = WebCore.CreateWebView(browserSettings.WindowWidth, browserSettings.WindowHeight, session, WebViewType.Offscreen);
                return default(int);
            });
        }

        public void OpenPage(string url) {
            WebCore.QueueWork(() => {
                _webView.Source = new Uri(url);
            });
            Thread.Sleep(5000);
        }

        private const int _triesToGetContent = 10;
        public string PageContent() {
            string content = null;
            for (var i = 0; i < _triesToGetContent; i++) {
                content = WebCore.DoWork(() => _webView.IsDocumentReady ? _webView.ExecuteJavascriptWithResult("document.documentElement.outerHTML").ToString() : string.Empty);
                if (content?.Length > 0) {
                    break;
                }
                Thread.Sleep(1000);
            }
            return content ?? string.Empty;
        }

        public void Dispose() {
            WebCore.QueueWork(() => {
                _webView?.Dispose();
            });
        }
    }
}
