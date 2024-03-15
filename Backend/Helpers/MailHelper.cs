using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Microsoft.Configuration.ConfigurationBuilders;
using System.Net.Mime;

namespace MSSPAPI.Helpers
{
    internal class MailHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="To"></param>
        /// <param name="From"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        internal static bool SendMail(string To, string From, string Subject, string Body, string server, string port, string from, string file, string CCEmails)//, string cc, string bcc)
        {
            var smtpClient = new SmtpClient(server, Convert.ToInt32(port))
            {
                UseDefaultCredentials = false,
                EnableSsl = false
            };
            MailMessage eml = new MailMessage();
            eml.To.Add(new MailAddress(To));
            eml.From = new MailAddress(from, From);
            eml.Subject = Subject;
            string[] cc = string.IsNullOrWhiteSpace(CCEmails) ? new string[0] : CCEmails.Split(';');
            foreach (string ccid in cc)
            {
                eml.CC.Add(new MailAddress(ccid));
            }
            eml.SubjectEncoding = System.Text.Encoding.UTF8;
            eml.Body = Body;

            if (file != null && file != "")
            {
                byte[] archivoBytes = Convert.FromBase64String(file);
                Stream stream = new MemoryStream(archivoBytes);
                string ext = ClaimFolioHelper.GetFileExtension(file);
                Attachment data = new Attachment(stream, "archivoadjunto." + ext);//aqui va el nombre del archivo con extension
                eml.Attachments.Add(data);
            }

            eml.IsBodyHtml = true;
            eml.Priority = MailPriority.Normal;
            try
            {
                smtpClient.EnableSsl = true;
                smtpClient.Send(eml);
                eml.Dispose();
                return true;
            }
            catch (SmtpException)
            {
                return false;
            }
        }

        internal static bool SendMailSendCliente(string To, string From, string Subject, string Body, string server, string port, string from, string file)//, string cc, string bcc)
        {
            var smtpClient = new SmtpClient(server, Convert.ToInt32(port))
            {
                UseDefaultCredentials = false,
                EnableSsl = false
            };
            MailMessage emlC = new MailMessage();
            emlC.To.Add(new MailAddress(To));
            emlC.From = new MailAddress(from, From);
            emlC.Subject = Subject;
            emlC.SubjectEncoding = System.Text.Encoding.UTF8;
            emlC.Body = Body;

            if (file != null && file != "")
            {
                byte[] archivoBytes = Convert.FromBase64String(file);
                Stream stream = new MemoryStream(archivoBytes);
                string ext = ClaimFolioHelper.GetFileExtension(file);
                Attachment data = new Attachment(stream, "archivoadjunto." + ext);//aqui va el nombre del archivo con extension
                emlC.Attachments.Add(data);
            }

            emlC.IsBodyHtml = true;
            emlC.Priority = MailPriority.Normal;
            try
            {
                smtpClient.EnableSsl = true;
                smtpClient.Send(emlC);
                emlC.Dispose();
                return true;
            }
            catch (SmtpException)
            {
                return false;
            }
        }
        /// <summary>
        /// sending the notification mail after n (parameter) attempts
        /// </summary>
        public static void SendEmail(string tipo, int id, string correo, string nombre, string claim)
        {
            string partialPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            EmailTemplate els = Body(tipo, id);
            Cliente cl = DBOperations.GetClientesById(id);
            if (els.MessageMail != null)
            {
                string newBody = File.ReadAllText(Path.Combine(partialPath, "Templates\\Liverpool\\TemplateLiverpool.html"))
                             .Replace("{name}", nombre)
                             .Replace("{numserv}", claim)
                             .Replace("{messagetitle}", els.MessageTitle)
                             .Replace("{messagebody}", els.MessageBody)
                             .Replace("{messagemail}", els.MessageMail);

                MailHelper.SendMail(
                                correo, //parametro del cliente a quien se le enviara el correo
                                els.EmailSenderName, //Sender name
                                els.SubjectPre + " " + claim + " " + els.SubjectPost,
                                newBody,
                                els.Server,
                                els.Port,
                                els.EmailFrom,
                                null,
                                els.CCEmails
                                );

            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="id"></param>
        /// <param name="correo"></param>
        /// <param name="nombre"></param>
        /// <param name="claim"></param>
        public static void SendEmailAlternoComercial(string tipo, int id, string Mail, string Nombre, string Folio, string Articulo, string Direccion, string CP, string Delegacion, string Estado, string Telefono, string Poliza, string Preguntas, string Documento, string ClaimNumber, string certificado)
        {
            string partialPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            EmailTemplate els = Body(tipo, id);
            Cliente cl = DBOperations.GetClientesById(id);
            string newBody = File.ReadAllText(Path.Combine(partialPath, "Templates\\Liverpool\\TemplateFlujoComercial.html"))
            .Replace("{folio}", Folio)
            .Replace("{name}", Nombre)
            .Replace("{items}", Articulo)
            .Replace("{poliza}", Poliza)
            .Replace("{certificado}", certificado)
            .Replace("{Telefono}", Telefono)
            .Replace("{Mail}", Mail)
            .Replace("{direccion}", Direccion)
            .Replace("{estado}", Estado)
            .Replace("{delegacion}", Delegacion)
            .Replace("{cp}", CP)
            .Replace("{preguntas}", Preguntas)
            .Replace("{messagetitle}", els.MessageTitle)
            .Replace("{messagebody}", els.MessageBody)
            .Replace("{messagemail}", els.MessageMail); MailHelper.SendMail(
            "edgar.mendoza@assurant.com", //parametro de comercial a quien se le enviara el correo TODO: CAMBIAR CORREO A COMERCIAL
                                       els.EmailSenderName, //Sender name
                                       els.SubjectPre + " " + Folio + " " + els.SubjectPost,
            newBody,
            els.Server,
            els.Port,
            els.EmailFrom,
            Documento,
            els.CCEmails
            );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="id"></param>
        /// <param name="correo"></param>
        /// <param name="nombre"></param>
        /// <param name="claim"></param>
        public static void SendEmailAlternoCliente(string tipo, int id, string correo, string nombre, string folio)
        {
            string partialPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
            EmailTemplate els = Body(tipo, id);
            Cliente cl = DBOperations.GetClientesById(id);
            string newBody = File.ReadAllText(Path.Combine(partialPath, "Templates\\Liverpool\\TemplateFlujoCliente.html"))
                             .Replace("{name}", nombre)
                             .Replace("{folio}", folio)
                             .Replace("{messagetitle}", els.MessageTitle)
                             .Replace("{messagebody}", els.MessageBody)
                             .Replace("{messagemail}", els.MessageMail);

            MailHelper.SendMailSendCliente(
                            correo, //parametro del cliente a quien se le enviara el correo
                            els.EmailSenderName, //Sender name
                            els.SubjectPre + " " + folio + " " + els.SubjectPost,
                            newBody,
                            els.Server,
                            els.Port,
                            els.EmailFrom,
                            null
                            );
        }

        /// <summary>
        /// Creation of a little body in html about the notification timeout with the respectives parameters
        /// showing them in the body page
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="serviceMethodMadeCall"></param>
        /// <param name="serviceMethodFailed"></param>
        /// <returns>HTML body that contains the notification related to the timeout</returns>
        private static EmailTemplate Body(string tipo, int id)
        {
            var htmlBody = new StringBuilder();
            EmailTemplate els = DBOperations.GetEmailTemplate(tipo, id);
            return els;
        }
    }
}