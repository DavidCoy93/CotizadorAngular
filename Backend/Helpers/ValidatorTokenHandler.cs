using Microsoft.Ajax.Utilities;
using Microsoft.IdentityModel.Tokens;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MSSPAPI.Helpers
{
    internal class ValidatorTokenHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;
            string ApiKey;

            if (!VerifyAuthorizationHeader(request, out token, out ApiKey))
            {
                statusCode = HttpStatusCode.Unauthorized;
                request.CreateResponse(statusCode, new {Message = "Por favor verificar los encabezados en la petición"});
                return base.SendAsync(request, cancellationToken);
            }


            try
            {
                string issuer = "https://www.mssptest.com";
                string audience = "https://www.mssptest.com/api";

                SecurityToken securityToken;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                JwtSecurityToken jwtSecurity = tokenHandler.ReadJwtToken(token);


                Claim userNameClaim = jwtSecurity.Claims.Where(c => c.Type == "UserName").FirstOrDefault();

                if (userNameClaim.Value != "")
                {
                    List<ApiKeyKitt> apiKeys = DBOperations.GetAllApiKeys();
                    ApiKeyKitt keyKitt = apiKeys.Where(ak => ak.Usernamemail == userNameClaim.Value).FirstOrDefault();


                    if (keyKitt.ApiKey != "")
                    {

                        if (keyKitt.ApiKey == ApiKey)
                        {
                            byte[] ks = Encoding.UTF8.GetBytes(keyKitt.Pwd);

                            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(ks);

                            TokenValidationParameters validationParameters = new TokenValidationParameters()
                            {
                                ValidIssuer = issuer,
                                ValidAudience = audience,
                                ValidateIssuerSigningKey = true,
                                ValidateLifetime = true,
                                LifetimeValidator = IsTokenExpired,
                                IssuerSigningKey = securityKey,
                            };

                            Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                            HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                            return base.SendAsync(request, cancellationToken);
                        } 
                        else
                        {
                            request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "El ApiKey que envio no es igual al del cliente que esta consultando" });
                            return base.SendAsync(request, cancellationToken);
                        }
                    } else {
                        request.CreateResponse(HttpStatusCode.BadRequest, new { Message  = "No existe un Api key para el usuario enviado" });
                        return base.SendAsync(request, cancellationToken);
                    }
                } 
                else
                {
                    request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "No existe ningún usuario en el payload del token" });
                    return base.SendAsync(request, cancellationToken);
                }
            }
            catch (SecurityTokenValidationException ex)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex) 
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));
        }



        public bool VerifyAuthorizationHeader(HttpRequestMessage request, out string token, out string apiKey)
        {
            token = null;
            apiKey = null;

            KeyValuePair<string, IEnumerable<string>> AuthorizationHeader = request.Headers.Where(h => h.Key == "Authorization").FirstOrDefault();
            KeyValuePair<string, IEnumerable<string>> ApiKeyHeader = request.Headers.Where(h => h.Key.ToLower() == "apikey").FirstOrDefault();


            if (AuthorizationHeader.Key == null || ApiKeyHeader.Key == null) 
            { 
                return false; 
            }
            
            string bearerToken = AuthorizationHeader.Value.First();
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            apiKey = ApiKeyHeader.Value.First();

            return true;
        }

        public bool IsTokenExpired(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) 
        {
            bool valid = false;

            if ((notBefore.HasValue && DateTime.UtcNow > notBefore) && (expires.HasValue && DateTime.UtcNow < expires))
            {
                return true;
            }

            return valid;
        }
    }
}