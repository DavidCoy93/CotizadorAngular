using System;
using System.Web.Http;
using MSSPAPI.Helpers;
using MSSPAPI.Models;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class LoginAdminController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/LoginAdmin")]
        public IHttpActionResult LoginAdmin(dynamic user)
        {
            try
            {
                if (user.Usr == "" || user.Usr == null)
                {
                    return BadRequest("El usuario no debe ir vacio");
                }
                if (user.Pd == "" || user.Pd == null)
                {
                    return BadRequest("Debe escribir la contraseña");
                }
                if (DBOperations.GetLogin(user.Usr, user.Pd))
                {
                    return Ok(user);
                }
                else return BadRequest("No ha sido posible hacer Login");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}