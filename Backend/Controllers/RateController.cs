using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Controlador que contiene los metodos para el manejo del objeto Rate
    /// </summary>
    public class RateController : ApiController
    {

        /// <summary>
        /// Endpoint pa obtener el listado de rates por producto y programa
        /// </summary>
        /// <param name="IdProduct">Id del producto al que se encuentra asociado el rate</param>
        /// <param name="IdProgram">Id del programa al que se encuentra asociado el producto</param>
        /// <response code="200">Array de objetos tipo Rate</response>
        /// <response code="500">Objeto de excepción</response>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRates")]
        [ResponseType(typeof(List<Rate>))]
        public IHttpActionResult GetRates(int IdProduct, int IdProgram)
        {
            List<Rate> rates = null;

            try
            {
                rates = DBOperations.GetRates(IdProduct, IdProgram);
                return Ok(rates);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Endpoint para insertar un nuevo rate
        /// </summary>
        /// <param name="rate">Objeto de tipo Rate</param>
        /// <response code="200">Objecto con un mensaje y un booleano</response>
        /// <response code="500">Objeto de excepción</response>
        /// <returns></returns>
        [HttpPost]
        [Route("AddRate")]
        public IHttpActionResult AddRate(Rate rate)
        {
            string message = "";
            bool isSuccess = false;

            try
            {
                int IdRate = 0;

                isSuccess = DBOperations.InsertUpdateRate(rate, "C", out IdRate);

                message = (isSuccess) ? "Se ha insertado correctamente el Rate" : "Ocurrio un error al insertar el Rate";

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


            return Ok(new { Message = message, Success = isSuccess });

        }

    }
}
