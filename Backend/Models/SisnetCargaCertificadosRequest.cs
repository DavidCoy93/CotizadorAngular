using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisnetCargaCertificadosNCPI
    {
        public string NroPrestamo { get; set; }
        public bool EsPEP { get; set; }
        public string FechaInicioVigenciaCertificado { get; set; }
        public string FechaFinVigenciaCertificado { get; set; }
        public string FechaAperturaCredito { get; set; }
        public string FechaVtoCredito { get; set; }
        public int PlazoCredito { get; set; }
        public float MontoPrestamo { get; set; }
        public float SaldoInsoluto { get; set; }
        public string NombreCliente { get; set; }
        public string RfcAsegurado { get; set; }
        public string FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public float FactorSeguroDeVida { get; set; }
        public string PolizaMaestra { get; set; }
        public string FechaInicioPolizaMaestra { get; set; }
        public string FechaFinPolizaMaestra { get; set; }
        public string Estado { get; set; }
        public string Ciudad { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public float ValorFactura { get; set; }
        public int CodigoPostal { get; set; }
        public string Domicilio { get; set; }
        public bool EmitirCertificado { get; set; }
        public string EndosoMaestraId { get; set; }
        public int ProductoId { get; set; }
    }

    public class SisnetCargaCertificadosSiCrea
    {
        public string RfcAsegurado { get; set; }
        public string FechaNacimiento { get; set; }
        public string FechaAperturaCredito { get; set; }
        public string FechaVtoCredito { get; set; }
        public int PlazoCredito { get; set; }
        public string FechaInicioPolizaMaestra { get; set; }
        public string FechaFinPolizaMaestra { get; set; }
        public float MontoPrestamo { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Sexo { get; set; }
        public string PolizaMaestra { get; set; }
        public string NroPrestamo { get; set; }
        public string Nacionalidad { get; set; }
        public string EntidadFederativa { get; set; }
        public int Anio { get; set; }
        public string CodigoPostal { get; set; }
        public string Domicilio { get; set; }
        public float CuotaMensual { get; set; }
        public string ActividadAsegurado { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Beneficiario1Rfc { get; set; }
        public string Beneficiario1Nombre { get; set; }
        public string Beneficiario1ApellidoPaterno { get; set; }
        public string Beneficiario1ApellidoMaterno { get; set; }
        public string Beneficiario2Rfc { get; set; }
        public string Beneficiario2Nombre { get; set; }
        public string Beneficiario2ApellidoPaterno { get; set; }
        public string Beneficiario2ApellidoMaterno { get; set; }
        public string Beneficiario3Rfc { get; set; }
        public string Beneficiario3Nombre { get; set; }
        public string Beneficiario3ApellidoPaterno { get; set; }
        public string Beneficiario3ApellidoMaterno { get; set; }
        public bool EmitirCertificado { get; set; }
        public string EndosoMaestraId { get; set; }
        public string ProductoId { get; set; }
    }

    public class SisnetCargaCertificadosMazdaGap
    {
        public string NroPrestamo { get; set; }
        public string FechaInicioVigenciaCertificado { get; set; }
        public string FechaFinVigenciaCertificado { get; set; }
        public string FechaCompraSeguro { get; set; }
        public string FechaAperturaCredito { get; set; }
        public string FechaInicioVigenciaPolizaPrimaria { get; set; }
        public int PlazoFabricante { get; set; }
        public int PlazoCredito { get; set; }
        public float ValorFactura { get; set; }
        public string CategoriaUso { get; set; }
        public string CanalVenta { get; set; }
        public string ProgramaNRFM { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Version { get; set; }
        public int Anio { get; set; }
        public string VIN { get; set; }
        public string NombreCliente { get; set; }
        public string RfcAsegurado { get; set; }
        public string FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Email1 { get; set; }
        public string Telefono1 { get; set; }
        public float CostoSeguro { get; set; }
        public int ContratanteId { get; set; }
        public string PolizaMaestra { get; set; }
        public string FechaInicioPolizaMaestra { get; set; }
        public string FechaFinPolizaMaestra { get; set; }
        public bool EmitirCertificado { get; set; }
        public string EndosoMaestraId { get; set; }
        public string ProductoId { get; set; }
    }

    public class SisnetCargaCertificadosNissanGap
    {
        public string NroPrestamo { get; set; }
        public string FechaInicioVigenciaCertificado { get; set; }
        public string FechaFinVigenciaCertificado { get; set; }
        public string FechaCompraSeguro { get; set; }
        public string FechaAperturaCredito { get; set; }
        public string FechaInicioVigenciaPolizaPrimaria { get; set; }
        public int PlazoFabricante { get; set; }
        public int PlazoCredito { get; set; }
        public float ValorFactura { get; set; }
        public string CategoriaUso { get; set; }
        public string CanalVenta { get; set; }
        public string ProgramaNRFM { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Version { get; set; }
        public int Anio { get; set; }
        public string VIN { get; set; }
        public string NombreCliente { get; set; }
        public string RfcAsegurado { get; set; }
        public string FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Email1 { get; set; }
        public string Telefono1 { get; set; }
        public float CostoSeguro { get; set; }
        public int ContratanteId { get; set; }
        public bool EmitirCertificado { get; set; }
        public string ProductoId { get; set; }

    }
}