using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace MSSPAPI.Controllers
{
    ///
    public class ApiKeyController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/ApiKey/1-IQTAUX6
        /// <summary>
        /// Solicita el token, Dealer Code, Authorization y Risk Type Code y lo devuelve al portal
        /// </summary>
        /// <param name="dealercode"></param>
        /// <returns></returns>
        [Route("api/apikey/{dealercode}")]
        public string GetApikey(string dealercode)
        {
            string js = null;
            try
            {
                if (OktaHelper.OktaValidation())
                {
                    Cliente cl = DBOperations.GetApikey(dealercode);
                    var jsRsp = new Apikey();
                    string token = DBOperations.GetTokenByCliente(cl.IdCliente);
                    jsRsp.IdCliente = cl.IdCliente;
                    jsRsp.NombreCliente = cl.NombreCliente;
                    jsRsp.ApiKey = cl.Apikey;
                    jsRsp.Authorization = cl.Authorization;
                    jsRsp.RiskTypeCode = cl.RiskTypeCode;
                    jsRsp.URLHeader = cl.URLHeader;
                    jsRsp.URLFooter = cl.URLFooter;
                    jsRsp.Tokens = token;

                    FlujoAlterno Fa = DBOperations.GetFlujoAlterno(cl.IdCliente);


                    jsRsp.Multiple = Fa.EnabledMultiple;


                    js = JsonConvert.SerializeObject(jsRsp);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método ApikeyToken exitosamente. " + js, Plataforma = "PIF" };
                    DBOperations.InsertBitacora(btresp);
                    log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{js},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                    return js;
                }
                else
                {
                    log.Error("Error al validad OKTA en API Key Controller");
                    return "Error al consultar api key";
                }
            }
            catch (Exception ex)
            {
                log.Error("Error al consultar api key" + ex.Message);
                return ex.Message;
            }
        }
        [HttpGet]
        [Route("api/GetApikeyKitt/{id}")]
        public IHttpActionResult GetApikeyKitt(int id)
        {
            string js = null;
            try
            {
                ApiKeyKitt kt = new ApiKeyKitt();
                kt = DBOperations.GetApiKeyKitt(id);
                return Ok(kt);
            }
            catch (TimeoutException ex)
            {
                log.Error("Error al consultar api key" + ex.Message);
                return BadRequest(); ;
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}