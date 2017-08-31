﻿using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using MIMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIMap.Controllers
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
                return RedirectToAction("Map", "Home");
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
            user.Active = "Yes";
            string result = userRepository.Register(user);
            if (result != "sccuess")
            {
                ModelState.AddModelError("", result);
            }
            else
            {
                ModelState.AddModelError("", "You are successfully registered, please reach out to administrator to grant your access");
                ModelState.AddModelError("", "You are successfully registered");
            }
            return View(model);
        }

    }
}