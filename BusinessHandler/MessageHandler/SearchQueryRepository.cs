using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace BusinessHandler.MessageHandler
{

    public interface ISearchQueryRepository
    {
        void AddSearchQuery(string query,string title);
        List<SearchQueryModel> GetSearchQuery();
    }
    public class SearchQueryRepository: ISearchQueryRepository
    {
        XmlSerializer serializer;
        string fileName = string.Empty;
        public SearchQueryRepository()
        {
            fileName = HttpContext.Current.Server.MapPath("~/File/SearchQuery.xml");
            serializer = new XmlSerializer(typeof(List<SearchQueryModel>), new XmlRootAttribute("SearyQueries"));
        }
        public void AddSearchQuery(string query,string title)
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
                }
            }
            return dezerializedList;
        }
    }
}
