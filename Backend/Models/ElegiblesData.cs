using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MSSPAPI.Models
{
    public class ElegiblesData
    {
        [Key]
        public int IdElegible { get; set; }
        public string Poliza { get; set; }
        public string IdCliente { get; set; }
        public string ElegibleData { get; set; }
        public string ElegibleHistoryList { get; set; }
        public bool Active { get; set; }
    }
}