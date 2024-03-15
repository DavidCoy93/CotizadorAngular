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
using System.Data;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class ClaimByCertificateNumberController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("api/GetPolicy/{poliza}/{certificate}/{id}/{dealercode}")]
        public IHttpActionResult GetPoliza(string poliza, string certificate, string id, string dealercode)
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
                        var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                        Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(id)); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];
                        List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(Convert.ToInt32(id));
                        List<DealerCodes> dco = DBOperations.GetDealerCodesByDealerCode(dealercode);

                        try
                        {
                            var claimlookup = new PolicyServiceWS.SearchPolicyByMembershipNumber
                            {
                                MembershipNumber = poliza,
                                CompanyCode = dco.First().CompanyCode,
                                DealerCode = dealercode
                            };
                            var searchRequest = new PolicyServiceWS.SearchRequest
                            {
                                PolicyLookup = claimlookup
                            };

                            // Extraigo el metodo el certificado
                            jsonrequest = JsonConvert.SerializeObject(searchRequest);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método SearchClaimByMembershipNumber exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            var response = claimServiceClient.Search(searchRequest);
                            jsonresponse = JsonConvert.SerializeObject(response);

                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método SearchClaimByMembershipNumber exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {

                            List<DealerCodes> listDealerCode = DBOperations.GetDealerCodesByDealerCode(dealercode);


                            try
                            {

                                SearchData search = new SearchData();
                                search.CompanyCode = dco.First().CompanyCode;
                                search.DealerCode = dealercode;
                                search.CertificateNumber = certificate;
                                search.idCliente = Convert.ToInt32(id);
                                jsonresponse = SearhCertificate(search);
                                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                                if (jsonresponse != "")
                                {
                                    return Ok(jsonresponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                                if (Fa.Enabled == true)
                                {
                                    log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                    ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                                    if (objFolio.Folio != null)
                                    {
                                        //Se valida si hay un Folio en proceso
                                        if (objFolio.Nombre != "")
                                            DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                        else
                                            //toma el ultimo folio y actualiza poliza
                                            DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                                    }
                                    //Se inserta primer registro en la tabla ClaimFolio
                                    else
                                        DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                    return Ok(GenerateJsonResponseSimulation(certificate));
                                }

                            }

                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                                if (objFolio.Folio != null)
                                {
                                    //Se valida si hay un Folio en proceso
                                    if (objFolio.Nombre != "")
                                        DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                    else
                                        //toma el ultimo folio y actualiza poliza
                                        DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                                }
                                //Se inserta primer registro en la tabla ClaimFolio
                                else
                                    DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                                if (objFolio.Folio != null)
                                {
                                    //Se valida si hay un Folio en proceso
                                    if (objFolio.Nombre != "")
                                        DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                    else
                                        //toma el ultimo folio y actualiza poliza
                                        DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                                }
                                //Se inserta primer registro en la tabla ClaimFolio
                                else
                                    DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                                if (objFolio.Folio != null)
                                {
                                    //Se valida si hay un Folio en proceso
                                    if (objFolio.Nombre != "")
                                        DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                    else
                                        //toma el ultimo folio y actualiza poliza
                                        DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                                }
                                //Se inserta primer registro en la tabla ClaimFolio
                                else
                                    DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (Exception ex)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                                if (objFolio.Folio != null)
                                {
                                    //Se valida si hay un Folio en proceso
                                    if (objFolio.Nombre != "")
                                        DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                    else
                                        //toma el ultimo folio y actualiza poliza
                                        DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                                }
                                //Se inserta primer registro en la tabla ClaimFolio
                                else
                                    DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {

                                List<DealerCodes> listDealerCode = DBOperations.GetDealerCodesByDealerCode(dealercode);

                                SearchData search = new SearchData();
                                search.CompanyCode = dco.First().CompanyCode;
                                search.DealerCode = dealercode;
                                search.CertificateNumber = certificate;
                                search.idCliente = Convert.ToInt32(id);
                                jsonresponse = SearhCertificate(search);
                                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                                if (jsonresponse != "")
                                {
                                    return Ok(jsonresponse);
                                }


                            }
                        }
                        return Ok(jsonresponse);
                    }
                    catch (Exception ex)
                    {
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                            if (objFolio.Folio != null)
                            {
                                //Se valida si hay un Folio en proceso
                                if (objFolio.Nombre != "")
                                    DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                                else
                                    //toma el ultimo folio y actualiza poliza
                                    DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                            }
                            //Se inserta primer registro en la tabla ClaimFolio
                            else
                                DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                            return Ok(GenerateJsonResponseSimulation(certificate));
                        }
                        else
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return BadRequest();
                        }
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
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                if (Fa.Enabled == true)
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    ClaimFolio objFolio = DBOperations.GetLastClaimFolio();
                    if (objFolio.Folio != null)
                    {
                        //Se valida si hay un Folio en proceso
                        if (objFolio.Nombre != "")
                            DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                        else
                            //toma el ultimo folio y actualiza poliza
                            DBOperations.UpdateClaimFolioFromBeginClaim(poliza, certificate);
                    }
                    //Se inserta primer registro en la tabla ClaimFolio
                    else
                        DBOperations.InsertClaimFolio(ClaimFolioHelper.FolioGeneration(), "", "", "", "", "", "", "", "", "", poliza, "", "", certificate);
                    return Ok(GenerateJsonResponseSimulation(certificate));
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        ///
        public string GenerateJsonResponseSimulation(string certificado)
        {
            string data = "{\"ErrorCode\":\"0\",\"ErrorMessage\":\"null\",\"PolicyResponse\":[{\"CertificateNumber\":\"" + certificado + "\",\"CustomerName\":\"GOMEZ ZAMARRIPA SONIA KARINA\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"HomePhone\":\"08100943\",\"IdentificationNumber\":\"GOZS7003096M2\",\"ImeiNumber\":\"\",\"ItemDescription\":\"04 PIF PLUS\",\"Carrier\":null,\"ItemEffectiveDate\":\"2020-03-01T00:00:00\",\"Manufacturer\":\"\",\"Model\":\"\",\"ProductSalesDate\":\"2020-03-01T00:00:00\",\"ProductDescription\":\"Liverpool PIF\",\"ProductCode\":\"LVPIF\",\"SerialNumber\":\"\",\"ServiceLineNumber\":\"\",\"SKUNumber\":\"\",\"Statuscode\":\"A\",\"WarrantySalesDate\":\"2020-03-01T00:00:00\",\"WorkPhone\":\"\",\"MasterCertificateNumber\":\"\",\"PromoCodes\":\"\",\"ActivationDate\":\"2020-03-01T00:00:00\"},{\"CertificateNumber\":\"1\",\"CustomerName\":\"RAMIREZ MILLAN PERLA\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"HomePhone\":\"5554710406\",\"IdentificationNumber\":\"RAMP760413LU2\",\"ImeiNumber\":\"\",\"ItemDescription\":\"PS4 DS4 Midnight Blue\",\"Carrier\":null,\"ItemEffectiveDate\":\"2020-03-01T00:00:00\",\"Manufacturer\":\"MICROSOFT\",\"Model\":\"\",\"ProductSalesDate\":\"2020-03-01T00:00:00\",\"ProductDescription\":\"Liverpool PIF\",\"ProductCode\":\"LVPIF\",\"SerialNumber\":\"\",\"ServiceLineNumber\":\"\",\"SKUNumber\":\"\",\"Statuscode\":\"A\",\"WarrantySalesDate\":\"2020-03-01T00:00:00\",\"WorkPhone\":\"\",\"MasterCertificateNumber\":\"\",\"PromoCodes\":\"\",\"ActivationDate\":\"2020-03-01T00:00:00\"}]}";
            return data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        ///
        [HttpPost]
        public string SearhCertificate(SearchData SeachData)
        {
            string jsonConfig = string.Empty;
            var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
            Cliente cl = DBOperations.GetClientesById(SeachData.idCliente); //Constantes cuando haya mas clientes
            jsonConfig = cl.Configuraciones;
            dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
            claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
            claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];
            string jsonresponse = string.Empty;
            string jsonrequest = string.Empty;
            try
            {

                var claimlookup = new PolicyServiceWS.SearchPolicyByCertificateNumber
                {
                    CertificateNumber = SeachData.CertificateNumber,
                    CompanyCode = SeachData.CompanyCode,
                    DealerCode = SeachData.DealerCode

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
                if (IdentificationNumber != "NULL" & SeachData.DealerCode != "HCMV")
                {
                    var policylookup = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                    {
                        CompanyCode = SeachData.CompanyCode,
                        IdentificationNumber = IdentificationNumber
                    };

                    searchRequest = new PolicyServiceWS.SearchRequest
                    {
                        PolicyLookup = policylookup
                    };

                    response = claimServiceClient.Search(searchRequest);
                    jsonresponse = JsonConvert.SerializeObject(response);
                }
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