using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using MSSPAPI.ClaimRecordingServiceWS;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class SaveMethodOfRepairedController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [HttpPost]
        [Route("api/InsertDataMethodofRepaired")]
        public IHttpActionResult InsertData(dynamic MethodRepairList)
        {
            try
            {

                var data2 = JsonConvert.SerializeObject(MethodRepairList.data);
               
                 DataTable dt = JsonConvert.DeserializeObject<DataTable>(data2);

                var hreq = this.Request.Headers;
                string msspt_idcliente = "5";//hreq.GetValues("idcliente").First();

                var data = DBOperations.InsertMethodOfRepaired(dt, msspt_idcliente);

                
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("api/InsertDataTextDinamic")]
        public IHttpActionResult InsertDataTextDinamic(dynamic TextDinamic)
        {
            try
            {

                var data2 = JsonConvert.SerializeObject(TextDinamic.data);

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(data2);

                var hreq = this.Request.Headers;
                string msspt_idcliente = "5";//hreq.GetValues("idcliente").First();

                var data = DBOperations.InsertTextDinamic(dt, msspt_idcliente);


                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


    }
}