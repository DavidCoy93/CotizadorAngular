using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    public class DataEvaluationController : ApiController
    {
        /// <summary>
        /// obtiene todos los registros de data evaluation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("api/GetDataEvaluationKitt")]
        public IHttpActionResult GetDataEvaluationKitt()
        {
            try
            {

                List<DataEvaluation> de =  DBOperations.GetDataEvaluationKitt();
                return Ok(de);
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
        /// Obtiene un registro de data evaluation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize]
        [Route("api/GetOneDataEvaluationKitt/{id}")]
        public IHttpActionResult GetOneDataEvaluationKitt(int id)
        {
            try
            {

                List<DataEvaluation> de = DBOperations.GetOneDataEvaluationKitt(id);
                return Ok(de);
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
        /// Inserta un nuevo registro en data evaluation
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/InsertDataEvaluationKitt")]
        public IHttpActionResult InsertDataEvaluationKitt(DataEvaluation dl)
        {
            try
            {

                DBOperations.InsertDataEvaluationKitt(dl);
                return Ok("Se ha registrado correctamente DataEvaluation ---" + dl.DataEvaluations);
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