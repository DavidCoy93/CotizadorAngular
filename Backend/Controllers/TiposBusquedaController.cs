using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;

namespace MSSPAPI.Controllers
{
    public class TiposBusquedaController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("api/TiposBusqueda/{poliza}/{certificate}/{id}")]
        public IHttpActionResult GetTiposBusqueda(string poliza, string certificate, string id)
        {
            try
            {
                List<TiposBusqueda> tb = DBOperations.GetTipoBusquedaByCliente();
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    string jsonresponse = string.Empty;
                    string jsonrequest = string.Empty;
                    string jsonConfig = string.Empty;

                    var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                    Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(id)); //Constantes cuando haya mas clientes
                    jsonConfig = cl.Configuraciones;
                    dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                    claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                    claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];
                    List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(Convert.ToInt32(id));
                    foreach(var tipo in tb)
                    {
                        dynamic jsonParametros = JsonConvert.DeserializeObject(tipo.Parametros);
                        switch (tipo.IdMetodoBusqueda)
                        {
                            case 1:
                                var claimlookup = new PolicyServiceWS.SearchPolicyByMembershipNumber
                                {
                                    MembershipNumber = jsonParametros["MembershipNumber"], //
                                    CompanyCode = jsonParametros["CompanyCode"],
                                    DealerCode = jsonParametros["DealerCode"]
                                };
                                var searchRequest = new PolicyServiceWS.SearchRequest
                                {
                                    PolicyLookup = claimlookup
                                };
                                jsonrequest = JsonConvert.SerializeObject(searchRequest);
                                break;
                            case 2:
                                var claimlookupBirth = new PolicyServiceWS.SearchPolicyByCertificateNumberDateofBirth
                                {
                                    BirthDate = jsonParametros["MembershipNumber"],
                                    CertificateNumber= jsonParametros["CertificateNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    CertificateNumber = jsonParametros["CertificateNumber"],
                                    CompanyGroup = jsonParametros["CompanyGroup"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    Email = jsonParametros["Email"],
                                    IdentificationNumber = jsonParametros["IdentificationNumber"],
                                    InvoiceNumber = jsonParametros["InvoiceNumber"],
                                    PhoneNumber = jsonParametros["PhoneNumber"],
                                    PostalCode = jsonParametros["PostalCode"],
                                    SerialNumber = jsonParametros["SerialNumber"],
                                    ServiceLineNumber = jsonParametros["ServiceLineNumber"],
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
                                    Email = jsonParametros["MembershipNumber"],
                                    DealerGroup = jsonParametros["CompanyCode"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    CompanyCode = jsonParametros["CompanyCode"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    Email = jsonParametros["Email"],
                                    PostalCode = jsonParametros["PostalCode"],
                                };
                                var searchRequestPostalC = new PolicyServiceWS.SearchRequest
                                {
                                    PolicyLookup = claimlookupPostalC
                                };
                                jsonrequest = JsonConvert.SerializeObject(searchRequestPostalC);
                                break;
                            case 6:
                                var claimlookupIdentiN = new PolicyServiceWS.SearchPolicyByIdentificationNumber
                                {
                                    BirthDate = jsonParametros["BirthDate"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    HomePhoneNumber = jsonParametros["HomePhoneNumber"],
                                    IdentificationNumber = jsonParametros["IdentificationNumber"],
                                    PhoneNumber = jsonParametros["PhoneNumber"]
                                };
                                var searchRequestIdentiN = new PolicyServiceWS.SearchRequest
                                {
                                    PolicyLookup = claimlookupIdentiN
                                };
                                jsonrequest = JsonConvert.SerializeObject(searchRequestIdentiN);
                                break;
                            case 7:
                                var claimlookupINPN = new PolicyServiceWS.SearchPolicyByIdentificationNumberPhoneNumber
                                {
                                    IdentificationNumber = jsonParametros["IdentificationNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    PhoneNumber = jsonParametros["PhoneNumber"]
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
                                    ImeiNumber = jsonParametros["ImeiNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    ImeiNumber = jsonParametros["ImeiNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    PhoneNumber = jsonParametros["PhoneNumber"]
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
                                    InvoiceNumber = jsonParametros["InvoiceNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    LastName = jsonParametros["LastName"],
                                    InvoiceNumber = jsonParametros["InvoiceNumber"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    InvoiceNumber = jsonParametros["InvoiceNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    PostalCode = jsonParametros["PostalCode"]
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
                                    BirthDate = jsonParametros["BirthDate"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    LastName = jsonParametros["LastName"]
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
                                    BirthDate = jsonParametros["BirthDate"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    HomePhoneNumber = jsonParametros["HomePhoneNumber"],
                                    MembershipNumber = jsonParametros["MembershipNumber"],
                                    PhoneNumber = jsonParametros["PhoneNumber"]
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
                                    AccountNumber = jsonParametros["AccountNumber"],
                                    BankSortCode = jsonParametros["BankSortCode"],
                                    BirthDate = jsonParametros["BirthDate"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    FirstName = jsonParametros["FirstName"],
                                    LastName = jsonParametros["LastName"]
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
                                    PhoneNumber = jsonParametros["PhoneNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    PostalCode = jsonParametros["PostalCode"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    PhoneNumber = jsonParametros["PhoneNumber"]
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
                                    SerialNumber = jsonParametros["SerialNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    ServiceLineNumber = jsonParametros["ServiceLineNumber"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"]
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
                                    BirthDate = jsonParametros["BirthDate"],
                                    DealerGroup = jsonParametros["DealerGroup"],
                                    DealerCode = jsonParametros["DealerCode"],
                                    
                                };
                                var searchRequestDB = new PolicyServiceWS.SearchRequest
                                {
                                    PolicyLookup = claimlookupDB
                                };
                                jsonrequest = JsonConvert.SerializeObject(searchRequestDB);
                                break;

                        }

                    }
                   
                    return Ok(tb);
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