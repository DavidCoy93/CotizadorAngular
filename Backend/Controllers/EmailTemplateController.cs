using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailTemplateController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene los templates configurados en la bd
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetEmailTemplate")]
        public IHttpActionResult GetEmailTemplate(EmailTemplate email)
        {
            try
            {
                List<EmailTemplate> em =  DBOperations.GetEmailByCliente(email.IdCliente);
                return Ok(em);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtiene los templates configurados en la bd
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertEmailTemplate")]
        public IHttpActionResult InsertEmailTemplate(EmailTemplate email)
        {
            try
            {
                DBOperations.InsertEmailTemplate(email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtiene los templates configurados en la bd
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateEmailTemplate")]
        public IHttpActionResult UpdateEmailTemplate(EmailTemplate email)
        {
            try
            {
                DBOperations.UpdateEmailTemplate(email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtiene los templates configurados en la bd
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/DisableEmailTemplate")]
        public IHttpActionResult DisableEmailTemplate(EmailTemplate email)
        {
            try
            {
                DBOperations.DesactivaEmail(email.IdEmail);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}