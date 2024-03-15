using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DocumentCertification
    {
        public int IdCliente { get; set; }
        public string Country { get; set; }
        public string Certificate { get; set; }
        public string DealerCode { get; set; }
        public string IdCertificate { get; set; }
        public string Base64File { get; set; }
        public string Extension { get; set; }
    }
}