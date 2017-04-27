using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace BusinessHandler.MessageHandler
{
    public interface IUserRepository
    {
        string Register(UserAccount message);
        void ActiveUser(UserAccount message);
        UserAccount Login(UserAccount message, out string result);

        List<UserAccount> GetUserList();


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
            var userList = ReadFromJsonFile();
            if (userList.Count(x => x.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                result = "This email has been registered";
            }
            else
            {
                message.Password = Base64Encode(message.Password);
                message.RoleType = GlobalKeyString.roleTypeGeneral;
                userList.Add(message);
                SaveToJsonFile(userList);
            }

            return result;
        }

        public void ActiveUser(UserAccount message)
        {
            var userList = ReadFromJsonFile();
            var user = userList.FirstOrDefault(x => x.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase));
            user.Active = message.Active;
            user.Cityes = message.Cityes;
            SaveToJsonFile(userList);

        }

        public UserAccount Login(UserAccount message, out string result)
        {
            result = "sccuess";
            var userList = ReadFromJsonFile();
            var user = userList.FirstOrDefault((x => x.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase) && x.Password.Equals(Base64Encode(message.Password), StringComparison.OrdinalIgnoreCase)));

            if (user != null)
            {
                if (user.Active == "No")
                {
                    result = "Your account it not activated, please reach out to admnistrator";
                }
                else
                {
                    result = "sccuess";
                }
            }
            else
            {
                result = "Your login credentials has proplem, incorrect email or passwrod ";
            }
            return user;
        }
        public List<UserAccount> GetUserList()
        {
            return ReadFromJsonFile();
        }

        public List<UserAccount> ReadFromJsonFile()
        {
            List<UserAccount> userList = new List<Model.UserAccount>();
            try
            {
                var fileName = HttpContext.Current.Server.MapPath("~/App_Data/Users.json");
                var json = File.ReadAllText(fileName);
                var jobj = JArray.Parse(json);
                userList = jobj.Select(x => new UserAccount { Email = x["Email"].ToString(), Password = x["Password"].ToString(), Cityes = x["Cityes"].ToString(), Active = x["Active"].ToString() , RoleType = x["RoleType"].ToString() })
                           .ToList();
            }
            finally
            {
                
            }
            return userList;
        }

        public void SaveToJsonFile(List<UserAccount> userList)
        {
            string json = JsonConvert.SerializeObject(userList.ToArray());

            var fileName = HttpContext.Current.Server.MapPath("~/App_Data/Users.json");
            System.IO.File.WriteAllText(fileName, json);
        }

        public  string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public  string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
