using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class FlujoAlterno
    {
        public int IdCliente { get; set; }
        public bool Enabled { get; set; }
        public bool EnabledLogin { get; set; }
        public bool EnabledMultiple { set; get; }
    }
}