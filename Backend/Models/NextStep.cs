using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class NextStep
    {
        public int IdCliente { get; set; }
        public string DealerCode { get; set; }
        public string CoverageType { set; get; }
        public string MethodOfRepairCode { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string RiskType { get; set; }
        public string Languaje { get; set; }
    }
}