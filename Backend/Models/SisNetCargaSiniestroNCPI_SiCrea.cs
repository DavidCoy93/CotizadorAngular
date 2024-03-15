using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisNetCargaSiniestroNCPI_SiCrea
    {
        public int PolizaCertificadoId { get; set; }
        public DateTime FechaSiniestro { get; set; }
        public DateTime FechaNotificacion { get; set; }
        public string TipoPerdida { get; set; }
        public string Reclamante {  get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Provincia { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public string EmailCorredor { get; set; }
        public string TelefonoCorredor { get; set; }
        public string NombreBeneficiario { get; set; }
        public string NombreCausa { get; set; }
        public string DetalleCausa { get; set; }
        public bool EsRecuperable { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string TipoSiniestro { get; set; }
        public string Lugar {  get; set; }
        public string DescripcionCausa { get; set; }
        //public double ValorFactura { get; set; }
        //public double ValorRecuperado { get; set; }
        //public double DeducibleAplicado {  get; set; }

    }
}