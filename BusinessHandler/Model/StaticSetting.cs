using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.Model
{
   public static class StaticSetting
    {
        public static bool GetDataFromDB()
        {
            if(ConfigurationManager.AppSettings["GetDataFromDB"]!=null&& ConfigurationManager.AppSettings["GetDataFromDB"].ToString()=="True")
            {
                return true;
            }
            return false;
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ToString();

        public static string filePath = ConfigurationManager.AppSettings["FilePath"].ToString();

        public static string queryFile = ConfigurationManager.AppSettings["queryFile"].ToString();

        public static string userFile = ConfigurationManager.AppSettings["userFile"].ToString();

        public static List<string> MapColorList()
        {
            List<string> list = new List<string>();
            list.Add("#00FF7F"); //<50
            list.Add("#00EE76");//<200
            list.Add("#00CD66"); //<500
            list.Add("#008B45"); //>500
            return list;
        }
    }
}
