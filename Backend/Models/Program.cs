using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Program
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string CertificateIdentifier { get; set; }
        public int SalesChannel {  get; set; }
        public int IdClient { get; set; }
        public string CrudHistoryList { get; set; }
        public bool Active { get; set; }
        public List<Product> Products { get; set; }
    }
}