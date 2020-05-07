using BusinessHandler.MessageHandler;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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

        public static string dynamicPriceDBconnectionString = ConfigurationManager.ConnectionStrings["DynamicPriceDB"].ToString();

        public static string version = string.IsNullOrEmpty(ConfigurationManager.AppSettings["version"]) ? "" : ConfigurationManager.AppSettings["version"].ToString();

        public static string uploadPath= string.IsNullOrEmpty(ConfigurationManager.AppSettings["uploadPath"]) ? "" : ConfigurationManager.AppSettings["uploadPath"].ToString();

        public static string DefaultTags= string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultTags"]) ? "" : ConfigurationManager.AppSettings["DefaultTags"].ToString();

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

        public static string GetKeyWordForFullSearch(string keyWord)
        {
            var result = "";
            foreach (var r in keyWord.Split(','))
            {
                result += "\"" + r + "\" or ";
            }
            if(result.Length>0)
            {
                result = result.Substring(0, result.Length - 3);
            }
            return result;
        }

        public static string GetKeyWordForFullSearch()
        {
            var keyWordRepository = new KeyWordRepository();
            var keyWord = keyWordRepository.GetKeyWords();
            return GetKeyWordForFullSearch(keyWord);
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
            if (string.IsNullOrWhiteSpace(message.KeyWord) || message.KeyWord.Contains("All"))
            {
                var keyWordRepository = new KeyWordRepository();
                message.KeyWord = keyWordRepository.GetKeyWords();  
            }
            if (!string.IsNullOrWhiteSpace(message.KeyWord))
            {
                message.KeyWord = StaticSetting.GetKeyWordForFullSearch(message.KeyWord);
                command.Parameters.AddWithValue("@KeyWord", message.KeyWord);
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
            if (!string.IsNullOrWhiteSpace(message.ObjectIds))
            {
                command.Parameters.AddWithValue("@ObjectIds", message.ObjectIds);
            }

            if (!string.IsNullOrWhiteSpace(message.FacilityType) )
            {
                message.OptStatus = "OptIn";
                var arr = message.FacilityType.Split(',');
                var facilityStr = "";
                foreach(var r in arr)
                {
                    facilityStr += string.Format(" and CO.{0} is not null and CO.{0}<>'' and CO.{0}<>'0' ", r);
                }
                command.Parameters.AddWithValue("@FacilityType", facilityStr);
            }
            if (!string.IsNullOrWhiteSpace(message.OptStatus) && !message.OptStatus.Split(',').Any(x => x.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                command.Parameters.AddWithValue("@OptStatus", StaticSetting.GetArrayQuery(message.OptStatus));
            }
        }

        
    }
}
