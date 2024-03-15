using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class ApiKeyKitt
    {
        public int Id { get; set; } 
        public int IdCliente { get; set; }
        public int TenantId { get; set; }
        public string ApiKey { get; set; }
        public string Usernamemail { get; set; }
        public string Pwd { get; set; }
    }
}