using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SearchPage.Models;

namespace SearchPage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            _logger.LogInformation($"Search of {query} started");
            
            HttpClient client = new HttpClient();
            var data = await client.GetStringAsync($"http://localhost:50494/api/v1/search?request={query}");

            var urlsDict = JsonConvert.DeserializeObject<IDictionary<string, double>>(data);

            var resultView = View(urlsDict.OrderByDescending(pair => pair.Value).ToList());
            resultView.ViewData.TryAdd("Query", query);

            return resultView;
        }
        
        public IActionResult Index()
        {
            _logger.LogInformation($"Index page loaded");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            _logger.LogInformation("About page loaded");

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            _logger.LogInformation("Contact page loaded");

            return View();
        }

        public IActionResult Error()
        {
            _logger.LogError("Some Error occured");
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}