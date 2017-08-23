using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIMap.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Map()
        {
            var municipalityList = DocQueryDB.GetMapMunicipality();
            var keyWordList = DocQueryDB.GetKeyWordList();
            ViewData["municipalityList"] = municipalityList;
            ViewData["keyWordList"] = keyWordList;
            return View();
        }

        public JsonResult GetParentDataList(DocQueryMessage message)
        {
            var result = new List<DocQueryParentModel>();
            int total = 0;
            result = DocQueryDB.GetAllDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UpdateDocStatus(DocQueryResultModel message)
        {
            DocQueryDB.UpdateDocStatus(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocImportant(DocQueryResultModel message)
        {
            DocQueryDB.UpdateDocImportant(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveComment(DocQueryResultModel message)
        {
            DocQueryDB.UpdateQueryComment(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSearchQuery()
        {
            var searchQueryRepository = new SearchQueryRepository(StaticSetting.queryFile);
            var data = searchQueryRepository.GetSearchQuery();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSearchQuery(string query)
        {
            var searchQueryRepository = new SearchQueryRepository(StaticSetting.queryFile);
            var data = JsonConvert.DeserializeObject<DocQueryMessage>(query);
            var title = string.Empty;
            if (!string.IsNullOrWhiteSpace(data.CityName))
            {
                title = data.CityName;
            }
            if (!string.IsNullOrWhiteSpace(data.CountyName))
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title += "&";
                }
                title += data.CountyName;
            }
            if (!string.IsNullOrWhiteSpace(data.KeyWord))
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title += "&";
                }
                title += data.KeyWord;
            }
            if (!string.IsNullOrWhiteSpace(data.DeployDate))
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title += "&";
                }
                title += data.DeployDate;
            }
            if (!string.IsNullOrWhiteSpace(data.MeetingDate))
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title += "&";
                }
                title += data.MeetingDate;
            }
            if (!string.IsNullOrEmpty(title))
            {
                searchQueryRepository.AddSearchQuery(query, title);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSearchQuery(string guid, string title, string query)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(guid))
            {
                var searchQueryRepository = new SearchQueryRepository(StaticSetting.queryFile);
                searchQueryRepository.UpdateSearchQuery(guid, title, query);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSearchQuery(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                var searchQueryRepository = new SearchQueryRepository(StaticSetting.queryFile);
                searchQueryRepository.DeleteSearchQuery(guid);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSearchQueryAmount(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                var searchQueryRepository = new SearchQueryRepository(StaticSetting.queryFile);
                searchQueryRepository.AddSearchQueryAmount(guid);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}