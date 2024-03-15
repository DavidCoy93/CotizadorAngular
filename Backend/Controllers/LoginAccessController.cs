using Microsoft.Ajax.Utilities;
using MSSPAPI.CDS;
using MSSPAPI.ClaimServiceWS;
using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using MSSPAPI.PolicyServiceWS;
using Newtonsoft.Json;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Configuration;
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
        public IHttpActionResult GetLogin(dynamic jsonString)
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
                            List<string> listDetails = new List<string>();
                            var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                            var policyServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                            Cliente cl = DBOperations.GetClientesById(jsonString.IdCliente); //Constantes cuando haya mas clientes
                            jsonConfig = cl.Configuraciones;
                            dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);

                            policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                            policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];

                            try
                            {
                                List<DealerCodes> listDealerCode = DBOperations.GetDealerCodesByIdCliente(cl.IdCliente);
                             
                               
                                foreach (var row in listDealerCode)
                                {
                                    try
                                    {
                                        switch (jsonString.Tipo)
                                        {
                                           
                                            case 1:
                                                var claimlookupSearchPolicyByCertificateNumber = new PolicyServiceWS.SearchPolicyByCertificateNumber
                                                {
                                                    CertificateNumber = jsonString.CertificateNumber,
                                                    CompanyCode = row.CompanyCode,
                                                    DealerCode = row.DealerCode
                                                };

                                                var searchRequestSearchPolicyByCertificateNumber = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupSearchPolicyByCertificateNumber
                                                };


                                                var respons = policyServiceClient.Search(searchRequestSearchPolicyByCertificateNumber);


                                                string IdentificationNumber = respons.PolicyResponse[0].IdentificationNumber;

                                                if (IdentificationNumber != "NULL")
                                                {
                                                    var policylookup = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                                                    {
                                                        CompanyCode = row.CompanyCode,
                                                        IdentificationNumber = IdentificationNumber
                                                    };

                                                    var searchreq = new PolicyServiceWS.SearchRequest
                                                    {
                                                        PolicyLookup = policylookup
                                                    };

                                                    var responseSearchPolicyByIdentificationNumber = policyServiceClient.Search(searchreq);
                                                    jsonresponse = JsonConvert.SerializeObject(responseSearchPolicyByIdentificationNumber);
                                                }

                                                jsonresponse = JsonConvert.SerializeObject(respons);
                                                break;

                                            case 2:
                                                var claimlookupBirth = new PolicyServiceWS.SearchPolicyByCertificateNumberDateofBirth
                                                {
                                                    BirthDate = jsonString.BirthDate,
                                                    CertificateNumber = jsonString.CertificateNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestBirth = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupBirth
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestBirth);
                                                break;
                                            case 3:
                                                var claimlookupDealergoup = new PolicyServiceWS.SearchPolicyByDealerGroup
                                                {
                                                    CertificateNumber = jsonString.CertificateNumber,
                                                    CompanyGroup = jsonString.CompanyGroup,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    Email = jsonString.Email,
                                                    IdentificationNumber = jsonString.IdentificationNumber,
                                                    InvoiceNumber = jsonString.InvoiceNumber,
                                                    PhoneNumber = jsonString.PhoneNumber,
                                                    PostalCode = jsonString.PostalCode,
                                                    SerialNumber = jsonString.SerialNumber,
                                                    ServiceLineNumber = jsonString.ServiceLineNumber,
                                                };
                                                var searchRequestDealergroup = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupDealergoup
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestDealergroup);
                                                break;
                                            case 4:
                                                var claimlookupEmail = new PolicyServiceWS.SearchPolicyByEmail
                                                {
                                                    Email = jsonString.MembershipNumber,
                                                    DealerGroup = jsonString.CompanyCode,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestEmail = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupEmail
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestEmail);
                                                break;
                                            case 5:
                                                var claimlookupPostalC = new PolicyServiceWS.SearchPolicyByEmailPostalCode
                                                {
                                                    DealerGroup = jsonString.DealerGroup,
                                                    CompanyCode = jsonString.CompanyCode,
                                                    DealerCode = jsonString.DealerCode,
                                                    Email = jsonString.Email,
                                                    PostalCode = jsonString.PostalCode,
                                                };
                                                var searchRequestPostalC = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupPostalC
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestPostalC);
                                                break;

                                            case 6:
                                                var SearchPolicyByIdentificationNumber = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                                                {
                                                    CompanyCode = row.CompanyCode,
                                                    IdentificationNumber = jsonString.CertificateNumber.Replace(".", "").Replace("-", "")
                                                };
                                                if (jsonString.IdCliente == 5)
                                                {
                                                    SearchPolicyByIdentificationNumber = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                                                    {
                                                        CompanyCode = row.CompanyCode,
                                                        IdentificationNumber = jsonString.CertificateNumber.Replace(".", "").Replace("-", "")
                                                    };
                                                }
                                                else
                                                {
                                                    SearchPolicyByIdentificationNumber = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                                                    {
                                                        CompanyCode = row.CompanyCode,
                                                        IdentificationNumber = jsonString.CertificateNumber
                                                    };
                                                }

                                                var searchRequest = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = SearchPolicyByIdentificationNumber
                                                };

                                                var response = policyServiceClient.Search(searchRequest);
                                                jsonresponse = JsonConvert.SerializeObject(response);
                                                jsonrequest = JsonConvert.SerializeObject(searchRequest);
                                                break;
                                            case 7:
                                                var claimlookupINPN = new PolicyServiceWS.SearchPolicyByIdentificationNumberPhoneNumber
                                                {
                                                    IdentificationNumber = jsonString.IdentificationNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,
                                                    PhoneNumber = jsonString.PhoneNumber
                                                };
                                                var searchRequestINPN = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupINPN
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestINPN);
                                                break;
                                            case 8:
                                                var claimlookupImei = new PolicyServiceWS.SearchPolicyByImeiNumber
                                                {
                                                    ImeiNumber = jsonString.ImeiNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestImei = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupImei
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestImei);
                                                break;
                                            case 9:
                                                var claimlookupImeiPN = new PolicyServiceWS.SearchPolicyByImeiNumberPhoneNumber
                                                {
                                                    ImeiNumber = jsonString.ImeiNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,
                                                    PhoneNumber = jsonString.PhoneNumber
                                                };
                                                var searchRequestImeiPN = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupImeiPN
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestImeiPN);
                                                break;
                                            case 10:
                                                var claimlookupInvoice = new PolicyServiceWS.SearchPolicyByInvoiceNumber
                                                {
                                                    InvoiceNumber = jsonString.InvoiceNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestInvoice = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupInvoice
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestInvoice);
                                                break;
                                            case 11:
                                                var claimlookupLM = new PolicyServiceWS.SearchPolicyByInvoiceNumberLastName
                                                {
                                                    LastName = jsonString.LastName,
                                                    InvoiceNumber = jsonString.InvoiceNumber,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestLM = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupLM
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestLM);
                                                break;
                                            case 12:
                                                var claimlookupPC = new PolicyServiceWS.SearchPolicyByInvoiceNumberPostalCode
                                                {
                                                    InvoiceNumber = jsonString.InvoiceNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,
                                                    PostalCode = jsonString.PostalCode
                                                };
                                                var searchRequestPC = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupPC
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestPC);
                                                break;
                                            case 13:
                                                var claimlookupLMD = new PolicyServiceWS.SearchPolicyByLastNameDateofBirth
                                                {
                                                    BirthDate = jsonString.BirthDate,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,
                                                    LastName = jsonString.LastName
                                                };
                                                var searchRequestLMD = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupLMD
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestLMD);
                                                break;
                                            case 14:
                                                var claimlookupMM = new PolicyServiceWS.SearchPolicyByMembershipNumber
                                                {
                                                    BirthDate = jsonString.BirthDate,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,
                                                    HomePhoneNumber = jsonString.HomePhoneNumber,
                                                    MembershipNumber = jsonString.MembershipNumber,
                                                    PhoneNumber = jsonString.PhoneNumber
                                                };
                                                var searchRequestMM = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupMM
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestMM);
                                                break;
                                            case 15:
                                                var claimlookupB = new PolicyServiceWS.SearchPolicyByNameDateofBirthBank
                                                {
                                                    AccountNumber = jsonString.AccountNumber,
                                                    BankSortCode = jsonString.BankSortCode,
                                                    BirthDate = jsonString.BirthDate,
                                                    DealerCode = jsonString.DealerCode,
                                                    FirstName = jsonString.FirstName,
                                                    LastName = jsonString.LastName
                                                };
                                                var searchRequestB = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupB
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestB);
                                                break;
                                            case 16:
                                                var claimlookupPNR = new PolicyServiceWS.SearchPolicyByPhoneNumber
                                                {
                                                    PhoneNumber = jsonString.PhoneNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestPNR = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupPNR
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestPNR);
                                                break;
                                            case 17:
                                                var claimlookupPNPC = new PolicyServiceWS.SearchPolicyByPhoneNumberPostalCode
                                                {
                                                    PostalCode = jsonString.PostalCode,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,
                                                    PhoneNumber = jsonString.PhoneNumber
                                                };
                                                var searchRequestPNPC = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupPNPC
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestPNPC);
                                                break;
                                            case 18:
                                                var claimlookupSN = new PolicyServiceWS.SearchPolicyBySerialNumber
                                                {
                                                    SerialNumber = jsonString.SerialNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestSN = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupSN
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestSN);
                                                break;
                                            case 19:
                                                var claimlookupSLN = new PolicyServiceWS.SearchPolicyByServiceLineNumber
                                                {
                                                    ServiceLineNumber = jsonString.ServiceLineNumber,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode
                                                };
                                                var searchRequestSLN = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupSLN
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestSLN);
                                                break;
                                            case 20:
                                                var claimlookupDB = new PolicyServiceWS.SearchPolicyByCertificateNumberDateofBirth
                                                {
                                                    BirthDate = jsonString.BirthDate,
                                                    DealerGroup = jsonString.DealerGroup,
                                                    DealerCode = jsonString.DealerCode,

                                                };
                                                var searchRequestDB = new PolicyServiceWS.SearchRequest
                                                {
                                                    PolicyLookup = claimlookupDB
                                                };
                                                jsonrequest = JsonConvert.SerializeObject(searchRequestDB);
                                                break;


                                        }
                                        listDetails.Add(jsonresponse);
                                        if (row.CompanyCode == "IBR" & jsonresponse != "")
                                        {
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                                        if (Fa.Enabled == true)
                                        {
                                            //Agregar datos flujo alterno
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }
                                }

                                Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método Details exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                                DBOperations.InsertBitacora(btreq);


                                log4net.ThreadContext.Properties["Response"] = jsonresponse;
                                log4net.ThreadContext.Properties["Request"] = jsonrequest;
                                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));

                            }
                            catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                            {

                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
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

                            catch (UnauthorizedAccessException ex)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            }

                            return Ok(listDetails);
                        }
                        catch (UnauthorizedAccessException ex)
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


    }
}