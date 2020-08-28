using System;
using System.Collections.Generic;

namespace dotnet.Models
{

    abstract public class Cloud
    {
        public abstract string Url { get; }
        public abstract string Filename { get; }
    }

    public class PublicCloud : Cloud
    { 
        public override string Url {
            get {
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=56519";
            }
        }
        public override string Filename {
            get {
                return "data_public.json";
            }
        }

    }

    public class USGovCloud : Cloud
    { 
        
        public override string Url {
            get {
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=57063";
            }
        }
        public override string Filename {
            get {
                return "data_usgov.json";
            }
        }
      
    }

    
    public class ChinaCloud : Cloud
    { 
       
        public override string Url {
            get {
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=57062";
            }
        }
        public override string Filename {
            get {
                return "data_china.json";
            }
        }
       
    }

    
    public class GermanyCloud : Cloud
    { 
        
        public override string Url {
            get {
                return "https://www.microsoft.com/en-us/download/confirmation.aspx?id=57064";
            }
        }
        public override string Filename {
            get {
                return "data_germany.json";
            }
        }
        
    }

}
