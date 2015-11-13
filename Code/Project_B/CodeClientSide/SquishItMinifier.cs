using System.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using SquishIt.Framework;
using SquishIt.Framework.Base;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace Project_B.CodeClientSide {
    public static class SquishItMinifierStatic {
        public static readonly bool IsDebug = ((CompilationSection) ConfigurationManager.GetSection("system.web/compilation")).Debug;
        public static SquishItMinifier<CSSBundle> Css {
            get {
                return SquishItMinifier<CSSBundle>.Css(IsDebug);
            }
        }

        public static SquishItMinifier<JavaScriptBundle> JavaScript {
            get {
                return SquishItMinifier<JavaScriptBundle>.JavaScript(IsDebug);
            }
        } 
    }
    public class SquishItMinifier<T> where T : BundleBase<T> {
        public const string MAIN = "main";
        private readonly string _path;
        private readonly string _assetScope;
        private readonly BundleBase<T> _bundle;
        private SquishItMinifier(string path, BundleBase<T> bundle) {
            _path = path;
            _assetScope = MAIN + path;
            _bundle = bundle;
        }

        public SquishItMinifier<T> Add(string path) {
            _bundle.Add(path);
            return this;
        }

        public SquishItMinifier<T> AddMinified(string path) {
            _bundle.AddMinified(path);
            return this;
        }

        public void AsCached(string bundleScope = null) {
            bundleScope = bundleScope ?? _assetScope;
            _bundle.AsCached(bundleScope, string.Format("~/assets/{0}/{1}?#", _path, bundleScope));
        }

        public MvcHtmlString MvcRenderCachedAssetTag(string bundleScope = null) {
            bundleScope = bundleScope ?? _assetScope;
            return MvcHtmlString.Create(_bundle.RenderCachedAssetTag(bundleScope));
        }

        /*
        public SquishItMinifier(Action<SquishItMinifier> fillRule) {
            _css = Bundle.Css()
                .AppendHashForAssets();
            _js = Bundle.JavaScript();
            if (!((CompilationSection)ConfigurationManager.GetSection("system.web/compilation")).Debug) {
                _js = _js.ForceRelease();
            }
            fillRule(this);
        }
        */
        public static SquishItMinifier<JavaScriptBundle> JavaScript(bool isDebug) {
            var javaScriptBundle = Bundle.JavaScript();
            return new SquishItMinifier<JavaScriptBundle>("js", isDebug ? javaScriptBundle : javaScriptBundle.ForceRelease());
        }

        public static SquishItMinifier<CSSBundle> Css(bool isDebug) {
            var cssBundle = Bundle.Css()
                .AppendHashForAssets();
            return new SquishItMinifier<CSSBundle>("css", isDebug ? cssBundle : cssBundle.ForceRelease());
        }
    }
}