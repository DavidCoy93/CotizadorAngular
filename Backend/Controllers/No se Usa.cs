using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class ElitaController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene la poliza mediante el numero de certificado
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        // GET api/elita/1-IQTAUX6
        [Route("api/searchClaimByCertificateNumber/{certificate}/{id}")]
        public IHttpActionResult GetPoliza(string certificate, string dealercode, string id)
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    string jsonresponse = string.Empty;
                    string jsonrequest = string.Empty;
                    string jsonConfig = string.Empty;
                    try
                    {
                        var claimServiceClient = new ClaimServiceWS.ClaimServiceClient(Constants.ENPTNAMECLAIM, Constants.URLCLAIM3);
                        Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(id)); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);

                        claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Password"];

                        try
                        {
                            var claimlookup = new ClaimServiceWS.SearchClaimByCertificateNumber
                            {
                                CertificateNumber = certificate,
                                DealerCode = jsonObj["DealerCode"],
                              
                            };
                            var searchRequest = new ClaimServiceWS.ClaimSearchRequest
                            {
                                CultureCode = jsonObj["CultureCode"],
                                ClaimLookup = claimlookup
                            };
                            jsonrequest = JsonConvert.SerializeObject(searchRequest);
                            var response = claimServiceClient.Search(searchRequest);
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
                            return BadRequest(fault.Message);
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                            return BadRequest(fault.Message);
                        }
                        catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                        {
                            log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                            return BadRequest(fault.Message);
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                            return BadRequest(fault.Message);
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return BadRequest(ex.Message);
                        }
                        return Ok(jsonresponse);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", "Error al obtener token"), null);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. Document", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                return BadRequest("Ocurrió un error, por favor póngase en contacto con el administrador");
            }
        }
    }
}