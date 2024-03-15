using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class TiposBusqueda
    {
        public int IdMetodoBusqueda { get; set; }
        public string MetodoBusqueda { get; set; }
        public int IdCliente { get; set; }
        public string Parametros { get; set; }

    }
}