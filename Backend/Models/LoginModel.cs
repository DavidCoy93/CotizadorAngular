using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class LoginModel
    {
        public int IdLogin { get; set; }
        public string Usr { get; set; }
        public string Pd { get; set; }
        public int Timeout { get; set; }
        public bool Active { get; set; }
    }
}