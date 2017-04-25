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
            if(user==null)
            {
                return RedirectToAction("Login", "Account");
            }

            var message = new DocQueryMessage();
            var docQuery = new DocQueryFactory();
            var docList = docQuery.GetDocQueryResult(message);
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
            if(string.IsNullOrEmpty(message.CityName))
            {
                message.CityName = GetCitys();
            }
            var result = docQuery.GetDocQueryParentResult(message);
            var total = result.Count;
            var rows = result.Skip(message.offset).Take(message.limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
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
            var docQuery = new DocQueryFactory();
            docQuery.UpdateQuery(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocStatus(DocQueryResultModel message)
        {
            var docQuery = new DocQueryFactory();
            docQuery.UpdateDocStatus(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDateBasedOnCity(DocQueryMessage message)
        {
            var docQuery = new DocQueryFactory();
            var result = docQuery.GetDocQueryResult(message);
            var dateList = result.Select(x => x.MeetingDateDisplay).Distinct().ToList();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}