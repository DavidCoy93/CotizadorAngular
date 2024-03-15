using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataCargaEndososNissanGap
    {
        public int PolizaId { get; set; }
        public string Certificado { get; set; }
        public float SumaAsegurada { get; set; }
        public string FechaEndoso { get; set; }
    }
}