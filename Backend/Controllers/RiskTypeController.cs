using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using MSSPAPI.Helpers;
using MSSPAPI.Models;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class RiskTypeController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="risk"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetRiskTypeByClienteRisk/")]
        public IHttpActionResult GetRiskTypeByClienteRisk(int id, string risk)
        {
            try
            {
                List<RiskTypeCode> rt = DBOperations.GetRiskTypeByClienteRisk(id, risk);
                return Ok(rt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertRiskType")]
        public IHttpActionResult InsertRiskType(RiskTypeCode dl)
        {
            try
            {
                RiskTypeCode rt = new RiskTypeCode();
                rt.IdCliente = dl.IdCliente;
                rt.RiskType = dl.RiskType;
                rt.Marca = dl.Marca;
                DBOperations.InsertRiskType(rt);
                return Ok(dl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateRiskType")]
        public IHttpActionResult UpdateRiskType(RiskTypeCode dl)
        {
            try
            {
                RiskTypeCode rt = new RiskTypeCode();
                rt.IdRiskTypeCode = dl.IdRiskTypeCode;
                rt.IdCliente = dl.IdCliente;
                rt.RiskType = dl.RiskType;
                rt.Marca = dl.Marca;
                DBOperations.UpdateRiskTypeCode(rt);
                return Ok(dl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}