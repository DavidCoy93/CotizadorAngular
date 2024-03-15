using Microsoft.Ajax.Utilities;
using MSSPAPI.CDS;
using MSSPAPI.ClaimServiceWS;
using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace MSSPAPI.Controllers
{

    public class LoginAccessController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        // GET api/login/
        [HttpPost]
        [Route("api/login/")]
        public IHttpActionResult GetLogin(LoginAccess jsonString)
        {
            try
            {
                //var hreq = this.Request.Headers;
                //string msspt = hreq.GetValues("tokenmssp").First();
                if (true)
                {
                    try
                    {
                        log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                        log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                        string jsonresponse = string.Empty;
                        string jsonConfig = string.Empty;
                        string jsonrequest = string.Empty;

                        try
                        {
                            var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                            var policyServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                            Cliente cl = DBOperations.GetClientesById(jsonString.IdCliente); //Constantes cuando haya mas clientes
                            jsonConfig = cl.Configuraciones;
                            dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                         
                            policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                            policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Password"];

                            try
                            {

                                var policylookup = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                                {
                                    CompanyCode = jsonObj["CompanyCode"],
                                    IdentificationNumber = jsonString.CertificateNumber.Replace("." , "").Replace("-","")
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
                                try
                                {

                                    List<DealerCodes> listDealerCode = DBOperations.GetDealerCodesByIdCliente(cl.IdCliente);

                                    foreach (var row in listDealerCode)
                                    {
                                        try
                                        {
                                            string companyCode = jsonObj["CompanyCode"];
                                            jsonresponse = SearhCertificate(row.DealerCode, jsonString.CertificateNumber, companyCode, jsonString.IdCliente);
                                            if (jsonresponse != "")
                                            {
                                                return Ok(jsonresponse);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                                            if (Fa.Enabled == true)
                                            {
                                                //log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                                //ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                                                //if (objFolio.Folio != null)
                                                //{
                                                    //Se valida si hay un Folio en proceso
                                                   //if (objFolio.Nombre != "")
                                                    //    DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                                   // else
                                                        //toma el ultimo folio y actualiza poliza
                                                     //   DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                                               // }
                                                //Se inserta primer registro en la tabla ClaimFolio
                                               // else
                                                 //   DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                               // return Ok(GenerateJsonResponseSimulation(certificate));
                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                    return BadRequest(ex.Message);
                                }
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
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", "Error al obtener token"), null);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = DateTime.Now, Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. ClaimByCertificateNumber", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                return BadRequest();
            }
        }

        public string SearhCertificate(string dealerCode, string certificateNumber, string conpanyCode, int idCliente)
        {
            string jsonConfig = string.Empty;
            var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
            Cliente cl = DBOperations.GetClientesById(idCliente); //Constantes cuando haya mas clientes
            jsonConfig = cl.Configuraciones;
            dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
            claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
            claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Password"];
            string jsonresponse = string.Empty;
            string jsonrequest = string.Empty;
            try
            {

                var claimlookup = new PolicyServiceWS.SearchPolicyByCertificateNumber
                {
                    CertificateNumber = certificateNumber,
                    CompanyCode = conpanyCode,
                    DealerCode = dealerCode
                };

                var searchRequest = new PolicyServiceWS.SearchRequest
                {
                    PolicyLookup = claimlookup
                };

                var response = claimServiceClient.Search(searchRequest);
                jsonresponse = JsonConvert.SerializeObject(response);
                log4net.ThreadContext.Properties["Response"] = jsonresponse;
                log4net.ThreadContext.Properties["Request"] = jsonrequest;

                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método SearchClaimByMembershipNumber exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                DBOperations.InsertBitacora(btresp);

                string IdentificationNumber = response.PolicyResponse[0].IdentificationNumber;

                var policylookup = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                {
                    CompanyCode = jsonObj["CompanyCode"],
                    IdentificationNumber = IdentificationNumber
                };

                searchRequest = new PolicyServiceWS.SearchRequest
                {
                    PolicyLookup = policylookup
                };

                response = claimServiceClient.Search(searchRequest);
                jsonresponse = JsonConvert.SerializeObject(response);
                return jsonresponse;


            }
            catch (FaultException<PolicyServiceWS.EnrollFault> fault)
            {
                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                return "";
            }

        }

    }
}