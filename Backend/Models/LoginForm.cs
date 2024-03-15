using System;
using System.ComponentModel.DataAnnotations;

namespace MSSPAPI.Models
{
    public class LoginForm
    {
        [Key]
        public int IdCliente { get; set; }
        public string CertificateNumber { get; set; }
        public string Email { get; set; }
 

    }
}