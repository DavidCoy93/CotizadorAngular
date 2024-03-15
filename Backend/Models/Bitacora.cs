using System;
using System.ComponentModel.DataAnnotations;

namespace MSSPAPI.Models
{
    public class Bitacora
    {
        [Key]
        public int IdBitacora { get; set; }
        public string IP { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Navegador { get; set; }
        public string Usuario { get; set; }
        public string Descripcion { get; set; }
        public string Plataforma { get; set; }

    }
}