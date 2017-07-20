
using BusinessHandler.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusinessHandler.MessageHandler
{
   public class DocQueryFactory
    {
        IDocQueryRepository docQueryRepository;
        ICacheRepository cacheRepository;
        public  DocQueryFactory()
        {

            docQueryRepository =  DependencyResolver.Current.GetService<IDocQueryRepository>();
            cacheRepository =  DependencyResolver.Current.GetService<ICacheRepository>();

        }

        public List<DocQueryParentModel> GetDocQueryParentResult(DocQueryMessage message)
        {
            var list = new List<DocQueryParentModel>();
            var tempList = GetDocQueryResult(message);

       
            foreach (var r in tempList.GroupBy(x=>x.DocId))
            {
                var data = new DocQueryParentModel();
                data.DocId = r.Key;
                var keyWordList = tempList.Where(x => x.DocId == r.Key).ToList();
                if (keyWordList != null)
                {
                    int count = 0;
                    var firstItem = keyWordList.FirstOrDefault();
                    data.DocFilePath = firstItem.DocFilePath;
                    data.DocUrl = firstItem.DocUrl;
                    data.CityScrapeDate = firstItem.CityScrapeDate;
                    data.DocType = firstItem.DocType;
                    data.IsViewed = firstItem.IsViewed;
                    data.ScrapeDate = firstItem.ScrapeDate;
                    data.CityNameDispaly = firstItem.CityNameDispaly;
                    data.MeetingDateDisplay = firstItem.MeetingDateDisplay;
                    // user asked to change the name, so if it's important eqal removed
                    data.Removed = firstItem.Important.Equals("True") ? "Yes" : "No";
                    var checkStr = data.Removed.Equals("Yes") ? "checked" : "";

                    data.ImportantDisplay = @"<input type='checkbox'  onclick='RemoveData(this);'   data-file='" + data.DocFilePath + "' data-docid='" + data.DocId + "' " + checkStr + " />";

                    keyWordList.ForEach(x => { count += Regex.Matches(x.Content, x.KeyWord, RegexOptions.IgnoreCase).Count; });
                    data.Number = count;
                    data.KeyWordString = string.Join(",", keyWordList.Select(x => x.KeyWord).Distinct().ToArray());
                    data.DocQuerySubList = (List<DocQueryResultModel>)keyWordList;
                }
                if (string.IsNullOrEmpty(message.IsViewed) || message.IsViewed.Equals("All", StringComparison.InvariantCultureIgnoreCase) || data.IsViewed.ToLower().Contains(message.IsViewed.ToLower()))
                {
                    if (string.IsNullOrEmpty(message.Important) || message.Important.Equals("All", StringComparison.InvariantCultureIgnoreCase) || data.Removed.ToLower().Contains(message.Important.ToLower()))
                    {
                        list.Add(data);
                    }
                }
            }
            if (!string.IsNullOrEmpty(message.sortName))
            {
                if (message.sortOrder.Equals("asc"))
                {
                    switch (message.sortName)
                    {
                        case "Number":
                            list = list.OrderBy(x => x.Number).ToList();
                            break;
                        case "KeyWordString":
                            list = list.OrderBy(x => x.KeyWordString).ToList();
                            break;
                        case "IsViewed":
                            list = list.OrderBy(x => x.IsViewed).ToList();
                            break;
                        case "ScrapeDate":
                            list = list.OrderBy(x => x.ScrapeDate).ToList();
                            break;
                        case "CityScrapeDate":
                            list = list.OrderBy(x => x.CityScrapeDate).ToList();
                            break;
                        case "ImportantDisplay":
                            list = list.OrderBy(x => x.Removed).ToList();
                            break;
                            
                    }
                }
                else
                {
                    switch (message.sortName)
                    {
                        case "Number":
                            list = list.OrderByDescending(x => x.Number).ToList();
                            break;
                        case "KeyWordString":
                            list = list.OrderByDescending(x => x.KeyWordString).ToList();
                            break;
                        case "IsViewed":
                            list = list.OrderByDescending(x => x.IsViewed).ToList();
                            break;
                        case "ScrapeDate":
                            list = list.OrderByDescending(x => x.ScrapeDate).ToList();
                            break;
                        case "CityScrapeDate":
                            list = list.OrderByDescending(x => x.CityScrapeDate).ToList();
                            break;
                        case "ImportantDisplay":
                            list = list.OrderByDescending(x => x.Removed).ToList();
                            break;
                    }
                }
            }
            return list;
        }
        public List<DocQueryResultModel> GetDocQueryResult(DocQueryMessage message)
        {
            var list = new List<DocQueryResultModel>();
            list = docQueryRepository.GetDocQueryResult(message);
            return list;
        }
        public void UpdateQuery(DocQueryResultModel message)
        {
            docQueryRepository.UpdateQuery(message);
            if (cacheRepository.Exists(GlobalKeyString.docQueryCacheKey))
            {
                cacheRepository.Clear(GlobalKeyString.docQueryCacheKey);
            }
        }
        public void UpdateDocStatus(DocQueryResultModel message)
        {
            docQueryRepository.UpdateDocStatus(message);
            if (cacheRepository.Exists(GlobalKeyString.docQueryCacheKey))
            {
                cacheRepository.Clear(GlobalKeyString.docQueryCacheKey);
            }
        }
        public void UpdateDocImportant(DocQueryResultModel message)
        {
            docQueryRepository.UpdateDocImportant(message);
            if (cacheRepository.Exists(GlobalKeyString.docQueryCacheKey))
            {
                cacheRepository.Clear(GlobalKeyString.docQueryCacheKey);
            }
        }

    }
}
