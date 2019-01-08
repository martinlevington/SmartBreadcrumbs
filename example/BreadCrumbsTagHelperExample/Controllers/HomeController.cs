using System.Diagnostics;
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

            var viewModel = new ExampleViewModel();
            viewModel.Title = "Nice new Home Title";

            return View(viewModel);
        }

        // this is the name of the property/method in the view model to get the value from - if not found the this is used as the title
        [Breadcrumb("Title", FromAction = "Home.Index")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var viewModel = new ExampleViewModel();
            viewModel.Title = "Nice new About Custom Title";

            return View(viewModel);
        }

        [Breadcrumb("GetTitle", FromAction = "Home.Index")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            var viewModel = new ExampleViewModel();
            viewModel.Title = "Contact Title";

            return View(viewModel);
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
