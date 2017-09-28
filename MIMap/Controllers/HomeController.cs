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
        ISearchQueryRepository searchQueryRepository;
        IMapDataRepository mapRepository;
        public HomeController()
        {
            searchQueryRepository = DependencyResolver.Current.GetService<ISearchQueryRepository>();
            mapRepository = DependencyResolver.Current.GetService<IMapDataRepository>();

        }
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
            var user = (UserAccount)Session["UserAccount"];
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var keyWordList = DocQueryDB.GetKeyWordList();
            ViewData["municipalityList"] = mapRepository.GetFilterData();
            ViewData["keyWordList"] = keyWordList;
            var message = new DocQueryMessage();
            var mapInitialData = mapRepository.GetMapAreaData(message);

            ViewData["mapInitialData"] = mapInitialData;
            return View();
        }

        public JsonResult GetParentDataList(DocQueryMessage message)
        {
            var result = new List<MapMeeting>();
            int total = 0;
            result = mapRepository.GetMainDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMapMasterData(DocQueryMessage message)
        {
            var result = mapRepository.GetMapAreaData(message);
            return Json(result, JsonRequestBehavior.AllowGet);

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
            var data = searchQueryRepository.GetSearchQuery();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSearchQuery(string query, string title)
        {
            var data = JsonConvert.DeserializeObject<DocQueryMessage>(query);
            if (string.IsNullOrEmpty(title))
            {
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
                searchQueryRepository.UpdateSearchQuery(guid, title, query);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSearchQuery(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                searchQueryRepository.DeleteSearchQuery(guid);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSearchQueryAmount(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                searchQueryRepository.AddSearchQueryAmount(guid);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateMapColor(string cityGuid, string color)
        {
            mapRepository.UpdateMapColor(cityGuid, color);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityOrdinance(string guid)
        {
            var list = mapRepository.GetCityOrdinanceList(null, guid);
            var data = new CityOrdinance();
            if (list.Any())
            {
                data = list.FirstOrDefault();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveCityOrdinance(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var data = JsonConvert.DeserializeObject<CityOrdinance>(str);
                var user = (UserAccount)Session["UserAccount"];
                if (user != null)
                {
                    data.ModifyUser = user.Email;
                }
                if (mapRepository.UpdateCityOrdinance(data))
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }

            }
            return Json("Error in server", JsonRequestBehavior.AllowGet);
        } 
    }
}