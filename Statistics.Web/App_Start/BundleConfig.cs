using System.Web.Optimization;

namespace ZondervanLibrary.Statistics.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/styles/layout.css", new LessTransform(), new CssMinify()).Include("~/Assets/Styles/Layout.less"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}
