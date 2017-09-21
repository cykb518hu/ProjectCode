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
using System.Data.SqlClient;

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
                var fileName = StaticSetting.userFile;
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

            var fileName = StaticSetting.userFile;
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

    public class SqlServerUserRepository:IUserRepository
    {
        public string Register(UserAccount message)
        {
            var result = "sccuess";
            var userList = GetUserList();
            if (userList.Count(x => x.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase)) > 0)
            {
                result = "This email has been registered";
            }
            else
            {
                message.Password =StaticSetting.Base64Encode(message.Password);
                message.RoleType = GlobalKeyString.roleTypeGeneral;
                string queryString = @"INSERT INTO dbo.ACCOUNT (EMAIL,Password,Active,RoleType) values (@Email,@Password,@Active,@RoleType)";
                using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@Email", message.Email);
                    command.Parameters.AddWithValue("@Password", message.Password);
                    command.Parameters.AddWithValue("@Active", message.Active);
                    command.Parameters.AddWithValue("@RoleType", message.RoleType);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return result;
        }

        public void ActiveUser(UserAccount message)
        {
           

        }

        public UserAccount Login(UserAccount message, out string result)
        {
            result = "sccuess";
            var userList = GetUserList();
            var user = userList.FirstOrDefault((x => x.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase) && x.Password.Equals(StaticSetting.Base64Encode(message.Password), StringComparison.OrdinalIgnoreCase)));

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
            List<UserAccount> userList = new List<UserAccount>();
            string queryString = @"SELECT * FROM ACCOUNT";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new UserAccount();
                    data.Email = DBNull.Value == reader["EMAIL"] ? "" : reader["EMAIL"].ToString();
                    data.Password = DBNull.Value == reader["Password"] ? "" : reader["Password"].ToString();
                    data.Cityes = DBNull.Value == reader["Cityes"] ? "" : reader["Cityes"].ToString();
                    data.Active = DBNull.Value == reader["Active"] ? "" : reader["Active"].ToString();
                    data.RoleType = DBNull.Value == reader["RoleType"] ? "" : reader["RoleType"].ToString();
                    data.Operation = DBNull.Value == reader["Operation"] ? "" : reader["Operation"].ToString();
                    userList.Add(data);
                }
            }
            return userList;
        }

     
    }

}

