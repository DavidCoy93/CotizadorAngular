using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    public class EncuestaController :ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el Aviso Privacidad code por el id del cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetEncuesta")]
        public IHttpActionResult GetEncuesta()
        {
            try
            {
                List<Encuesta> dc = DBOperations.GetEncuestaKitt();
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
        [Route("api/GetEncuesta/{id}")]
        public IHttpActionResult GetEncuesta(int id)
        {
            try
            {
                List<Encuesta> dc = DBOperations.GetEncuestaKittByCliente(id);
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
        [Route("api/InsertEncuestaKitt")]
        public IHttpActionResult InsertEncuestaKitt(Encuesta dl)
        {
            try
            {

                DBOperations.InsertEncuestaKitt(dl);
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
        [Route("api/UpdateEncuestaKitt")]
        public IHttpActionResult UpdateEncuestaKitt(Encuesta dl)
        {
            try
            {
                DBOperations.UpdateEncuestaKitt(dl);
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
        [Route("api/DeleteEncuestaKitt")]
        public IHttpActionResult DeleteEncuestaKitt(int id)
        {
            try
            {
                DBOperations.DeleteEncuestaKitt(id);
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