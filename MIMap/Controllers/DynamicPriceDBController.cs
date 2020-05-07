using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIMap.Controllers
{
    public class DynamicPriceDBController : Controller
    {
        IDynamicPriceRepository dynamicPriceRepository;
        
        public DynamicPriceDBController()
        {
            dynamicPriceRepository= DependencyResolver.Current.GetService<IDynamicPriceRepository>();
        }

        // GET: DynamicPriceDB
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTableDataList(DynamicPricingQueryModel message)
        {
            var result = new List<DynamicPricingTableModel>();
            int total = 0;
            result = dynamicPriceRepository.GetDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }
    }
}