using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.IO;

namespace dotnet.Models
{

    abstract public class Cloud 
    {
        public abstract string Url { get; }
        public abstract string Filename { get; }
        
        public string CloudName {
            get {
                return this.GetType().Name;
            }
        }
        public string Filepath {
            get {
                return "wwwroot/data/";
            }
        }

        public string FileLocation {
            get {
                return this.Filepath + this.Filename;
            }
        }

        public void updateClouds(List<Cloud> clouds) {

            foreach (Cloud cloud in clouds)
            {
                updateCloud(cloud);
            }

        }

        public async void updateCloud(Cloud cloud) {

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(cloud.Url);

            string jsonUri = doc.DocumentNode.SelectSingleNode("//*[@id='rootContainer_DLCDetails']/section[3]/div/div/div/div/div/a").Attributes["href"].Value;

            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(jsonUri);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsByteArrayAsync();
                string directory = Path.GetDirectoryName(cloud.FileLocation);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllBytes(cloud.FileLocation, content);
            }
        
        }

    }

    public class Public : Cloud
    { 
        public override string Url {
            get {
                return "https://www.microsoft.com/download/details.aspx?id=56519";
            }
        }
        public override string Filename {
            get {
                return "Public.json";
            }
        }

    }

    public class AzureGovernment : Cloud
    { 
        
        public override string Url {
            get {
                return "https://www.microsoft.com/download/details.aspx?id=57063";
            }
        }
        public override string Filename {
            get {
                return "AzureGovernment.json";
            }
        }
      
    }

    
    public class China : Cloud
    { 
       
        public override string Url {
            get {
                return "https://www.microsoft.com/download/details.aspx?id=57062";
            }
        }
        public override string Filename {
            get {
                return "China.json";
            }
        }
       
    }

    
    public class AzureGermany : Cloud
    { 
        
        public override string Url {
            get {
                return "https://www.microsoft.com/download/details.aspx?id=57064";
            }
        }
        public override string Filename {
            get {
                return "AzureGermany.json";
            }
        }
        
    }

}
