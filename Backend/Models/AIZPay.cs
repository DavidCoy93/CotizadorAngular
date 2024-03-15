using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class AIZPay
    {
        public int Id { get; set; }
        public int IdVendor { get; set; }
        public string AIZPayData { get; set; }
        public string CrudHistoryList { get; set; }
        public bool Active { get; set; }
    }
}