using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessHandler.MessageHandler
{

    public interface ISearchQueryRepository
    {
        void AddSearchQuery(string query, string title);
        List<SearchQueryModel> GetSearchQuery();

        void UpdateSearchQuery(string guid, string title,string query);

        void DeleteSearchQuery(string guid);

        void AddSearchQueryAmount(string guid);
    }
    public class SearchQueryRepository : ISearchQueryRepository
    {
        XmlSerializer serializer;
        string fileName = string.Empty;
        public SearchQueryRepository(string file = "")
        {
            if (string.IsNullOrEmpty(file))
            {
                fileName = HttpContext.Current.Server.MapPath("~/File/SearchQuery.xml");
            }
            else
            {
                fileName = file;
            }
            serializer = new XmlSerializer(typeof(List<SearchQueryModel>), new XmlRootAttribute("SearyQueries"));
        }
        public void AddSearchQuery(string query, string title)
        {
            var dezerializedList = new List<SearchQueryModel>();
            using (FileStream stream = File.OpenRead(fileName))
            {
                dezerializedList = (List<SearchQueryModel>)serializer.Deserialize(stream);

                var queryModel = new SearchQueryModel();
                queryModel.Title = title;
                queryModel.Content = query;
                dezerializedList.Add(queryModel);
            }
            using (FileStream stream = File.OpenWrite(fileName))
            {
                serializer.Serialize(stream, dezerializedList);
            }
        }

        public List<SearchQueryModel> GetSearchQuery()
        {
            var dezerializedList = new List<SearchQueryModel>();
            using (FileStream stream = File.OpenRead(fileName))
            {
                dezerializedList = (List<SearchQueryModel>)serializer.Deserialize(stream);
                if (dezerializedList.Any())
                {
                    dezerializedList = dezerializedList.Where(x => x.Disabled == false && !string.IsNullOrWhiteSpace(x.Title)).ToList();
                    dezerializedList.OrderByDescending(x => x.FrequentlyUsed).ThenByDescending(x => x.ModifyDate).ToList();
                }
            }
            return dezerializedList;
        }

        public void UpdateSearchQuery(string guid, string title,string query)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode myNode = doc.SelectNodes("SearyQueries/SearchQuery").OfType<XmlNode>().FirstOrDefault(n => n["Guid"].InnerText == guid);
            if (myNode != null)
            {
                myNode["Title"].InnerText = title;
                myNode["Content"].InnerText = query;
                myNode["ModifyDate"].InnerText = DateTime.Now.ToString("o");
                doc.Save(fileName);
            }
        }

        public void DeleteSearchQuery(string guid)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode myNode = doc.SelectNodes("SearyQueries/SearchQuery").OfType<XmlNode>().FirstOrDefault(n => n["Guid"].InnerText == guid);
            if (myNode != null)
            {
                myNode.ParentNode.RemoveChild(myNode);
                doc.Save(fileName);
            }
        }
        public void AddSearchQueryAmount(string guid)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode myNode = doc.SelectNodes("SearyQueries/SearchQuery").OfType<XmlNode>().FirstOrDefault(n => n["Guid"].InnerText == guid);
            if (myNode != null)
            {
                var amount = Convert.ToInt32(myNode["FrequentlyUsed"].InnerText);
                amount++;
                myNode["ModifyDate"].InnerText = DateTime.Now.ToString("o");
                myNode["FrequentlyUsed"].InnerText = amount.ToString();
                doc.Save(fileName);
            }
        }

    }

    public class SqlServerSearchQueryRepository:ISearchQueryRepository
    {
        public void AddSearchQuery(string query, string title)
        {
            var queryModel = new SearchQueryModel();
            string queryString = @"INSERT INTO dbo.SearchQuery (Guid,Title,Content,FrequentlyUsed,Disabled) values (@Guid,@Title,@Content,1,'false')";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Guid", queryModel.Guid);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Content", query);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<SearchQueryModel> GetSearchQuery()
        {
            var list = new List<SearchQueryModel>();
            string queryString = @"SELECT * FROM SearchQuery where disabled='false'";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var data = new SearchQueryModel();
                    data.Guid = DBNull.Value == reader["Guid"] ? "" : reader["Guid"].ToString();
                    data.Title = DBNull.Value == reader["Title"] ? "" : reader["Title"].ToString();
                    data.Content = DBNull.Value == reader["Content"] ? "" : reader["Content"].ToString();
                    data.FrequentlyUsed = DBNull.Value == reader["FrequentlyUsed"] ? 1 : Convert.ToInt32(reader["FrequentlyUsed"]);
                    data.ModifyDate = DBNull.Value == reader["USR_MDFN_TS"] ? DateTime.MinValue : Convert.ToDateTime(reader["USR_MDFN_TS"]);
                    list.Add(data);
                }
            }
            list = list.OrderByDescending(x => x.FrequentlyUsed).ThenByDescending(x => x.ModifyDate).ToList();
            return list;
        }

        public void UpdateSearchQuery(string guid, string title, string query)
        {
            string queryString = @"UPDATE dbo.SearchQuery SET Title=@Title, Content=@Content WHERE GUID=@Guid ";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Guid", guid);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Content", query);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void DeleteSearchQuery(string guid)
        {
            string queryString = @"DELETE FROM dbo.SearchQuery WHERE GUID=@Guid ";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Guid", guid);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void AddSearchQueryAmount(string guid)
        {
           string queryString = @"UPDATE SearchQuery SET FrequentlyUsed = FrequentlyUsed + 1 WHERE GUID=@Guid ";
            using (SqlConnection connection = new SqlConnection(StaticSetting.connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Guid", guid);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
