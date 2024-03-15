using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class CrudHistory
    {
        public DateTime Date { get; set; }
        public char Type { get; set; }
        public int IdUser { get; set; }
        public string Comments { get; set; }
    }
}