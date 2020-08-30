using System;
using HtmlAgilityPack;
using System.Text.Json;
using System.Net;

namespace dotnet.Models
{
    public class UrlModel
    {
 
        public void updateClouds() {

            updateCloud(new Public());
            updateCloud(new AzureGovernment());
            updateCloud(new China());
            updateCloud(new AzureGermany());

        }

        public void updateCloud(Cloud cloud) {

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(cloud.Url);

            string jsonUri = doc.DocumentNode.SelectSingleNode("//div[@class = 'link-align']//a").Attributes["href"].Value;

            WebClient myWebClient = new WebClient();
            myWebClient.DownloadFile(jsonUri, cloud.Filename);		
            
        
        }

    }
}
