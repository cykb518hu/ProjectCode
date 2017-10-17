using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessHandler.Model
{
    public static class StaticSetting
    {
        static StaticSetting()
        {
        }
        public static bool GetDataFromDB()
        {
            if (ConfigurationManager.AppSettings["GetDataFromDB"] != null && ConfigurationManager.AppSettings["GetDataFromDB"].ToString() == "True")
            {
                return true;
            }
            return false;
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ToString();

        public static string filePath = string.IsNullOrEmpty(ConfigurationManager.AppSettings["FilePath"]) ? "" : ConfigurationManager.AppSettings["FilePath"].ToString();

        public static string userFile = string.IsNullOrEmpty(ConfigurationManager.AppSettings["userFile"]) ? "" : ConfigurationManager.AppSettings["userFile"].ToString();


        public static string version = string.IsNullOrEmpty(ConfigurationManager.AppSettings["version"]) ? "" : ConfigurationManager.AppSettings["version"].ToString();

        public static string uploadPath= string.IsNullOrEmpty(ConfigurationManager.AppSettings["uploadPath"]) ? "" : ConfigurationManager.AppSettings["uploadPath"].ToString();

        public static string DefaultTags= string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultTags"]) ? "" : ConfigurationManager.AppSettings["DefaultTags"].ToString();

        public static List<string> MapColorList()
        {
            List<string> list = new List<string>();
            list.Add("#00FF7F"); //<50
            list.Add("#00EE76");//<200
            list.Add("#00CD66"); //<500
            list.Add("#008B45"); //>500
            return list;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string GetArrayQuery(string arrayStr)
        {
            var arr = arrayStr.Split(',');
            var query = string.Empty;
            foreach (var r in arr)
            {
                if (!string.IsNullOrWhiteSpace(r))
                {
                    if (r.Equals("All", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                    query += "'" + r + "',";
                }
            }
            query = query.TrimEnd(',');
            return query;
        }


        public static string GetUserEmail()
        {

            var user = (UserAccount)HttpContext.Current.Session["UserAccount"];
            if(user!=null)
            {
                return user.Email;
            }
            return "";
        }
    }
}
