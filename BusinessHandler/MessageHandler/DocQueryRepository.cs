
using BusinessHandler.Model;
using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BusinessHandler.MessageHandler
{
    public interface IDocQueryRepository
    {
        List<DocQueryResultModel> GetDocQueryResult(DocQueryMessage message);
        List<DocQueryResultModel> GetDocQueryResult();

        void UpdateQuery(DocQueryResultModel message);
       
    }
    public class DocQueryCSVRepository : IDocQueryRepository
    {
        ICacheRepository cacheRepository;
        public const string docQueryCacheKey = "docQueryCacheKey";
        public DocQueryCSVRepository()
        {
           // cacheRepository = new AspNetCacheRepository();// DependencyResolver.Current.GetService<ICacheRepository>();
            cacheRepository =  DependencyResolver.Current.GetService<ICacheRepository>();
        }
        public List<DocQueryResultModel> GetDocQueryResult()
        {
            var resultList = new List<DocQueryResultModel>();
            if (cacheRepository.Exists(docQueryCacheKey))
            {
                resultList = cacheRepository.Get(docQueryCacheKey);
            }
            else
            {
                resultList = GetDocQueryList();
                cacheRepository.Add(docQueryCacheKey, resultList);
            }
            return resultList;
        }
        public List<DocQueryResultModel> GetDocQueryResult(DocQueryMessage message)
        {

            var resultList = GetDocQueryResult();
            
            if (!string.IsNullOrEmpty(message.CityName))
            {
                resultList = resultList.Where(x => x.CityName.Equals(message.CityName)).ToList();
            }
            if (!string.IsNullOrEmpty(message.KeyWord))
            {
                resultList = resultList.Where(x => x.KeyWord.Contains(message.KeyWord)).ToList();
            }
            if (!string.IsNullOrEmpty(message.MeetingDate))
            {
                var dt = DateTime.Now;
                if (DateTime.TryParse(message.MeetingDate, out dt))
                {
                    resultList = resultList.Where(x => x.MeetingDate >= dt).ToList();
                }
            }
            if(!string.IsNullOrEmpty(message.sortName))
            {
                if(message.sortOrder.Equals("asc"))
                {
                    resultList = resultList.OrderBy(x => x.MeetingDate).ToList();
                }
                else
                {
                    resultList = resultList.OrderByDescending(x => x.MeetingDate).ToList();
                }
            }
            return resultList;
        }

        public List<DocQueryResultModel> GetDocQueryList()
        {
            var filePath = ConfigurationManager.AppSettings.Get("DocQueryFilePath").ToString();
            var docUrlList = Directory.GetFiles(filePath, "*Docs.csv");
            var queriesUrlList = Directory.GetFiles(filePath, "*Queries.csv");

            var resultList = new List<DocQueryResultModel>();

            var docList = new List<DocData>();
           
            for (int i = 0; i < docUrlList.Length; i++)
            {
                docList.AddRange(CSVFileHelper.OpenDocCSV(docUrlList[i]));
            }
            var queriesList = new List<QueryData>();
            for (int i = 0; i < docUrlList.Length; i++)
            {
                queriesList.AddRange(CSVFileHelper.OpenQueryCSV(queriesUrlList[i]));
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
                        result.DocId = r.DocId;
                        result.DocUrl = @"<a href='" + r.DocUrl + "' target='_blank'>" +  r.DocUrl.Substring(r.DocUrl.LastIndexOf('/') + 1) + " </a>";
                        result.DocType = r.DocType;
                        result.MeetingTitle = s.MeetingTitle;
                        result.MeetingDate = s.MeetingDate;
                        result.MeetingDateDisplay = s.MeetingDateDisplay;
                        result.MeetingLocation = s.MeetingLocation;
                        result.Content = s.Content;
                        result.KeyWord = s.KeyWord;
                        result.DocFilePath = r.DocFilePath;
                        result.QueryFilePath = s.QueryFilePath;
                        result.QueryGuid = s.QueryGuid;
                        result.Operation = @"<button type='button' class='btn btn-default glyphicon glyphicon-edit' aria-label='Left Align' data-file='" + result.QueryFilePath + "' data-docid='" + result.DocId + "' data-queryguid='" + result.QueryGuid + "' onclick='OpenDataDetail(this); return false'></button>";


                        result.Comment = "<span id=" + result.QueryGuid + ">" + s.Comment + "</span>";
                        resultList.Add(result);
                    }
                }
                //else
                //{
                //    var result = new DocQueryResultModel();
                //    result.CityName = r.CityName;
                //    result.DocUrl = @"<a href='" + r.DocUrl + "' target='_blank'>" + r.DocUrl + " </a>";
                //    result.DocId = r.DocId;
                //    result.DocType = r.DocType;
                //    result.MeetingTitle = string.Empty;
                //    result.MeetingDate = DateTime.MinValue;
                //    result.MeetingDateDisplay = string.Empty;
                //    result.MeetingLocation = string.Empty;
                //    result.Content = string.Empty;
                //    result.Operation = string.Empty;
                //    result.KeyWord = string.Empty;
                //    result.DocFilePath = r.DocFilePath;
                //    result.QueryFilePath = string.Empty;
                //    resultList.Add(result);
                //}
            }
            return resultList;
        }

        public void UpdateQuery(DocQueryResultModel message)
        {
            CSVFileHelper.UpdateQueryCSV(message);
        }
    }

}
