using System.Web;
using System.Web.Optimization;

namespace FIX.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

#if !DEBUG
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/bundles/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/bundles/jqueryval.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/bundles/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bundles/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                      "~/Scripts/bundles/app.js"));

            bundles.Add(new ScriptBundle("~/Content/main").Include(
                      "~/Scripts/bundles/main.js"));
#else

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                      "~/Scripts/app/extension.js",
                      "~/Scripts/app/settings.js",
                      "~/Scripts/app/styles.js",
                      "~/Scripts/app/bootstrap-table.custom.js",
                      "~/Scripts/app/bootstrap-modal.custom.js"));
#endif

            bundles.Add(new StyleBundle("~/Content/style").Include(
                      "~/Content/styles.min.css"));
        }

    }
}
