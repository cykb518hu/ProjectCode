using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BusinessHandler.MessageHandler
{

    public interface IDataFileHelper
    {
        List<DocData> OpenDoc(string filePath);
        List<QueryData> OpenQuery(string filePath);
        void UpdateQuery(DocQueryResultModel message);
        void UpdateDocStauts(DocQueryResultModel message);



    }
   public class XmlHelper: IDataFileHelper
    {
        public  List<DocData> OpenDoc(string filePath)
        {
            var resultList = new List<DocData>();
            XDocument xdoc = XDocument.Load(filePath);
            resultList = (from lv1 in xdoc.Descendants("Doc")
                                  select new DocData
                                  {
                                      CityName = lv1.Element("CityId").Value,
                                      DocId = lv1.Element("DocId").Value,
                                      DocType = lv1.Element("DocType").Value,
                                      LocalPath = lv1.Element("DocLocalPath").Value,
                                      DocUrl = lv1.Element("DocSource").Value,
                                      CanBeRead = lv1.Element("Readable").Value,
                                      IsViewed = lv1.Element("Checked").Value,
                                      Important= lv1.Element("Important").Value,
                                      DocFilePath = filePath
                                  }).ToList();
            return resultList;
        }

        public  List<QueryData> OpenQuery(string filePath)
        {
            var resultList = new List<QueryData>();

            var xmlDataList = new List<QueryXmlData>();

            XDocument xdoc = XDocument.Load(filePath);
            xmlDataList = (from lv1 in xdoc.Descendants("Query")
                          select new QueryXmlData
                          {
                              CityName = lv1.Element("CityId").Value,
                              DocId = lv1.Element("DocId").Value,

                              MeetingDate = DateTime.Parse(lv1.Element("MeetingDate").Value),
                              MeetingDateDisplay = DateTime.Parse(lv1.Element("MeetingDate").Value).ToString("yyyy/MM/dd"),
                              SearchDate = DateTime.Parse(lv1.Element("SearchDate").Value).ToString("yyyy/MM/dd"),
                              MeetingTitle = lv1.Element("MeetingTitle").Value,
                              MeetingLocation = lv1.Element("MeetingLocation").Value,
                              Entries = (from lv2 in lv1.Element("Entries").Elements("Entry")
                                         select new QueryEntryXmlData
                                         {

                                             KeyWord = lv2.Element("Keyword").Value,
                                             PageNumber = lv2.Element("PageNumber").Value,
                                             ContentList = (from lv3 in lv2.Element("Contents").Elements("Content")
                                                            select new QueryContentXmlData
                                                            {
                                                                Content = lv3.Value,
                                                                QueryGuid =lv3.Attribute("GUID").Value,
                                                                Comment = lv3.Attribute("Comment").Value
                                                            }
                                                          ).ToList()
                                         }

                              ).ToList()
                          }).ToList();

            foreach(var r in xmlDataList)
            {
                foreach(var e in r.Entries)
                {
                    foreach(var c in e.ContentList)
                    {
                        QueryData data = new Model.QueryData();
                        data.CityName = r.CityName;
                        data.DocId = r.DocId;
                        data.MeetingTitle = r.MeetingTitle;
                        data.MeetingLocation = r.MeetingLocation;
                        data.MeetingDate = r.MeetingDate;
                        data.MeetingDateDisplay = r.MeetingDateDisplay;
                        data.ScrapeDate = r.SearchDate;

                        data.KeyWord = e.KeyWord;
                        data.PageNumber = e.PageNumber;

                        data.Comment = c.Comment;
                        data.Content = c.Content;
                        data.QueryGuid = c.QueryGuid;

                        data.QueryFilePath = filePath;
                        resultList.Add(data);
                    }
                }
                        
            }
            return resultList;
        }


        public void UpdateQuery(DocQueryResultModel message)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(message.QueryFilePath);
            XmlNode myNode = doc.SelectNodes("Queries/Query/Entries/Entry/Contents/Content").OfType<XmlNode>().FirstOrDefault(n => n.Attributes["GUID"].InnerText == message.QueryGuid);
            if (myNode != null)
            {
                myNode.Attributes["Comment"].InnerText = message.Comment;
                doc.Save(message.QueryFilePath);
            }
        }
        public void UpdateDocStauts(DocQueryResultModel message)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(message.DocFilePath);
            XmlNode myNode = doc.SelectNodes("Docs/Doc").OfType<XmlNode>().FirstOrDefault(n => n["DocId"].InnerText == message.DocId);
            if (myNode != null)
            {
                myNode["Checked"].InnerText = "True";
                doc.Save(message.DocFilePath);
            }
         
        }
    }
}
