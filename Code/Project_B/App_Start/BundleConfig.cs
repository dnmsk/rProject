using Project_B.Code;
using SquishIt.CoffeeScript;
using SquishIt.Framework;
using SquishIt.Less;

namespace Project_B {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles() {
            Bundle.RegisterStylePreprocessor(new LessPreprocessor());
            Bundle.RegisterScriptPreprocessor(new CoffeeScriptPreprocessor());
            SquishItMinifierStatic.Css
                                  .AddMinified("~/Content/bootstrap.min.css")
                                  .Add("~/Content/site.css")
                                  //.Add("~/Content/Less/test.less")
                                  .AsCached();
            SquishItMinifierStatic.JavaScript
                                  .AddMinified("~/Scripts/jquery-2.1.4.min.js")
                                  .AddMinified("~/Scripts/jquery.validate.min.js")
                                  .AddMinified("~/Scripts/react/react-0.13.1.min.js")
                                  .AddMinified("~/Scripts/react/react-0.13.1.min.js")
                                  .Add("~/Scripts/modernizr-2.8.3.js")
                                  .Add("~/Scripts/bootstrap.js")
                                  .Add("~/Scripts/respond.js")
                                  .AsCached();
            /*
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));*/
        }
    }
}
