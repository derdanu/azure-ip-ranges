using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.Json;
using System.Net;

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
                return "data/";
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

        public void updateCloud(Cloud cloud) {

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(cloud.Url);

            string jsonUri = doc.DocumentNode.SelectSingleNode("//div[@class = 'link-align']//a").Attributes["href"].Value;

            System.IO.Directory.CreateDirectory(cloud.Filepath);

            WebClient myWebClient = new WebClient();
            myWebClient.DownloadFile(jsonUri, cloud.FileLocation);		
            
        
        }

    }

    public class Public : Cloud
    { 
        public override string Url {
            get {
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=56519";
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
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=57063";
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
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=57062";
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
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=57064";
            }
        }
        public override string Filename {
            get {
                return "AzureGermany.json";
            }
        }
        
    }

}
