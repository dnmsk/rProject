using System;
using System.Collections.Generic;
using Awesomium.Core;

namespace VirtualBrowser {
    public class Browser : IDisposable {
        private readonly BrowserSettings _browserSettings;
        private readonly HashSet<BrowserTab> _tabs = new HashSet<BrowserTab>();
        private WebSession _session;

        public Browser(BrowserSettings browserSettings) {
            _browserSettings = browserSettings;
            var preferences = new WebPreferences();
            if (browserSettings.Proxy?.Length > 0) {
                preferences.ProxyConfig = browserSettings.Proxy;
            }
            WebCore.DoWork(() => {
                _session = WebCore.CreateWebSession(preferences);
                return default(int);
            });
        }
        
        public void SetCookie(string domain, string[] strings) {
            foreach (var path in new[] {"http://", "https://", "http://www.", "https://www"}) {
                var uri = new Uri(path + domain);
                WebCore.DoWork(() => {
                    foreach (var cookie in strings) {
                        _session.SetCookie(uri, cookie, false, false);
                    }
                    return default(int);
                });
            }
        }

        public BrowserTab GetNewTab() {
            var tab = new BrowserTab(_browserSettings, _session);
            _tabs.Add(tab);
            return tab;
        }

        public void CloseTab(BrowserTab browserTab) {
            if (!_tabs.Contains(browserTab)) {
                return;
            }
            _tabs.Remove(browserTab);
            browserTab.Dispose();
        }
        
        public void Dispose() {
            foreach (var browserTab in _tabs) {
                browserTab.Dispose();
            }
            WebCore.QueueWork(() => {
                _session?.Dispose();
            });
        }
    }
}
