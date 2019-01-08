﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BreadCrumbsTagHelperExample.Models;
using SmartBreadcrumbs;

namespace BreadCrumbsTagHelperExample.Controllers
{
    public class HomeController : Controller
    {

        [DefaultBreadcrumb("My home")]
        public IActionResult Index()
        {
            return View();
        }

        [Breadcrumb("ViewData.Title", FromAction = "Home.Index")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
