using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class CertDetail
    {
        public string CERT_NUMBER { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string WORK_PHONE { get; set; }
        public string FROM_DATE { get; set; }
        public string TO_DATE { get; set; }
        public string PREMIUM { get; set; }
    }
}