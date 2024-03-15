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
using System.Xml.Linq;

namespace MSSPAPI.Controllers
{
    public class GenerarTokenController : ApiController
    {
        [HttpPost]
        //[Authorize]
        [Route("GenerateTokenKitt")]
        public IHttpActionResult GenerateTokenKitt(int cliente)
        {
            try
            {
                GenerarToken tk = new GenerarToken();
                string cv = tk.GeneraMSSPClave(cliente);
                string tkn = tk.BuildToken(cv);
                return Ok(tkn);
            }
            catch (TimeoutException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}