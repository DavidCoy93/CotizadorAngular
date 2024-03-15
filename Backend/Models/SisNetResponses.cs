using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisNetResponses
    {
        public string id { get; set; }
        public string data { get; set; }
        public int total { get; set; }
        public bool success { get; set; }
    }
}