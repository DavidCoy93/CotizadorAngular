using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class AvisoPrivacidad
    {
        public int Id { get; set; }
        public int IdClienteAviso { get; set; }
        public bool UseURL { get; set; }
        public string URL { get; set; }
        public string Aviso { get; set; }
        public bool Active { get; set; }
    }
}