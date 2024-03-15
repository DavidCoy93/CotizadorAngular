using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class DocumentController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el documento previamente subido
        /// </summary>
        /// <returns></returns>
        // GET: Document
        [Route("api/downloaddocument")]
        public IHttpActionResult GetDocument()
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
                        var policyServiceClient = new CDS.CertificateDocumentServiceClient(Constants.ENPTNAME, Constants.URLDOCUMENT);
                        policyServiceClient.ClientCredentials.UserName.UserName = Constants.USERNAMEA1A2;
                        policyServiceClient.ClientCredentials.UserName.Password = Constants.PSWDA1A2;
                        Cliente cl = DBOperations.GetClientesById(Constants.IdClienteLiverpool); //Constantes cuando haya mas clientes

                        CDS.DownloadCertificateDocumentResponse attachCertificateDocumenthResponse = null;
                        CDS.DownloadCertificateDocumentRequest attachCertificateDocumentRequest = null;
                        CDS.CertificateByCompanyLookup certificate = null;
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        try
                        {
                            certificate = new CDS.CertificateByCompanyLookup
                            {
                                CertificateNumber = "78760852730920"
                                ,
                                CompanyCode = "PRC"
                            };
                            attachCertificateDocumentRequest = new CDS.DownloadCertificateDocumentRequest
                            {
                                CertificateSearch = certificate
                                ,
                                ImageId = Guid.Parse("b412565d-ec4d-4f09-82af-4b0405e0808e")
                            };

                            jsonrequest = JsonConvert.SerializeObject(attachCertificateDocumentRequest);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método Document exitosamente. ", Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            attachCertificateDocumenthResponse = policyServiceClient.DownloadDocument(attachCertificateDocumentRequest);
                            jsonresponse = JsonConvert.SerializeObject(attachCertificateDocumenthResponse);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método Document exitosamente. ", Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                            BadRequest(fault.Message);
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