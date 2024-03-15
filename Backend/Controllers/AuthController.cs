using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web.UI.WebControls;
using System.Linq;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using MSSPAPI.EmisionServiceDev;
using System.Configuration;
using MSSPAPI.Globals;
using Org.BouncyCastle.Crypto.Tls;
using System.ServiceModel;
using System.Web;

namespace MSSPAPI.Controllers
{
    public class AuthController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(Login model)
        {
            if(model == null)
            {
                return BadRequest();
            }

            if(model.UserName == ""  && model.Password == "")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@2410"));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(
                    issuer: "CodeMaze",
                    audience: "",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signingCredentials

                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetToken")]
        public IHttpActionResult GetToken(JwtTokenRequest credentials)
        {
            JwtTokenResponse response = new JwtTokenResponse()
            {
                accessToken = "",
                encryptedAccessToken = "",
                expiresInSeconds = 0,
                success = false,
                error = null
            };

            if (!this.Request.Headers.Contains("TenantId") || !this.Request.Headers.Contains("ApiKey") || string.IsNullOrEmpty(credentials.usernameOrEmailAddresss) || string.IsNullOrEmpty(credentials.password))
            {
                if (!this.Request.Headers.Contains("TenantId"))
                {
                    response.error = new ErrorJwtToken { statusCode = 400, message = "No se encuentra el encabezado TenantId"};
                } 
                else if (!this.Request.Headers.Contains("ApiKey"))
                {
                    response.error = new ErrorJwtToken { statusCode = 400, message = "No se encuentra el encabezado ApiKey" };
                } 
                else if (string.IsNullOrEmpty(credentials.usernameOrEmailAddresss))
                {
                    response.error = new ErrorJwtToken { statusCode = 400, message = "No se ha ingresado un usuario" };
                } 
                else if (string.IsNullOrEmpty(credentials.password))
                {
                    response.error = new ErrorJwtToken { statusCode = 400, message = "No se ha ingresado una contraseña" };
                }

                return Content(System.Net.HttpStatusCode.BadRequest, new { result = response });
            }
            else
            {
                try
                {
                    string TenantId = this.Request.Headers.Where(h => h.Key == "TenantId").FirstOrDefault().Value.ElementAt(0);
                    string ApiKey = this.Request.Headers.Where(h => h.Key == "ApiKey").FirstOrDefault().Value.ElementAt(0);
                    string token = "";

                    List<Cliente> clientes = DBOperations.GetAllClientes();
                    Cliente cliente = clientes.Find(x => x.IdCliente == Convert.ToInt32(TenantId));

                   

                    if (cliente.IdCliente == 0) 
                    {
                        response.error = new ErrorJwtToken { statusCode = 401, message = "No existe un cliente con que coincida con el TenantId ingresado" };
                        return Content(System.Net.HttpStatusCode.BadRequest, response);
                    } 
                    else
                    {
                        ApiKeyKitt apiKeyKitt = new ApiKeyKitt();

                        apiKeyKitt = DBOperations.GetApiKeyKitt(cliente.IdCliente);

                        string EncPwd = EncDec.Encript(credentials.password);

                        if (apiKeyKitt.ApiKey != ApiKey) 
                        {
                            response.error = new ErrorJwtToken { statusCode = 401, message = "El ApiKey ingresado no corresponde con el del cliente" };
                            return Content(System.Net.HttpStatusCode.BadRequest, response);
                        } 
                        else if (apiKeyKitt.Pwd != EncPwd)
                        {
                            response.error = new ErrorJwtToken { statusCode = 401, message = "La contrseña es incorrecta" };
                            return Content(System.Net.HttpStatusCode.BadRequest, response);
                        } 
                        else
                        {
                            token = JwtHelper.GenerateJwtToken(credentials.usernameOrEmailAddresss, credentials.password);

                            response.accessToken = token;
                            response.success = true;
                            response.encryptedAccessToken = EncDec.Encript(token);
                            response.expiresInSeconds = 14400;

                            return Ok(new { result = response });
                        }   
                    }
                }
                catch (Exception ex)
                {
                    response.error = new ErrorJwtToken() { statusCode = 500, message = ex.Message };
                    return Content(System.Net.HttpStatusCode.InternalServerError, response);
                }
            }
        }
    }
}