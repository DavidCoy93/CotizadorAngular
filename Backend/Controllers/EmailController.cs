using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Http;


namespace MSSPAPI.Controllers
{
    public class EmailController : ApiController
    {
        [HttpPost]
        [Route("api/email/send")]
        public IHttpActionResult Send(Models.Mail mailObject)
        {
            try
            {
                EmailTemplate els = Body("LATAM", 1);
           
                bool mailSent = MailHelper.SendMail(mailObject.To, els.EmailSenderName, mailObject.Subject, mailObject.Body, els.Server, els.Port, els.EmailFrom, null, els.CCEmails);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        private static EmailTemplate Body(string tipo, int id)
        {
            var htmlBody = new StringBuilder();
            EmailTemplate els = DBOperations.GetEmailTemplate(tipo, id);
            return els;
        }

    }
}
