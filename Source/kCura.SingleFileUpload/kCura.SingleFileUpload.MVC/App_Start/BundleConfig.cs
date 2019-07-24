using System.Diagnostics;
using System.Reflection;
using System.Web.Optimization;

namespace kCura.SingleFileUpload.MVC
{
    public class BundleConfig
    {

        private static string _currentVersion;
        public static void RegisterBundles()
        {
            BundleTable.Bundles.Add(new ScriptBundle("~/js/resx").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/angular/angular.js",
                "~/Scripts/Common.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/js/sfuapp").Include(
                "~/Scripts/SingleFileUpload/SFUApp.js",
                "~/Scripts/SingleFileUpload/Controllers/SFUController.js"));


            BundleTable.Bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/Upload.css"));

            #region Enable Optimization
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
            #endregion
        }
        public static string CurrentVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_currentVersion))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    _currentVersion = fvi.FileVersion;
                }
                return _currentVersion;
            }
        }
    }
}