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
    public class VehiclesController : ApiKeyController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("api/GetVehicles")]
        public IHttpActionResult GetVehicles()
        {
            try
            {
                List<Vehicles> vehicless = DBOperations.GetAllVehicles();
                return Ok(vehicless);
            }
            catch (Exception ex)
            {

                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/GetVehiclesById")]
        public IHttpActionResult GetVehiclesById(int ef)
        {
            try
            {
                Vehicles vehicless = DBOperations.GetVehiclesById(ef);
                return Ok(vehicless);
            }
            catch (Exception ex)
            {

                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/GetModelById/{Id}")]
        public IHttpActionResult GetModelById(int Id)
        {
            try
            {
                Model vehicless = DBOperations.GetModelsById(Id);
                return Ok(vehicless);
            }
            catch (Exception ex)
            {

                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/GetVehiclesVersionById/{Id}/{IdModel}")]
        public IHttpActionResult GetVehiclesVersionById(int Id, int IdModel)
        {
            try
            {
                VehicleVersion vehicless = DBOperations.GetVehicleVersionById(Id, IdModel);
                return Ok(vehicless);
            }
            catch (Exception ex)
            {

                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/GetVehiclesUsesById/{Id}")]
        public IHttpActionResult GetVehiclesUsesById(int Id)
        {
            try
            {
                VehicleUse vehicless = DBOperations.GetVehicleUseById(Id);
                return Ok(vehicless);
            }
            catch (Exception ex)
            {

                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("api/InsertEVehicles")]
        public IHttpActionResult InsertEVehicles(Vehicles ef)
        {
            try
            {
                int vehicless = DBOperations.InsertVehicles(ef);
                return Ok(vehicless);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("api/DeleteVehicles/{Id}")]
        public IHttpActionResult DeleteVehicles(int Id)
        {
            try
            {
                int vehicless = DBOperations.DeleteVehicles(Id);
                return Ok(vehicless);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }
    }
}