using kCura.SingleFileUpload.Core.Managers.Implementation;
using NSerio.Relativity;
using Relativity.CustomPages;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace kCura.SingleFileUpload.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            RepositoryHelper.ConfigureRepository(ConnectionHelper.Helper());
            try
            {
                SearchExportManager.Instance.ConfigureOutsideIn();
            }
            catch (System.Exception ex)
            {

                SearchExportManager.Instance.LogError(ex);
            }
            
        }


      
    }
}