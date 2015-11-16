using Project_B.CodeClientSide;
using SquishIt.CoffeeScript;
using SquishIt.Framework;
using SquishIt.Less;

namespace Project_B {
    public class BundleConfig {
        public static void RegisterBundles() {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
            SquishItMinifierStatic.Instance.Css(SquishItMinifierStatic.MAIN)
                                  .AddMinified("~/Content/bootstrap.min.css")
                                  .Add("~/Content/site.css")
                                  //.Add("~/Content/Less/test.less")
                                  .AsCached(SquishItMinifierStatic.MAIN);
            SquishItMinifierStatic.Instance.JavaScript(SquishItMinifierStatic.MAIN)
                                  .AddMinified("~/Scripts/jquery-2.1.4.min.js")
                                  .AddMinified("~/Scripts/jquery.validate.min.js")
                                  .AddMinified("~/Scripts/react/react-0.13.1.min.js")
                                  .AddMinified("~/Scripts/react/react-0.13.1.min.js")
                                  .Add("~/Scripts/modernizr-2.8.3.js")
                                  .Add("~/Scripts/bootstrap.js")
                                  .Add("~/Scripts/respond.js")
                                  .AsCached(SquishItMinifierStatic.MAIN);
        }
    }
}
