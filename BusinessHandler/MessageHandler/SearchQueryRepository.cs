using BusinessHandler.Model;
using System;
using System.Collections.Generic;
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
}
