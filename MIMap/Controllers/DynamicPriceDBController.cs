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
            ViewData["storeList"] = dynamicPriceRepository.GetStoreList();
            var message = new DynamicPricingMapStoreQueryModel();
            ViewData["storeColorList"] = dynamicPriceRepository.GetStoreIdWithColorList(message);

            ViewData["categoryList"] = dynamicPriceRepository.GetCategoryList();

            return View();
        }

        public JsonResult GetTableDataList(DynamicPricingQueryModel message)
        {
            var result = new List<DynamicPricingTableModel>();
            int total = 0;
            result = dynamicPriceRepository.GetDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreDetailList(DynamicPricingMapStoreQueryModel message)
        {
            var result = dynamicPriceRepository.GetStoreDetailList(message);
            var total = result.Count;
            result = result.Skip(message.offset).Take(message.limit).ToList();
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStoreIdWithColorList(DynamicPricingMapStoreQueryModel message)
        {
            var result = dynamicPriceRepository.GetStoreIdWithColorList(message);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetStoreDetail(string storeId)
        {
            var result = dynamicPriceRepository.GetStoreDetail(storeId);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

    }
}