using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class IndividualMultiplePDF
    {
        public string POLICY_NUMBER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string DATE_PLACE_OF_BIRTH { get; set; }
        public string NATIONALITY { get; set; }
        public string GENDER { get; set; }
        public string MARTIAL_STATUS { get; set; }
        public string ADDRESS { get; set; }
        public string TAX_ID { get; set; }
        public string CUIL { get; set; }
        public string WORK_PHONE { get; set; }
        public string EMAIL { get; set; }
        public string PROFESSION { get; set; }
        public List<CertDetail> CertList { get; set; }
    }
}