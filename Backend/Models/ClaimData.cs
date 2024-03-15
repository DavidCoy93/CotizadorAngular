using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class ClaimData
    {
        [Key]
        public int IdClaim { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUpdate { get; set; }
        public string Poliza { get; set; }
        public int IdCliente { get; set; }
        public string ClaimDatas { get; set; }
        public string ClaimHistoryList { get; set; }
        public string DealerCode { get; set; }
        public bool Active { get; set; }
    }
}