using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class LoginAccess
    {
   
        public int IdCliente { get; set; }
        public string CertificateNumber { get; set; }
        public string ZipCode { get; set; }    
        public int Tipo { get; set; }

    }
}