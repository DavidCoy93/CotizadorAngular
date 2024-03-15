using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSSPAPI.Controllers
{
    public class EmailTesterController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: BeginClaim
        /// <summary>
        /// Registra el claim hacia Elita
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/emailtester")]
        public IHttpActionResult GetEmailTester()
        {
            try
            {
                MailHelper.SendEmail("New", 1, "jose.miranda@it-seekers.com", "Alberto Miranda", "234567");
                return Ok("SE HA ENVIADO EL CORREO AL REMITENTE");
            }
            catch
            {
                return BadRequest();
            }
                
        }
    }
}