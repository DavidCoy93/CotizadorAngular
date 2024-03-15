using System;
using System.Collections.Generic;
using System.Web.Http;
using MSSPAPI.Helpers;
using MSSPAPI.Models;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class TrackingCodeController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      

        [HttpGet]
        [Route("api/GetTracking/{Id}")]
        public IHttpActionResult GetTrackingCode(int Id)
        {
            try
            {
                List<TrackingCode> trackingCodes = DBOperations.GetTrackingCode(Id);
                return Ok(trackingCodes);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

       
    }
}