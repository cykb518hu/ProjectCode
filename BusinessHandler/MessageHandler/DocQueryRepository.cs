
using BusinessHandler.Model;
using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BusinessHandler.MessageHandler
{
    public interface IDocQueryRepository
    {
        List<DocQueryResultModel> GetDocQueryResult(DocQueryMessage message);

        void UpdateQuery(DocQueryResultModel message);

        void UpdateDocStatus(DocQueryResultModel message);

        void UpdateDocImportant(DocQueryResultModel message);

    }
    public class DocQueryCSVRepository : IDocQueryRepository
    {
        ICacheRepository cacheRepository;
        IDataFileHelper readDataHelper;
        public DocQueryCSVRepository()
        {
            cacheRepository =  DependencyResolver.Current.GetService<ICacheRepository>();
            readDataHelper = DependencyResolver.Current.GetService<IDataFileHelper>();
        }

        public List<DocQueryResultModel> GetDocQueryResult(DocQueryMessage message)
        {

            var resultList = GetDocQueryResult();

            if (!string.IsNullOrEmpty(message.CityName))
            {
                resultList = resultList.Where(x => message.CityName.Contains(x.CityName)).ToList();
            }
            if (!string.IsNullOrEmpty(message.KeyWord))
            {
                resultList = resultList.Where(x => message.KeyWord.Contains(x.KeyWord)).ToList();
                resultList.ForEach(x => { x.Content = Regex.Replace(x.Content, x.KeyWord, string.Format("<b style='color:red'>{0}</b>", x.KeyWord), RegexOptions.IgnoreCase); });
            }
            if (!string.IsNullOrEmpty(message.MeetingDate))
            {
                var dt = DateTime.Now;
                if (DateTime.TryParse(message.MeetingDate, out dt))
                {
                    resultList = resultList.Where(x => x.MeetingDate >= dt).ToList();
                }
            }
            if (!string.IsNullOrEmpty(message.sortName))
            {
                if (message.sortOrder.Equals("asc"))
                {
                    switch(message.sortName)
                    {
                        case "CityNameDispaly":
                            resultList = resultList.OrderBy(x => x.CityName).ToList();
                            break;
                        case "MeetingDateDisplay":
                            resultList = resultList.OrderBy(x => x.MeetingDate).ToList();
                            break;
                        case "DocUrl":
                            resultList = resultList.OrderBy(x => x.DocUrl).ToList();
                            break;
                        case "PageNumber":
                            resultList = resultList.OrderBy(x => x.PageNumber).ToList();
                            break;
                        case "DocType":
                            resultList = resultList.OrderBy(x => x.DocType).ToList();
                            break;
                        case "KeyWord":
                            resultList = resultList.OrderBy(x => x.KeyWord).ToList();
                            break;
                        case "Important":
                            resultList = resultList.OrderBy(x => x.Important).ToList();
                            break;

                    }
                }
                else
                {
                    switch (message.sortName)
                    {
                        case "CityNameDispaly":
                            resultList = resultList.OrderByDescending(x => x.CityName).ToList();
                            break;
                        case "MeetingDateDisplay":
                            resultList = resultList.OrderByDescending(x => x.MeetingDate).ToList();
                            break;
                        case "DocUrl":
                            resultList = resultList.OrderByDescending(x => x.DocUrl).ToList();
                            break;
                        case "PageNumber":
                            resultList = resultList.OrderByDescending(x => x.PageNumber).ToList();
                            break;
                        case "DocType":
                            resultList = resultList.OrderByDescending(x => x.DocType).ToList();
                            break;
                        case "KeyWord":
                            resultList = resultList.OrderByDescending(x => x.KeyWord).ToList();
                            break;
                        case "Important":
                            resultList = resultList.OrderByDescending(x => x.Important).ToList();
                            break;

                    }
                }
            }
            return resultList;
        }

        public List<DocQueryResultModel> GetDocQueryResult()
        {
            var resultList = new List<DocQueryResultModel>();
            if (cacheRepository.Exists(GlobalKeyString.docQueryCacheKey))
            {
                resultList = cacheRepository.Get(GlobalKeyString.docQueryCacheKey);
            }
            else
            {
                resultList = GetDocQueryList();
                cacheRepository.Add(GlobalKeyString.docQueryCacheKey, resultList);
            }
            return resultList;
        }
 

        public List<DocQueryResultModel> GetDocQueryList()
        {
            var filePath = ConfigurationManager.AppSettings.Get("DocQueryFilePath").ToString();
            var docUrlList = Directory.GetFiles(filePath, "*Docs*");
            var queriesUrlList = Directory.GetFiles(filePath, "*Queries*");

            var resultList = new List<DocQueryResultModel>();

            var queriesList = new List<QueryData>();
            for (int i = 0; i < queriesUrlList.Length; i++)
            {
                queriesList.AddRange(readDataHelper.OpenQuery(queriesUrlList[i]));
            }

            var docList = new List<DocData>();
           
            for (int i = 0; i < docUrlList.Length; i++)
            {
                docList.AddRange(readDataHelper.OpenDoc(docUrlList[i]));
            }
          

            foreach (var r in docList)
            {
                var subQueriesList = queriesList.Where(x => x.DocId.Equals(r.DocId)).ToList();
                if (subQueriesList.Count > 0)
                {
                    foreach (var s in subQueriesList)
                    {
                        var result = new DocQueryResultModel();
                        result.CityName = r.CityName;
                        result.CityNameDispaly = "<span class='showDatePicker' onclick='showDatePicker(this); return false' style='cursor: pointer'>" + r.CityName + "</span>";
                        // result.CityNameDispaly = "<span class='showDatePicker' style='cursor: pointer'>" + r.CityName + "</span>";
                        result.IsViewed = "<span class='sp_" + r.DocId + "'>" + (r.IsViewed.Equals("True") ? "Yes" : "No") + "</span>";
                        result.DocId = r.DocId;
                        result.DocUrl = @"<a href='" + r.DocUrl + "' target='_blank'>" +  r.DocUrl.Substring(r.DocUrl.LastIndexOf('/') + 1) + " </a>";
                        result.DocType = r.DocType;
                        result.MeetingTitle = s.MeetingTitle;
                        result.MeetingDate = s.MeetingDate;
                        result.MeetingDateDisplay = s.MeetingDateDisplay;
                        result.MeetingLocation = s.MeetingLocation;
                        result.ScrapeDate = s.ScrapeDate;
                        result.Content = s.Content;
                        result.KeyWord = s.KeyWord;
                        result.DocFilePath = r.DocFilePath;
                        result.QueryFilePath = s.QueryFilePath;
                        result.QueryGuid = s.QueryGuid;
                        result.Operation = @"<button type='button' class='btn btn-default glyphicon glyphicon-edit' aria-label='Left Align' data-file='" + result.QueryFilePath + "' data-docid='" + result.DocId + "' data-queryguid='" + result.QueryGuid + "' onclick='OpenDataDetail(this); return false'></button>";
                        result.PageNumber = s.PageNumber;
                     
                        result.Important = r.Important;
                        result.Comment = "<span id=" + result.QueryGuid + ">" + s.Comment + "</span>";
                        resultList.Add(result);
                    }
                }
            }
            return resultList;
        }

        public void UpdateQuery(DocQueryResultModel message)
        {
            readDataHelper.UpdateQuery(message);
        }
        public void UpdateDocStatus(DocQueryResultModel message)
        {
            readDataHelper.UpdateDocStauts(message);
        }
        public void UpdateDocImportant(DocQueryResultModel message)
        {
            readDataHelper.UpdateDocImportant(message);
        }
    }

}
