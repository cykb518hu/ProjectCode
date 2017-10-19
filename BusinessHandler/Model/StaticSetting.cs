using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

        public static void BuildParameters(SqlCommand command, DocQueryMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message.CityName) && !message.CityName.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@CityName", StaticSetting.GetArrayQuery(message.CityName));
            }
            if (!string.IsNullOrWhiteSpace(message.CountyName) && !message.CountyName.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@CountyName", StaticSetting.GetArrayQuery(message.CountyName));
            }
            if (!string.IsNullOrWhiteSpace(message.KeyWord) && !message.KeyWord.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@KeyWord", StaticSetting.GetArrayQuery(message.KeyWord));
            }
            if (!string.IsNullOrWhiteSpace(message.MeetingDate))
            {
                command.Parameters.AddWithValue("@StartMeetingDate", message.MeetingDate);
            }
            if (!string.IsNullOrWhiteSpace(message.StartMeetingDate))
            {
                command.Parameters.AddWithValue("@StartMeetingDate", message.StartMeetingDate);
            }
            if (!string.IsNullOrWhiteSpace(message.EndMeetingDate))
            {
                command.Parameters.AddWithValue("@EndMeetingDate", message.EndMeetingDate);
            }
            if (!string.IsNullOrWhiteSpace(message.DeployDate) && !message.DeployDate.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@DeployeDate", StaticSetting.GetArrayQuery(message.DeployDate));
            }
            if (!string.IsNullOrWhiteSpace(message.IsViewed) && !message.IsViewed.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var isChecked = message.IsViewed == "Yes" ? "True" : "False";
                command.Parameters.AddWithValue("@IsChecked", isChecked);
            }
            //important means removed
            if (!string.IsNullOrWhiteSpace(message.Important) && !message.Important.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var isImportant = message.Important == "Yes" ? "True" : "False";
                command.Parameters.AddWithValue("@IsImportant", isImportant);
            }

            var email = StaticSetting.GetUserEmail();
            if (!string.IsNullOrWhiteSpace(email))
            {
                command.Parameters.AddWithValue("@UserEmail", email);
            }
            if (!string.IsNullOrWhiteSpace(message.State))
            {
                command.Parameters.AddWithValue("@State", message.State);
            }
        }
    }
}
