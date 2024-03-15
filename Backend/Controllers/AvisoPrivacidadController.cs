using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    public class AvisoPrivacidadController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el Aviso Privacidad code por el id del cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAvisoPrivacidad")]
        public IHttpActionResult GetAvisoPrivacidad()
        {
            try
            {
                List<AvisoPrivacidad> dc = DBOperations.GetAvisoPrivacidadKitt();
                return Ok(dc);
            }
            catch (TimeoutException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el Aviso Privacidad code por el id del cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAvisoPrivacidad/{id}")]
        public IHttpActionResult GetAvisoPrivacidad(int id)
        {
            try
            {
                List<AvisoPrivacidad> dc = DBOperations.GetAvisoPrivacidadKittByCliente(id);
                return Ok(dc);
            }
            catch (TimeoutException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertAvisoPrivacidadKitt")]
        public IHttpActionResult InsertAvisoPrivacidadKitt(AvisoPrivacidad dl)
        {
            try
            {

                DBOperations.InsertAvisoPrivacidadKitt(dl);
                return Ok(dl);
            }
            catch (TimeoutException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateAvisoPrivacidadKitt")]
        public IHttpActionResult UpdateAvisoPrivacidadKitt(AvisoPrivacidad dl)
        {
            try
            {
                DBOperations.UpdateAvisoPrivacidadKitt(dl);
                return Ok(dl);
            }
            catch (TimeoutException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/DeleteAvisoPrivacidadKitt")]
        public IHttpActionResult DeleteAvisoPrivacidadKitt(int id)
        {
            try
            {
                DBOperations.DeleteAvisoPrivacidadKitt(id);
                return Ok();
            }
            catch (TimeoutException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}