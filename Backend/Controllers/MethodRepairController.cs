using System;
using System.Web.Http;
using MSSPAPI.Helpers;
using MSSPAPI.Models;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class MethodRepairController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene los detalles del reclamo
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MethodRepair")]
        public IHttpActionResult GetMethodRepair(MethodRepairAdd mr)
        {
            try
            {
                MethodRepair mrr = DBOperations.GetMethorRepair(mr.DealerCode, mr.RiskType, mr.City, mr.StateProvidence, mr.ServiceCenterCode, mr.Marca);
                return Ok(mrr);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}