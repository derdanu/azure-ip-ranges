using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace dotnet.Models
{

    abstract public class Cloud 
    {
        public abstract string Url { get; }
        public abstract string Filename { get; }
        
        public string CloudName => GetType().Name;
        public string Filepath => "wwwroot/data/";
        public string FileLocation => Path.Combine(Filepath, Filename);

        public async Task UpdateCloudsAsync(List<Cloud> clouds) {

            foreach (Cloud cloud in clouds)
            {
                await UpdateCloudAsync(cloud);
            }

        }

        public async Task UpdateCloudAsync(Cloud cloud) {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(cloud.Url);

                var linkNode = doc.DocumentNode
                    .SelectSingleNode("//*[@id='rootContainer_DLCDetails']/section[3]/div/div/div/div/div/a");
                
                if (linkNode?.Attributes["href"]?.Value == null)
                    throw new InvalidOperationException($"Download link not found for {cloud.CloudName}");

                string jsonUri = linkNode.Attributes["href"].Value;

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
                    
                    await File.WriteAllBytesAsync(cloud.FileLocation, content);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you should inject ILogger here)
                throw new InvalidOperationException($"Failed to update {cloud.CloudName}: {ex.Message}", ex);
            }
        }

    }

    public class Public : Cloud
    { 
        public override string Url => "https://www.microsoft.com/download/details.aspx?id=56519";
        public override string Filename => "Public.json";
    }

    public class AzureGovernment : Cloud
    { 
        public override string Url => "https://www.microsoft.com/download/details.aspx?id=57063";
        public override string Filename => "AzureGovernment.json";
    }

    public class China : Cloud
    { 
        public override string Url => "https://www.microsoft.com/download/details.aspx?id=57062";
        public override string Filename => "China.json";
    }

    public class AzureGermany : Cloud
    { 
        public override string Url => "https://www.microsoft.com/download/details.aspx?id=57064";
        public override string Filename => "AzureGermany.json";
    }

}
