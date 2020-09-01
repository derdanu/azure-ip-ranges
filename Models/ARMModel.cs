using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace dotnet.Models
{
    public class ARMModel
    {
        [JsonIgnore]
        public string filename { get; set; } 
        [JsonPropertyName("$schema")]
        public string schema { get; set; }
        public string contentVersion { get; set; }
        public string apiProfile { get; set; }
        public Parameters parameters { get; set; }
        public List<Variables> variables { get; set; }
        public List<Functions> functions { get; set; }
        public List<Resources> resources { get; set; }
        public List<Outputs> outputs { get; set; }
    }

    public class Parameters
    {
        public ParameterOptions  name { get; set; }

    }

    public class ParameterOptions
    {
        public String type { get; set; }
        public String defaultValue { get; set; }

    }

    public class Variables
    {

    }

    public class Functions
    {

    }

    public class Resources
    {
        public string type { get; set; }
        public string name { get; set; }
        public string apiVersion { get; set; }
        public string location { get; set; }
        public Properties properties { get; set; }

    }

    public class Outputs
    {

    }

    public class Properties {

       public List<Route> routes { get; set; } 

    }

    public class Route {

        public string name { get; set; }
        public RouteProperties properties { get; set; }

    }

    public class RouteProperties {
        public string addressPrefix { get; set; }
        public string nextHopType { get; set; }
        public string nextHopIpAddress { get; set; }

    }


}
