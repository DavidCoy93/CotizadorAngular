using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    /// <summary>
    /// Filtro para obtener un listado de certificados de los productos
    /// NCPI, SiCrea, MazdaGAP y Nissan Gap por busqueda por codigosecundario
    /// o id de la poliza
    /// </summary>
    public class DataCertificadoGenericoExterno
    {
        public string PolizaId { get; set; }
        public string Certificado { get; set; }
    }
}