using System;
using System.ComponentModel.DataAnnotations;

namespace MSSPAPI.Models
{
    public class ClaimFolio
    {
        [Key]
        public int IdFolio { get; set; }
        public string Folio { get; set; }
        public string Nombre { get; set; }
        public string Articulo { get; set; }
        public string Poliza { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }
        public string Delegacion { get; set; }
        public string CP { get; set; }
        public string Preguntas { get; set; }
        public string Mail { get; set; }
        public string Documento { get; set; }
        public string ClaimNumber { get; set; }
        public string Certificado { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}