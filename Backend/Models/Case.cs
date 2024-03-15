using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string CaseIdentifier { get; set; }
        public string PhoneNumber { get; set; }
        public int StatusId { get; set; }
        public string Status {  get; set; }
        public string CustomerData { get; set; }
        public string CrudHistoryList { get; set; }
        public bool Active { get; set; }
    }


    public class CaseStatus
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string CrudHistoryList { get; set; }
        public bool Active { get; set; }
    }

 
}