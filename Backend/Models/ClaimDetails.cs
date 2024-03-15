using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ClaimDetails
    {
        [Key]
        public int IdClaim { get; set; }
        public string NumeroCaso { get; set; }
        public string ClaimNumber { get; set; }
        public string NumeroSerie { get; set; }
        public string RespInfoPersonal { get; set; }
        public string RespInfoDispositivo { get; set; }
        public string RespInfoPreguntas { get; set; }
        public string RespInfoClaimDescription { get; set; }
        public string RespDireccion { get; set; }
        public string RespInfoCLaimDetails { get; set; }
        public string CertificateNumber { get; set; }
        public string NombreCliente { get; set; }

        public string CorreoCliente { get; set; }

        public string StatusType { get; set; }

        public int IdCliente { get; set; }
        public string CertificadoPadre { get; set; }
    }

}