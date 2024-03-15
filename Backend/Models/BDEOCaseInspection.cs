using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class BDEOCaseInspection
    {
        public int Id { get; set; } 
        public string RequestData { get; set; }
        public string CaseData { get; set; }
        public string CrudHistoryList { get; set; }
        public bool Active { get; set; }
    }
}