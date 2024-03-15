using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web.UI.WebControls;

namespace MSSPAPI.Controllers
{
    public class DynamicController : ApiController
    {
        [HttpGet]
        [Route("FrequentlyAskedQuestions")]
        public IHttpActionResult FrequentlyAskedQuestions(int IdCliente)
        {
            return Ok();
         
        }
    }
}