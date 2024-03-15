using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisNetObtenerCertificadoGenericoExternoResponse
    {
        public int PolizaId { get; set; }
        public string Estado { get; set; }
        public string EntidadFederativa { get; set; }
        public string NroPrestamo { get; set; }
        public DateTime FechaInicioVigenciaCertificado { get; set; }
        public DateTime FechaFinVigenciaCertificado { get; set; }
        public DateTime FechaInicioPolizaMaestra { get; set; }
        public DateTime FechaFinPolizaMaestra { get; set; }
        public string PolizaMaestra { get; set; }
        public string EndosoMaestra { get; set; }
        public string Producto { get; set; }
        public DateTime FechaAperturaCredito { get; set; }
        public DateTime FechaVtoCredito { get; set; }
        public int PlazoCredito { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string RfcAsegurado { get; set; }
        public bool EsPed { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public int Anio { get; set; }
        public string CodigoPostal { get; set; }
        public string Domicilio { get; set; }
        public string VIN { get; set; }
        public string Version { get; set; }
        public DateTime FechaCompraSeguro { get; set; }
        public int PlazoFabricante { get; set; }
        public string FechaInicioVigenciaPolizaPrimaria { get; set; }
        public bool EmitirCertificado { get; set; }
    }
}