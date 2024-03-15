using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class AzureBlob
    {
        public string Uri { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }
    }
}