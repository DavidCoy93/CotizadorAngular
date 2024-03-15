using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class TrackingCode
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Code { get; set; }
        public string Text { set; get; }
        public bool Visible { get; set; }
        public bool comment_extra { get; set; }



    }
}