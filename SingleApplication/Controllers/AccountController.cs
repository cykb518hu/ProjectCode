using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using Newtonsoft.Json;
using SingleApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SingleApplication.Controllers
{
    public class AccountController : Controller
    {
        IUserRepository userRepository;
        ISearchQueryRepository searchQueryRepository;
        public AccountController()
        {
            userRepository = DependencyResolver.Current.GetService<IUserRepository>();
            searchQueryRepository = DependencyResolver.Current.GetService<ISearchQueryRepository>();

        }
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            UserAccount user = new UserAccount();
            user.Email = model.Email;
            user.Password = model.Password;
            string result = string.Empty;
            user = userRepository.Login(user, out result);
            if (result != "sccuess")
            {
                ModelState.AddModelError("", result);
                return View(model);
            }
            else
            {
                Session["UserAccount"] = user;
                return RedirectToAction("DataDetail", "Home");
            }


        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            UserAccount user = new UserAccount();
            user.Email = model.Email;
            user.Password = model.Password;
            user.Cityes = "";
            user.Active = "No";
            string result = userRepository.Register(user);
            if (result != "sccuess")
            {
                ModelState.AddModelError("", result);
            }
            else
            {
                ModelState.AddModelError("", "You are successfully registered, please reach out to administrator to grant your access");
            }
            return View(model);
        }

        public ActionResult Manage()
        {
            var message = new DocQueryMessage();
            var docQuery = new DocQueryFactory();
            var docList = docQuery.GetDocQueryResult(message);
            return View(docList);
        }
        public JsonResult GetDataList()
        {
            var result = userRepository.GetUserList();
            var total = result.Count;
            foreach(var r in result)
            {
                r.Operation = @"<button type='button' class='btn btn-default glyphicon glyphicon-edit' aria-label='Left Align' data-cities='" + r.Cityes + "' data-active='" + r.Active + "' data-email='" + r.Email + "' onclick='OpenDataDetail(this); return false'></button>";

            }
            return Json(new { total = total, data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveUser(UserAccount message)
        {
            userRepository.ActiveUser(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session["UserAccount"] = null;
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public JsonResult SaveSearchQuery(string query)
        {
            var data = JsonConvert.DeserializeObject<DocQueryMessage>(query);
            var title = string.Empty;
            if (!string.IsNullOrWhiteSpace(data.CityName))
            {
                title = data.CityName;
            }
            if (!string.IsNullOrWhiteSpace(data.KeyWord))
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title += "&";
                }
                title += data.KeyWord;
            }
            if (!string.IsNullOrWhiteSpace(data.CityScrapeDate))
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title += "&";
                }
                title += data.CityScrapeDate;
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

        [HttpGet]
        public JsonResult GetSearchQuery()
        {
            var data  = searchQueryRepository.GetSearchQuery();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSearchQuery(string guid,string title)
        {
            if (!string.IsNullOrWhiteSpace(title) &&!string.IsNullOrWhiteSpace(guid))
            {
                searchQueryRepository.UpdateSearchQuery(guid, title);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSearchQuery(string guid)
        {
            if ( !string.IsNullOrWhiteSpace(guid))
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


    }
}