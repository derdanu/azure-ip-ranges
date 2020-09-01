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

        private Cloud cloud = new Public();
        private List<Cloud> clouds = new List<Cloud>();
        public const string SessionKeyName = "_CloudEnv";

        private JsonSerializerOptions options;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            clouds.Add(new Public());
            clouds.Add(new AzureGovernment());
            clouds.Add(new China());
            clouds.Add(new AzureGermany());
        }


        public IActionResult Index()
        {

            var jsonModel = getServiceTagsModel();

            ViewData["changeNumber"] = jsonModel.changeNumber;
            ViewData["cloud"] = jsonModel.cloud;

            ViewBag.serviceTags = jsonModel.values;

            return View();

        }

        private ARMModel getARMTemplate(string id, string env) 
        {
            
            var jsonModel = getServiceTagsModel(env);

            ARMModel arm = new ARMModel();                        
            arm.schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";
            arm.contentVersion = "1.0.0.0";
            arm.filename = "template.json";
            
            Properties prop = new Properties();
            prop.routes = new List<Route>();
   
            foreach (ValuesModel values in jsonModel.values)
            {
                if (values.id.Equals(id))
                {
                    var i = 1;
                    foreach (String prefix in values.properties.addressPrefixes) {
                        
                        Route route = new Route();
                        route.name = $"{jsonModel.cloud}.{values.id}-{i:000}"; 

                        RouteProperties routeprop = new RouteProperties();
                        routeprop.addressPrefix = prefix;
                        routeprop.nextHopType = "Internet";
                        
                        route.properties = routeprop;
                        
                        prop.routes.Add(route);
                        
                        i++;
                    }
                                
                    arm.filename = jsonModel.cloud + "." + values.id + ".arm.json";
                }
            }
           

            Resources res = new Resources();
            res.type = "Microsoft.Network/routeTables";
            res.name = "Routetable";
            res.apiVersion = "2015-06-15";
            res.location = "[resourceGroup().location]";
            res.properties = prop;

            arm.resources = new List<Resources>();
            arm.resources.Add(res);
            
            return arm;
        }


        [HttpGet("/downloadARMTemplate/{env}/{id}")]
        public FileContentResult downloadARMTemplate(string id, string env)
        {

            ARMModel arm = getARMTemplate(id, env);

            var armtemplate = JsonSerializer.SerializeToUtf8Bytes(arm, options);
                        
            return File(armtemplate, "application/json", arm.filename);


        }

        [HttpGet("/deployARMTemplate/{env}/{id}")]
        public String deployARMTemplate(string id, string env)
        {

            ARMModel arm = getARMTemplate(id, env);

            return JsonSerializer.Serialize(arm, options);
                        
        }        

        public IActionResult getPrefixes(string id)
        {

            var jsonModel = getServiceTagsModel();

            byte[] addressPrefixes = {};

            String filename = "prefixes.json";

            foreach (ValuesModel values in jsonModel.values)
            {
                if (values.id.Equals(id))
                {
                    addressPrefixes = JsonSerializer.SerializeToUtf8Bytes(values.properties, options);
                    filename = jsonModel.cloud + "." + values.id + ".json";
                }
            }

           return File(addressPrefixes, "application/json", filename);

        }
        [HttpGet("/getOPNsenseUrlTable/{env}/{id}")]
        public string getOPNsenseUrlTable(string id, string env)
        {

            var jsonModel = getServiceTagsModel(env);

            var opnsenseout = new System.Text.StringBuilder();
            opnsenseout.AppendLine("; Azure IP Ranges - (c) 2020 dafalkne");
            opnsenseout.AppendLine("; " + Request.GetDisplayUrl());
            opnsenseout.AppendLine("; Cloud: " + jsonModel.cloud + " - Changenumber: " + jsonModel.changeNumber);

            foreach (ValuesModel values in jsonModel.values)
            {
                if (values.id.Equals(id))
                {
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

            try
            {

                cloud.updateClouds(clouds);

            }
            catch (Exception e)
            {
                ViewData["error"] = e.ToString();
            }

            return View();

        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult changeCloud(string env)
        {

            Response.Cookies.Append(SessionKeyName, env);

            return RedirectToAction("Index");

        }

        private ServiceTagsModel getServiceTagsModel(string env)
        {

            cloud = clouds.Find(x => x.CloudName.Contains(env));
            if (cloud == null)
            {
                cloud = clouds[0];
            }

            if (!System.IO.File.Exists(cloud.FileLocation))
            {
                cloud.updateCloud(cloud);
            }

            ViewData["env"] = cloud.CloudName;

            var jsonString = System.IO.File.ReadAllText(cloud.FileLocation);
            return JsonSerializer.Deserialize<ServiceTagsModel>(jsonString, options);

        }
        private ServiceTagsModel getServiceTagsModel()
        {

            string env;

            if (String.IsNullOrEmpty(Request.Cookies[SessionKeyName]))
            {
                env = clouds[0].CloudName;
            }
            else
            {
                env = Request.Cookies[SessionKeyName];
            }
            return getServiceTagsModel(env);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
