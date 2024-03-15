using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class SisNetCargaCancelacionCertificadoNCPI_MazdaGap_SiCrea
    {
        public string Certificado { get; set; }
        public string FechaCancelacion { get; set; }
        public string PolizaMaestra { get; set; }
        public string FechaInicioVigenciaPolizaMaestra { get; set; }
        public string FechaFinVigenciaPolizaMaestra { get; set; }
        public string MotivoCancelacion { get; set; }
    }

    public class SisNetCargaCancelacionCertificadoNissanGap
    {
        public string Certificado { get; set; }
        public string FechaCancelacion { get; set; }
        public string MotivoCancelacion { get; set; }
    }
}