using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InteractiveMap.Controllers
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
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CityMap()
        {
            
            return View();
        }

        public JsonResult GetDataList()
        {
            var result = MapCounty.GetMapCountyList();
            var total = result.Count;
            var rows = result.Skip(0).Take(10).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }
    }
}