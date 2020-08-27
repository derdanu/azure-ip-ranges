using System;
using System.Collections.Generic;

namespace dotnet.Models
{
    public class ServiceTagsModel
    {
        public int changeNumber { get; set; }
        public string cloud { get; set; }
        public List<ValuesModel> values { get; set; }

    }

    public class ValuesModel
    {
        public string name { get; set; }
        public string id { get; set; }
        public PropertiesModel properties { get; set; }

    }


    public class PropertiesModel
    {
        public int changeNumber { get; set; }
        public string region { get; set; }
        public string platform { get; set; }
        public string systemService { get; set; }
        public List<string> addressPrefixes { get; set; }

    }

}
