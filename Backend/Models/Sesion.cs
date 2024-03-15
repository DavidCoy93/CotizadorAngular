using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Sesion
    {
        public int IdSesion { get; set; }
        public int IdCliente { get; set; }
        public string Token { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public bool Active { get; set; }
    }
}