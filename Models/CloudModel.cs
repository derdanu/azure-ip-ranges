using System;
using System.Collections.Generic;

namespace dotnet.Models
{

    abstract public class Cloud
    {
        public abstract string Url { get; }
        public abstract string Filename { get; }
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
                return "data_Public.json";
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
                return "data_AzureGovernment.json";
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
                return "data_China.json";
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
                return "data_AzureGermany.json";
            }
        }
        
    }

}
