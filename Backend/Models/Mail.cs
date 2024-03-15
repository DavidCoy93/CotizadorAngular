using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
	public class Mail
	{
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string server { get; set; }
        public string port { get; set; }
        public string file { get; set; }
        public string CCEmails { get; set; }
    }
}