using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    public class EmisoresFacturaController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("api/GetEmisores")]
        public IHttpActionResult GetEmisores(int ef)
        {
            try
            {
                EmisoresFactura emisionServ = DBOperations.GetEmisoresFactById(ef);
                return Ok(emisionServ);
            }
            catch (Exception ex)
            {

                var emisionServ = new EmisionServiceDev.MessageCode();
                string jsonresponse = JsonConvert.SerializeObject(emisionServ);
                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(jsonresponse);
            }
        }

        [HttpPost]
        [Route("api/InsertEmisores")]
        public IHttpActionResult InsertEmisores(EmisoresFactura ef)
        {
            try
            {
                int emisionServ = DBOperations.InsertSenderFact(ef);
                return Ok(emisionServ);
            }
            catch (Exception ex)
            {

                var emisionServ = new EmisionServiceDev.MessageCode();
                string jsonresponse = JsonConvert.SerializeObject(emisionServ);
                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(jsonresponse);
            }
        }
    }
}