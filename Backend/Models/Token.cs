using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    /// <summary>
    /// class token
    /// </summary>
    public class Token
    {
        public int IdSesion { get; set; }
        public int IdCliente { get; set; }
        public string Tokens { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Active { get; set; }
    }
}