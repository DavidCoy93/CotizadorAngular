using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataBeginClaim
    {
        public int IdCliente { get; set; }
        public string CertificateNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RelationshipTypeCode { get; set; }
        public string ChannelCode { get; set; }
        public string EmailAddress { get; set; }
        public string CultureCode { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ClientIpAddress { get; set; }
        public string CallBackNumber { get; set; }
        public string PurposeCode { get; set; }
        public string DealerCode { get; set; }
        public string R3Fechaservicio { get; set; }

        public string Poliza { get; set; }
        public string CompanyCode { get; set; }

    }

}