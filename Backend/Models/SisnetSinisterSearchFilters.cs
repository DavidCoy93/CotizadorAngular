using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisnetSinisterSearchFilters
    {
        public int? PolizaId { get; set; }
        public int? SiniestroId { get; set; }
        public string CodigoSiniestro { get; set; }
        public string CodigoSecundarioCertificado { get; set; }

    }
}