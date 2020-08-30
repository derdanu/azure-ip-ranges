using System;
using HtmlAgilityPack;
using System.Text.Json;
using System.Net;
using System.Collections.Generic;

namespace dotnet.Models
{
    public class UrlModel
    {
 
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

            WebClient myWebClient = new WebClient();
            myWebClient.DownloadFile(jsonUri, cloud.Filename);		
            
        
        }

    }
}
