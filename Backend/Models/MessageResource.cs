using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Web;

namespace MSSPAPI.Models
{
    public class MessageResource
    {
        public string body { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public List<Uri> media { get; set; }
    }
}