using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisNetTracking
    {
        public int Id { get; set; }
        public string Command { get; set; }
        public int StCo { get; set; }
        public string Rqt { get; set; }
        public string Rsn { get; set; }
        public string CrudHistoryList { get; set; }
        public bool Active { get; set; }
    }
}