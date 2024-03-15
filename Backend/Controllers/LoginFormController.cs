using MSSPAPI.CDS;
using MSSPAPI.ClaimServiceWS;
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

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class LoginFormController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene los detalles de la póliza
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        [Route("api/login/")]
        public IHttpActionResult GetDetails(LoginForm jsonString)
        {
            var hreq = this.Request.Headers;
            string msspt = hreq.GetValues("tokenmssp").First();
            if (DBOperations.GetToken(msspt))
            {
        

                try
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    var policyServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                    policyServiceClient.ClientCredentials.UserName.UserName = Constants.USERNAMEA1A2;
                    policyServiceClient.ClientCredentials.UserName.Password = Constants.PSWDA1A2;
                    string jsonresponse = string.Empty;
                    string jsonConfig = string.Empty;
                    string jsonrequest = string.Empty;
                    Cliente cl = DBOperations.GetClientesById(jsonString.IdCliente); //Constantes cuando haya mas clientes
                    jsonConfig = cl.Configuraciones;
                    dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                    try
                    {
                        var policylookup = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                        {
                            DealerCode = jsonObj["DealerCode"],
                            IdentificationNumber = jsonString.CertificateNumber
                        };

                        var searchRequest = new PolicyServiceWS.SearchRequest
                        {
                            PolicyLookup = policylookup
                        };

                        jsonrequest = JsonConvert.SerializeObject(searchRequest);
                        Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método Details exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                        DBOperations.InsertBitacora(btreq);
                        var response = policyServiceClient.Search(searchRequest);
                        jsonresponse = JsonConvert.SerializeObject(response);
                        log4net.ThreadContext.Properties["Response"] = jsonresponse;
                        log4net.ThreadContext.Properties["Request"] = jsonrequest;
                        log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                        Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método Details exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                        DBOperations.InsertBitacora(btresp);
                    }
                    catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                    {
                        log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    }
                    catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                    {
                        log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                        return BadRequest();
                    }
                    catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                    {
                        log.Error(string.Format("DealerNotFoundFault- {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    }
                    catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                    {
                        log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    }

                    return Ok(jsonresponse);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. PolicyDetails", Plataforma = "MSSP_ELITA" };
                DBOperations.InsertBitacora(btresp);
                return BadRequest("{'Error':'Token Invalido'}");
            }
        }
    }
}