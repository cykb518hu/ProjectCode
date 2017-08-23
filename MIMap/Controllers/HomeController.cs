using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
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
    }
}