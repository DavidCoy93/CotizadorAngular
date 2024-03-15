using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class EmailTemplate
    {
        public int IdEmail { get; set; }
        public string Descripcion { get; set; }
        public string BodyName { get; set; }
        public string Body { get; set; }
        public string SubjectPre { get; set; }
        public string SubjectPost { get; set; }
        public string CCEmails { get; set; }
        public string BCCEmails { get; set; }
        public bool Active { get; set; }
        public int IdCliente { get; set; }
        public string EmailFrom { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string EmailSenderName { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
        public string MessageMail { get; set; }



    }
}