using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class ServiceProviderConfiguration
    {
        public int Id { get; set; }
		public string Configuration { get; set; }
        public string ProviderName { get; set; }
		public string BaseUrl { get; set; }
		public string ApiKey { get; set; }
		public string SvcUsr { get; set; }
		public string SvcPwd { get; set; }
		public string SvcType { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime ModifiedDate { get; set; }
		public bool Active { get; set; }
    }
}