using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.MessageHandler
{
    public class GlobalKeyString
    {
        public static string docQueryCacheKey = ConfigurationManager.AppSettings["DocQueryCacheKey"] ?? "docQueryCacheKey";
        public static string roleTypeAdmin = ConfigurationManager.AppSettings["RoleTypeAdmin"] ?? "Admin";
        public static string roleTypeGeneral = ConfigurationManager.AppSettings["RoleTypeGeneral"] ?? "General";
    }
}
