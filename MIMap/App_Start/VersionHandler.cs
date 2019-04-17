using BusinessHandler.Model;
using System.Web;

namespace MIMap.App_Start
{
    public class VersionHandler:IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }
        public void ProcessRequest(HttpContext context)
        {
            var path = context.Request.Url.PathAndQuery;
            if(string.IsNullOrEmpty(path)|| path.Contains("?version"))
            {
                return;
            }
            if(path.Equals(".js")||path.EndsWith(".css"))
            {
                var version = StaticSetting.version;
                path += "?version=" + version;
                context.Response.Redirect(path);
            }
            context.ApplicationInstance.CompleteRequest();
        }
    }
}