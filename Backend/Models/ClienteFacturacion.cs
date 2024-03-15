using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSSPAPI.Models
{
    public class ClienteFacturacion
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string RFC { get; set; }
        public string Names { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Genre { get; set; }
        public string RegimenFiscal { get; set; }
        public DateTime DateCreation { get; set; }
        public Guid UUID { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Colonia { get; set; }
        public string CP { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Localidad { get; set; }
        public string PhoneType { get; set; }
        public string Telefono { get; set; }
        public DateTime DateInicioPoliza { get; set; }
        public DateTime DateFinPoliza { get; set; }
        public string Currency { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal PrecioSIVA { get; set; }
        public decimal IVA { get; set; }
        public string Email { get; set; }
        public string VIN { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Anio { get; set; }
        public string UsoCFDI { get; set; }
        public int IdEmisores { get; set; }
        public int Cantidad { get; set; }
        public string CertificateNumber { get; set; }
        public string XMLResponse { get; set; }
        public bool RequiereFactura { get; set; }
        public string URLCertificate { get; set; }
        public string InvoiceXML { get; set; }
        public string InvoicePDF { get; set; }
    }
}