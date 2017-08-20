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
    }
}
