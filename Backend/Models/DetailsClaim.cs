using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DetailsClaim
    {
        public int IdCliente { get; set; }
        public string ClaimNumber { get; set; }
        public string DealerCode { get; set; }
        public string CompanyCode { get; set; }
    }
}