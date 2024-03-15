using MSSPAPI.Helpers;
using MSSPAPI.Models;
using MSSPAPI.PolicyServiceWS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    public class AIZPayController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el AIZPAY activados
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAIZPay")]
        public IHttpActionResult GetAIZPay()
        {
            try
            {
                List<AIZPay> dc = DBOperations.GetAllAIZPay();
                return Ok(dc);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtiene el AIZPay por proveedor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAIZPay/{id}")]
        public IHttpActionResult GetAIZPay(int id)
        {
            try
            {
                Cliente cl = DBOperations.GetClientesById(id); //Constantes cuando haya mas clientes
                string jsonConfig = cl.Configuraciones;
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);

                List<AIZPay> dc = DBOperations.GetAIZPayByVendor(Convert.ToInt32(jsonObj["IdVendor"]));
                return Ok(dc);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///Insert un nuevo registro en aiz pay
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertAIZPay")]
        public IHttpActionResult InsertAIZPay(AIZPay dl)
        {
            try
            {

                DBOperations.InsertAIZPay(dl);
                return Ok(dl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Actualiza el registro de aiz pay
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateAIZPay")]
        public IHttpActionResult UpdateAIZPay(AIZPay dl)
        {
            try
            {
                DBOperations.UpdateAIZPay(dl);
                return Ok(dl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Desactiva el registro de aiz pay
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/DeleteAIZPay")]
        public IHttpActionResult DeleteAIZPay(int id)
        {
            try
            {
                DBOperations.DeleteAIZPay(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}