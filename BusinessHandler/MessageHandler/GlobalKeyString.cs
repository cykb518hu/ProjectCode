using System.Configuration;

namespace BusinessHandler.MessageHandler
{
    public class GlobalKeyString
    {
        public static string docQueryCacheKey = ConfigurationManager.AppSettings["DocQueryCacheKey"] ?? "docQueryCacheKey";
        public static string roleTypeAdmin = ConfigurationManager.AppSettings["RoleTypeAdmin"] ?? "Admin";
        public static string roleTypeGeneral = ConfigurationManager.AppSettings["RoleTypeGeneral"] ?? "General";
    }
}
