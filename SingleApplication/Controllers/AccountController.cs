﻿using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
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
        public AccountController()
        {
            userRepository = DependencyResolver.Current.GetService<IUserRepository>();

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
                ModelState.AddModelError("", "You are successfully register, please waiting our Administrator to grant your access");
            }
            return View(model);
        }
    }
}