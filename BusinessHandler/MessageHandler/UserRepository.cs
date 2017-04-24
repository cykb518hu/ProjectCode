using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessHandler.MessageHandler
{
    public interface IUserRepository
    {
        string Register(UserAccount message);
        
    }

    public class UserRepository:IUserRepository
    {
        XmlSerializer serializer;
        public UserRepository()
        {
            serializer = new XmlSerializer(typeof(List<UserAccount>));
        }
        public string Register(UserAccount message)
        {
            var result = "sccuess";
            var fileName = @"C:\TestCode\Project\SingleApplication\App_Data\Users.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            List<UserAccount> userList = ReadFromXmlFile<List<UserAccount>>(fileName);
            return result;
        }

        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
