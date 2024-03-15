using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MSSPAPI.Models
{
    public class SisNetResponse
    {
        [JsonProperty(propertyName: "$id")]
        public string id { get; set; }
        public SisNetResponseData[] data { get; set; }
        public int total { get; set; }
        public bool sucess { get; set; }
    }

    public class SisNetResponseData
    {
        [JsonProperty(propertyName: "$id")]
        public string id { get; set; }
        public string texto { get; set; }
        public string href { get; set; }
        public string tipo { get; set; }
    }
}