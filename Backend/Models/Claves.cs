using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Claves
    {
        public int IdClaveCliente { get; set; }
        public int IdCliente { get; set; }
        public string Clave { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaCaducidad { get; set; }
    }
}