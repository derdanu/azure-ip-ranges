using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnet.Models;
using HtmlAgilityPack;

namespace dotnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private JsonSerializerOptions options; 

        private string remoteUri = "https://www.microsoft.com/en-us/download/confirmation.aspx?id=56519";
        private string jsonUri;
        private string fileName = "data.json";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            options =  new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public IActionResult Index()
        {

            var jsonModel = getServiceTagsModel();

            ViewData["changeNumber"] = jsonModel.changeNumber;
            ViewData["cloud"] = jsonModel.cloud;

            ViewBag.serviceTags = jsonModel.values;

            return View();
        
        }

        public string getPrefixes(string id)
        {
            
            var jsonModel = getServiceTagsModel();

            String addressPrefixes = ""; 

            foreach (ValuesModel values in jsonModel.values) {
                if (values.id.Equals(id)) {
                    addressPrefixes = JsonSerializer.Serialize(values.properties.addressPrefixes);
                }
            }
                        
            return addressPrefixes;

        }

        public IActionResult Update()
        {

            try {

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(remoteUri);

                jsonUri = doc.DocumentNode.SelectSingleNode("//div[@class = 'link-align']//a").Attributes["href"].Value;

                WebClient myWebClient = new WebClient();
                myWebClient.DownloadFile(jsonUri,fileName);		
            
            } catch (Exception e) {
                ViewData["error"]  = e.ToString();
            }

            return View();

        }

        private ServiceTagsModel getServiceTagsModel() {

            if (!System.IO.File.Exists("data.json")) {
                this.Update();
            }

            var jsonString = System.IO.File.ReadAllText("data.json");
            return JsonSerializer.Deserialize<ServiceTagsModel>(jsonString, options);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
