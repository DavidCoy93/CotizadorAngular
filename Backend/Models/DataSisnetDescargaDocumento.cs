using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataSisnetDescargaDocumento
    {
        public string Accion { get; set; }
        public int?  DocumentoId { get; set; }
        public int? SiniestroId { get; set; }
    }
}