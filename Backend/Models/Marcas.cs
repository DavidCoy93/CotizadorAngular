using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Marcas
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Branch { get; set; }
        public ConfigurationBranch Configuration { get; set; }
        public string CrudHistoyList { get; set; }
        public string TokenEnrollment { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Active { get; set; }
    }

    public class ConfigurationBranch
    {
        public string UsrServiceConsumer { get; set; }
        public string PwdServiceConsumer { get; set; }
        public string GroupServiceConsumer { get; set; }
    }
}