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
using System.Data;

namespace BusinessHandler.MessageHandler
{
    public interface IUserRepository
    {
        string Register(UserAccount message);
        void ActiveUser(UserAccount message);
        UserAccount Login(UserAccount message, out string result);

        List<UserAccount> GetUserList();

        void SaveUser(UserAccount message);


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
                    data.AddDate = Convert.ToDateTime(reader["USR_CRTN_TS"]).ToString("yyyy-MM-dd");
                    userList.Add(data);
                }
            }
            foreach(var r in userList)
            {
                SubList(r);
            }
           
            return userList;
        }

        public  void SubList(UserAccount user)
        {
            var userStr = string.Empty;
            user.CityList = new List<string>();

            var queryString = @"SELECT * FROM DBO.ACCOUNT_CITY WHERE EMAIL = @EMAIL";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@EMAIL", user.Email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cityGuid = DBNull.Value == reader["City_Guid"] ? "" : reader["City_Guid"].ToString().ToUpper();
                        if(!string.IsNullOrWhiteSpace(cityGuid))
                        {
                            user.CityList.Add(cityGuid);
                        }
                    }
                }
            }
        }


        public void SaveUser(UserAccount message)
        {
            string queryString = @"UPDATE ACCOUNT SET ACTIVE=@Active where email='" + message.Email + "'";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Active", message.Active);
                connection.Open();
                command.ExecuteNonQuery();

            }

            queryString= @"DELETE FROM ACCOUNT_CITY WHERE email='" + message.Email + "'";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
            if(message.CityList.Any())
            {
                var dt = new DataTable();
                dt.Columns.Add("City_Guid");
                dt.Columns.Add("Email");
                foreach(var r in message.CityList)
                {
                    dt.Rows.Add(r,message.Email);
                }
                using (var sqlBulk = new SqlBulkCopy(StaticSetting.connectionString))
                {
                    sqlBulk.DestinationTableName = "DBO.ACCOUNT_CITY";
                    sqlBulk.WriteToServer(dt);
                }

            }
        }


    }

}

