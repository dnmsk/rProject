using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace MainLogic.WebFiles {
    public class WebVirtualFileManager : VirtualPathProvider {
        private readonly Dictionary<string, VirtualFile> _hostedFiles;
        public WebVirtualFileManager() {
            _hostedFiles = new Dictionary<string, VirtualFile>();
            foreach (var instance in Assembly.GetCallingAssembly().GetTypes()
                                                .Where(t => typeof(IVirtualFile).IsAssignableFrom(t) && !t.IsInterface)
                                                .Select(GetActualVirtualFile)
                                                .Where(t => t != null)) {
                _hostedFiles[instance.VirtualPath.ToLower()] = instance;
            }
        }

        private VirtualFile GetActualVirtualFile(Type t) {
            var obj = (IVirtualFile)Activator.CreateInstance(t);
            if (typeof(IVirtualFile).IsAssignableFrom(t)) {
                return new WebVirtualFile(obj);
            }
            return null;
        }

        public override bool FileExists(string virtualPath) {
            var appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

            return IsVirtualFile(appRelativeVirtualPath) || Previous.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath) {
            string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

            if (IsVirtualFile(appRelativeVirtualPath)) {
                return _hostedFiles[virtualPath.ToLower()];
            }
            return Previous.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
            string appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);

            return IsVirtualFile(appRelativeVirtualPath)
                ? null
                : Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        private bool IsVirtualFile(string appRelativeVirtualPath) {
            return _hostedFiles.ContainsKey(appRelativeVirtualPath.ToLower().Trim('~'));
        }

        private string ToAppRelativeVirtualPath(string virtualPath) {
            string appRelativeVirtualPath = VirtualPathUtility.ToAppRelative(virtualPath);

            if (!appRelativeVirtualPath.StartsWith("~/")) {
                throw new HttpException("Unexpectedly does not start with ~.");
            }

            return appRelativeVirtualPath;
        }
    }
}
