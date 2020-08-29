using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using dotnet.Models;
using HtmlAgilityPack;

namespace dotnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private Cloud cloud; 

        public const string SessionKeyName = "_CloudEnv";

        private JsonSerializerOptions options; 

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

        public IActionResult getPrefixes(string id)
        {
            
            var jsonModel = getServiceTagsModel();

            String addressPrefixes = ""; 
            String filename = "prefixes.json";

            foreach (ValuesModel values in jsonModel.values) {
                if (values.id.Equals(id)) {
                    addressPrefixes = JsonSerializer.Serialize(values.properties);
                    filename = jsonModel.cloud + "." + values.id + ".json";
                }
            }
            
            var file = Encoding.ASCII.GetBytes(addressPrefixes);
            return File(file,  "application/json", filename);             

        }

        public string getOPNsenseUrlTable(string id)
        {
          
            
            var jsonModel = getServiceTagsModel();

            var opnsenseout = new System.Text.StringBuilder();
            opnsenseout.AppendLine("; Azure IP Ranges - (c) 2020 dafalkne");
            opnsenseout.AppendLine("; " + Request.GetDisplayUrl());
            opnsenseout.AppendLine("; Cloud: " + jsonModel.cloud + " - Changenumber: " + jsonModel.changeNumber);

            foreach (ValuesModel values in jsonModel.values) {
                if (values.id.Equals(id)) {
                    opnsenseout.AppendLine("; Service: " + values.id + " - Changenumber: " + values.properties.changeNumber);
                    foreach (string net in values.properties.addressPrefixes)
                    {
                        opnsenseout.AppendLine(net + " ; " + values.id);
                    } 
                }
            }
                        
            return opnsenseout.ToString();

        }

        public IActionResult Update()
        {

            try {
                
                UrlModel model = new UrlModel();
                model.updateClouds();
            
            } catch (Exception e) {
                ViewData["error"]  = e.ToString();
            }

            return View();

        }

        public IActionResult changeCloud(string env) {

            Response.Cookies.Append(SessionKeyName, env);
            
            return RedirectToAction("Index");

        }


        private ServiceTagsModel getServiceTagsModel() {

            
            string env = Request.Cookies[SessionKeyName];

          
             switch (env) {
                case "PublicCloud":
                    cloud = new PublicCloud();
                    break;
                case "USGovCloud":
                    cloud = new USGovCloud();
                    break;
                case "ChinaCloud": 
                    cloud = new ChinaCloud();
                    break;
                case "GermanyCloud":
                    cloud = new GermanyCloud();
                    break; 
                default:
                    cloud = new PublicCloud();
                    break;
            }


            if (!System.IO.File.Exists(cloud.Filename)) {
                UrlModel model = new UrlModel();
                model.updateCloud(cloud);
            }

            var jsonString = System.IO.File.ReadAllText(cloud.Filename);
            return JsonSerializer.Deserialize<ServiceTagsModel>(jsonString, options);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
