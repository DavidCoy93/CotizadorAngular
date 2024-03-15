using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Apikey
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApiKey { set; get; }
        public string Authorization { get; set; }
        public string RiskTypeCode { get; set; }
        public string Tokens { get; set; }
        public bool Multiple { get; set; }
        public string URLHeader { get; set; }
        public string URLFooter { get; set; }
    }
}