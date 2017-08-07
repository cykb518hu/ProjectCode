using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SingleApplication.Controllers
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

        public ActionResult DataDetail()
        {
            var user = (UserAccount)Session["UserAccount"];
            if (user == null)
            {
                  return RedirectToAction("Login", "Account");
            }
            var message = new DocQueryMessage();
            message.CityName = GetCitys();
            var docQuery = new DocQueryFactory();
            var docList = new List<DocQueryResultModel>();
            if (StaticSetting.GetDataFromDB())
            {
                docList = DocQueryDB.GetCityDate();
            }
            else
            {
                docList= docQuery.GetDocQueryResult(message);
            }
            var keyWordRepository = DependencyResolver.Current.GetService<IKeyWord>();
            ViewData["KeyWordList"] = keyWordRepository.GetKeyWordList();

            if (StaticSetting.GetDataFromDB())
            {
                Dictionary<string, string> cityDployeList = new Dictionary<string, string>();
    
                foreach (var r in docList)
                {
                    if (!cityDployeList.ContainsKey(r.CityName)&&!string.IsNullOrWhiteSpace(r.CityDeployDate))
                    {
                        cityDployeList.Add(r.CityName, r.CityDeployDate);
                    }
                }
                ViewData["cityDeployDateList"] = cityDployeList;
            }
            else
            {
                ViewData["cityDeployDateList"] = docQuery.GetCityScrapeDateList();
            }
           



            return View(docList);

        }

        public JsonResult GetDataList(DocQueryMessage message)
        {
            var docQuery = new DocQueryFactory();
            var result = docQuery.GetDocQueryResult(message);
            var total = result.Count;
            var rows = result.Skip(message.offset).Take(message.limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetParentDataList(DocQueryMessage message)
        {
            var docQuery = new DocQueryFactory();
            if (string.IsNullOrEmpty(message.CityName) || message.CityName.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                message.CityName = GetCitys();
            }
            var result = new List<DocQueryParentModel>();
            if (StaticSetting.GetDataFromDB())
            {
                int total = 0;
                result = DocQueryDB.GetAllDataList(message, out total);
                return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result = docQuery.GetDocQueryParentResult(message);
                var total = result.Count;
                var rows = result.Skip(message.offset).Take(message.limit).ToList();
                return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
            }
          
        }

        public string GetCitys()
        {
            string citys = string.Empty;
            var user = (UserAccount)Session["UserAccount"];
            if (user != null&& user.RoleType == GlobalKeyString.roleTypeGeneral)
            {
                citys = user.Cityes;
            }
            return citys;
        }
        public JsonResult SaveComment(DocQueryResultModel message)
        {
       
            if (StaticSetting.GetDataFromDB())
            {
                DocQueryDB.UpdateQueryComment(message);
            }
            else
            {
                var docQuery = new DocQueryFactory();
                docQuery.UpdateQuery(message);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocStatus(DocQueryResultModel message)
        {
            if (StaticSetting.GetDataFromDB())
            {
                DocQueryDB.UpdateDocStatus(message);
            }
            else
            {
                var docQuery = new DocQueryFactory();
                docQuery.UpdateDocStatus(message);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocImportant(DocQueryResultModel message)
        {
            if (StaticSetting.GetDataFromDB())
            {
                DocQueryDB.UpdateDocImportant(message);
            }
            else
            {
                var docQuery = new DocQueryFactory();
                docQuery.UpdateDocImportant(message);
            }
      
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDateBasedOnCity(DocQueryMessage message)
        {
            var docQuery = new DocQueryFactory();
            var result = docQuery.GetDocQueryResult(message);


            var dateList = result.Select(x => x.MeetingDateDisplay).Distinct().ToList();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNewKeyWord(string keyWord)
        {
            string result = "Success";
            if(!string.IsNullOrWhiteSpace(keyWord))
            {
                var keyWordRepository = DependencyResolver.Current.GetService<IKeyWord>();
                keyWordRepository.AddKeyWord(keyWord, out result);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}