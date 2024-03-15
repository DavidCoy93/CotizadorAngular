using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisNetRequest
    {
        public int Start { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public string Comando { get; set; }
        public string Data { get; set; }
        public int UserGroupData { get; set; }

    }
}