using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_4Point1.Models;

namespace MVC_4Point1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // "return View()" will call the view that is associated with the path "baseurl.com/Controller/Action".
            // In this case baseurl.com/Home/Index or Views/Home/Index.cshtml.
            return View();
        }

        public IActionResult Privacy()
        {
            // In this case we call baseurl.com/Home/Privacy or Views/Home/Privacy.cshtml.
            return View();
        }

        // When we click our "Test Page" link in the menu, it calls:
        //      asp-controller="Home" asp-action="Test"
        // This means it will call the controller called "HomeController" (not just "Home"), and the action method called "Test()".
        // The get parameter content will be stored in the content parameter when the action is called.
        public IActionResult Test(string content)
        {
            // This will output to the "Output" tab, allowing for console-like debugging outputs in an MVC application.
            Debug.WriteLine("--------------------\nDEBUGGING OUTPUT: Test() Action Called!\n--------------------");

            // If we want the View to be able to see the content, we can forward it along using the ViewBag.
            // We assign the data that is stored in our parameter to the ViewBag for use in the page.
            // By Null-Coalescing, we can ensure that the ViewBag has at least something, even if the GET parameter is not provided.
            ViewBag.GETParameterData = content ?? "No Data Provided";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
