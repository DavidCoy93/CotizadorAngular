using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataCargaEndososSiCrea
    {
        public int PolizaId { get; set; }
        public string Certificado { get; set; }
        public float SumaAsegurada { get; set; }
        public string FechaEndoso { get; set; }
        public string PolizaMaestra { get; set; }
        public int PlazoEditado { get; set; }
    }
}