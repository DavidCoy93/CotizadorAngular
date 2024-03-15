using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataSisnetCargaDocumento
    {
        public int SiniestroId { get; set; }
        public string Cobertura { get; set; }
        public string Binario { get; set; }
        public string Nombre { get; set; }
        public bool EsRecaudo { get; set; }
        public string FechaSolicitud { get; set; }
    }
}