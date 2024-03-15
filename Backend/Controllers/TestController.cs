using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class TestController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene los detalles del reclamo
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/test")]
        public IHttpActionResult testclaim()
        {
            try
            {
                DBOperations.InsertClaimData("98F0E491-8892-11E8-A78E-000000000000-417", 1, "{jsons}", "{jsons}", true, "EA13");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("api/testUpd")]
        public IHttpActionResult testclaimUpd()
        {
            try
            {
                DBOperations.UpdateClaimData(1, "98F0E491-8892-11E8-A78E-000000000000-417", 1, "{jsons}", "{jsons}", true, "EA13");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}