using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class TransaccionController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numerotransaccion"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/transaccion/")]
        public IHttpActionResult Transaccion(dynamic json)
        {
            try
            { 
                var hreq = this.Request.Headers;
              
                string numerotransaccion = (hreq.GetValues("numerotransaccion").First());
                if (numerotransaccion != "") 
                {
                    log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{json},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                    bool result = DBOperations.InsertTransaction(numerotransaccion, JsonConvert.SerializeObject(json.json));
                    return Ok("Registrado correctamente");
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", "Error al obtener token"), null);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = DateTime.Now, Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. TransaccionController", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Exception TransaccionController- {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{json},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                return BadRequest();
            }
        }
    }
}