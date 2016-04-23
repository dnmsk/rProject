using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Core;

namespace VirtualBrowser {
    class CustomResourceInterceptor : IResourceInterceptor {
        private static readonly HashSet<string> _enabledExtensions = new HashSet<string> {
            ".html", ".htm", /*".css",/**/ ".js", ".php", ".asp", ".aspx"
        };

        public ResourceResponse OnRequest(ResourceRequest request) {
            var path = request.Url.ToString().ToLower();
            
            try {
                path = path.IndexOf("?", StringComparison.InvariantCultureIgnoreCase) >= 0 ? path.Substring(0, path.IndexOf("?", StringComparison.InvariantCultureIgnoreCase)) : path;
                var fileName = path.Substring(path.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1);
                var ext = Path.GetExtension(fileName);
                var badFileName = !string.IsNullOrEmpty(ext) && !_enabledExtensions.Contains(ext.ToLower());
                if (badFileName) {
                    request.Cancel();
                    return null;
                }
            } catch (Exception e) {
                request.Cancel();
                return null;
            }

            return null;
        }

        public bool OnFilterNavigation(NavigationRequest request) {
            return false;
        }
    }
}
