using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class TransaccionData
    {
        public int IdTransaccion { get; set; }
        public string NumeroTransaccion { get; set; }
        public string TransaccionJson { get; set; }
    }
}