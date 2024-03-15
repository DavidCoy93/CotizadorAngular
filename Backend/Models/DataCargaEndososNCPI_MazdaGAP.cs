using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataCargaEndososNCPI_MazdaGAP
    {
        public int PolizaId { get; set; }
        public string Certificado { get; set; }
        public float SumaAsegurada { get; set; }
        public string FechaEndoso { get; set; }
        public string PolizaMaestra { get; set; }
    }
}