using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using CommonUtils.ExtendedTypes;
using SquishIt.Framework;
using SquishIt.Framework.Base;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace Project_B.CodeClientSide {
    public class SquishItMinifierStatic : Singleton<SquishItMinifierStatic> {
        public const string MAIN = "main";
        private static readonly bool _isDebug = ((CompilationSection) ConfigurationManager.GetSection("system.web/compilation")).Debug;
        private readonly Dictionary<string, SquishItMinifier<CSSBundle>> _dictionaryCssBundles = new Dictionary<string, SquishItMinifier<CSSBundle>>();
        private readonly Dictionary<string, SquishItMinifier<JavaScriptBundle>> _dictionaryJsBundles = new Dictionary<string, SquishItMinifier<JavaScriptBundle>>();

        public SquishItMinifier<CSSBundle> Css(string key) {
            SquishItMinifier<CSSBundle> css;
            if (!_dictionaryCssBundles.TryGetValue(key, out css)) {
                css = SquishItMinifier<CSSBundle>.Css(_isDebug);
                _dictionaryCssBundles[key] = css;
            }
            return css;
        }

        public SquishItMinifier<JavaScriptBundle> JavaScript(string key) {
            SquishItMinifier<JavaScriptBundle> js;
            if (!_dictionaryJsBundles.TryGetValue(key, out js)) {
                js = SquishItMinifier<CSSBundle>.JavaScript(_isDebug);
                _dictionaryJsBundles[key] = js;
            }
            return js;
        }

        public MvcHtmlString MvcRenderCachedAssetTag(IEnumerable<string> assets = null) {
            assets = assets != null ? assets.Distinct() : new [] { MAIN };
            var result = new MvcHtmlString(string.Empty);
            foreach (var asset in assets) {
                SquishItMinifier<CSSBundle> css;
                if (_dictionaryCssBundles.TryGetValue(asset, out css)) {
                    result = new MvcHtmlString(result.ToHtmlString() + css.MvcRenderCachedAssetTag(asset).ToHtmlString());
                }
                SquishItMinifier<JavaScriptBundle> js;
                if (_dictionaryJsBundles.TryGetValue(asset, out js)) {
                    result = new MvcHtmlString(result.ToHtmlString() + js.MvcRenderCachedAssetTag(asset).ToHtmlString());
                }
            }
            return result;
        }
    }
    public class SquishItMinifier<T> where T : BundleBase<T> {
        private readonly string _path;
        private readonly BundleBase<T> _bundle;
        private SquishItMinifier(string path, BundleBase<T> bundle) {
            _path = path;
            _bundle = bundle;
        }

        public SquishItMinifier<T> Add(string path) {
            _bundle.Add(path);
            return this;
        }

        public SquishItMinifier<T> AddDirectory(string path, bool recursive = true) {
            _bundle.AddDirectory(path, recursive);
            return this;
        }

        public SquishItMinifier<T> AddDirectoryMinified(string path, bool recursive = true) {
            _bundle.AddMinifiedDirectory(path, recursive);
            return this;
        }

        public SquishItMinifier<T> AddMinified(string path) {
            _bundle.AddMinified(path);
            return this;
        }

        public void AsCached(string bundleScope) {
            _bundle.ClearCache();
            _bundle.AsCached(bundleScope, string.Format("~/assets/{0}/{1}?#", _path, bundleScope));
        }

        public SquishItMinifier<T> AddString(string content) {
            _bundle.AddString(content);
            return this;
        }

        public MvcHtmlString MvcRenderCachedAssetTag(string bundleScope) {
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