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
            return View();
        }

        public JsonResult GetDataList(DocQueryMessage message)
        {
            var docQuery = new DocQueryFactory();
            var result = docQuery.GetDocQueryResult(message);
            var total = result.Count;
            var rows = result.Skip(message.offset).Take(message.limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveComment(DocQueryResultModel message)
        {
            var docQuery = new DocQueryFactory();
            docQuery.UpdateQuery(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadCityList()
        {
            var docQuery = new DocQueryFactory();
            var result = docQuery.GetCityList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}