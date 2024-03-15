using MSSPAPI.Globals;
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
    public class ClienteFacturacionController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Route("api/InsertClienteFacturacion")]
        public IHttpActionResult InsertClienteFact(ClienteFacturacion cf)
        {
            try
            {
                cf.Names = EncDec.Encript(cf.Names);
                cf.LastName = EncDec.Encript(cf.LastName);
                cf.RegimenFiscal = EncDec.Encript(cf.RegimenFiscal);
                cf.Calle = EncDec.Encript(cf.Calle);
                cf.Numero = EncDec.Encript(cf.Numero);
                cf.Colonia = EncDec.Encript(cf.Colonia);
                cf.Telefono = EncDec.Encript(cf.Telefono);
                cf.CP = EncDec.Encript(cf.CP);
                cf.Email = EncDec.Encript(cf.Email);
                cf.Estado = EncDec.Encript(cf.Estado);
                cf.Localidad = EncDec.Encript(cf.Localidad);
                cf.PrecioSIVA = Convert.ToDecimal(Convert.ToDouble(cf.PrecioTotal) / 1.16);
                cf.IVA = Convert.ToDecimal(Convert.ToDouble(cf.PrecioSIVA) * 0.16);
                var emisionServ = DBOperations.InsertClienteFact(cf);
                return Ok(emisionServ);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut]
        [Route("api/UpdateClienteFacturacion/{Id}/{UUID}")]
        public IHttpActionResult UpdateClienteFact(int Id, string UUID)
        {
            try
            {
                //ClienteFacturacion cf = new ClienteFacturacion();
                //cf.Id = Id;
                //ClienteFacturacion emisionServ = DBOperations.UpdateClienteFactUUID(cf);
                return Ok();
            }
            catch (Exception ex)
            {
                //log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.ToString());
            }
        }
    }
}