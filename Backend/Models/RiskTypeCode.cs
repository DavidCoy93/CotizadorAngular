using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MSSPAPI.Models
{
    public class RiskTypeCode
    {
        [Key]
        public int IdRiskTypeCode { get; set; }

        public int IdCliente { get; set; }
        public string RiskType { get; set; }
        public string Marca { get; set; }
    }
}