using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Encuesta
    {
        public int Id { get; set; }
        public int IdCteEnc { get; set; }
        public bool UURL { get; set; }
        public string URLPgts { get; set; }
        public string Prgts { get; set; }
        public string Respts { get; set; }
        public bool Active { get; set; }
    }
}