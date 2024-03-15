using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    /// <summary>
    /// Clase repersentativa para el campo data del request de Carga Siniestro Nizzan GAP y Mazda GAP
    /// </summary>
    public class DataSisNetCargaSiniestroMazdaNissan
    {
        public int PolizaCertificado { get; set; }
        public string Provincia { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public string FechaSiniestro { get; set; }
        public string FechaNotificacion { get; set; }
        public string TipoPerdida { get; set; }
        public string Reclamante { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string EmailCorredor { get; set; }
        public string TelefonoCorredor { get; set; }
        public string NombreBeneficiario { get; set; }
        public string NombreCausa { get; set; }

        public string DetalleCausa { get; set; }
        public bool EsRecuperable { get; set; }
        public int Latitud { get; set; }
        public int Longitud { get; set; }
        public string TipoSiniestro { get; set; }
        public string Lugar { get; set; }
        public string DescripcionCausa { get; set; }
        public int ValorFactura { get; set; }
        public int ValorRecuperado { get; set; }
        public decimal DeducibleAplicado { get; set; }
    }
}