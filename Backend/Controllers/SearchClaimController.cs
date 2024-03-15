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
using System.Dynamic;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class SearchClaimController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene un objeto por certificado
        /// </summary>
        /// <param name="certificate"></param>
        /// <response code="200"> OK. Devuelve el objeto solicitado</response>
        /// <returns></returns>
        // 0000013000039606251
        // GET: SearchClaim
        [Route("api/SearchClaim/{certificate}/{dealercode}/{idcliente}")]
        public IHttpActionResult GetDetails(string certificate, string dealercode, string idcliente)
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    string jsonresponse = "[]";
                    string jsonrequest = string.Empty;
                    string jsonConfig = string.Empty;
                    try
                    {
                        var policyServiceClient = new ClaimServiceWS.ClaimServiceClient(Constants.ENPTNAMECLAIM, Constants.URLCLAIM3);
                        Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(idcliente)); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];

                        ClaimServiceWS.ClaimSearchResponse searchResponse = null;

                        try
                        {
                            var claimLookup = new ClaimServiceWS.SearchClaimByCertificateNumber
                            {
                                DealerCode = dealercode
                                ,
                                CertificateNumber = certificate
                            };

                            var searchRequest = new ClaimServiceWS.ClaimSearchRequest
                            {
                                ClaimLookup = claimLookup
                               ,
                                CultureCode = jsonObj["CultureCode"]
                            };
                            jsonrequest = JsonConvert.SerializeObject(searchRequest);
                            searchResponse = policyServiceClient.Search(searchRequest);

                            var result = searchResponse.ClaimResponse;

                            DataTable dt = DBOperations.GetDescriptionIssue(certificate);
                            List<ResponseSearchData> listaresponse = new List<ResponseSearchData>();
                            ResponseSearchData newdata = null;
                            int loop = 0;
                            foreach (var data in result)
                            {
                                string text = (from DataRow myRow in dt.Rows
                                               where myRow.Field<string>("ClaimNumber") == data.ClaimNumber
                                               select myRow.Field<string>("RespInfoClaimDescription")).FirstOrDefault();

                                newdata = new ResponseSearchData
                                {
                                    CustomerDescription = text,
                                    AuthorizationCount = data.AuthorizationCount,
                                    AuthorizationNumber = data.AuthorizationNumber,
                                    AuthorizedAmount = data.AuthorizedAmount,
                                    CallerEmail = data.CallerEmail,
                                    CallerName = data.CallerName,
                                    CallerPhoneNumber = data.CallerPhoneNumber,
                                    CaseCreatedDate = data.CaseCreatedDate,
                                    CaseDenialReason = data.CaseDenialReason,
                                    CaseNumber = data.CaseNumber,
                                    CaseStatus = data.CaseStatus,
                                    CertificateNumber = data.CertificateNumber,
                                    ClaimCreatedBy = data.ClaimCreatedBy,
                                    ClaimEquipment = data.ClaimEquipment,
                                    ClaimEquipmentList = data.ClaimEquipmentList,
                                    ClaimId = data.ClaimId,
                                    ClaimNumber = data.ClaimNumber,
                                    ClaimStatus = data.ClaimStatus,
                                    ClosedReasonCode = data.ClosedReasonCode,
                                    CompanyCode = data.CompanyCode,
                                    CoverageType = data.CoverageType,
                                    CoverageTypeCode = data.CoverageTypeCode,
                                    DateOfLoss = data.DateOfLoss,
                                    DealerCode = data.DealerCode,
                                    DealerGroup = data.DealerGroup,
                                    Deductible = data.Deductible,
                                    DeviceNickname = data.DeviceNickname,
                                    DeviceSerialNumber = data.DeviceSerialNumber,
                                    ExpectedRepairDate = data.ExpectedRepairDate,
                                    ExtensionData = data.ExtensionData,
                                    ExtStatusCode = data.ExtStatusCode,
                                    ExtStatusDescription = data.ExtStatusDescription,
                                    FulfillmentOptionCode = data.FulfillmentOptionCode,
                                    IssueStatus = data.IssueStatus,
                                    MethodOfRepair = data.MethodOfRepair,
                                    PaymentAmount = data.PaymentAmount,
                                    PaymentAmountWithoutConseqDamage = data.PaymentAmountWithoutConseqDamage,
                                    PickupDate = data.PickupDate,
                                    RepairDate = data.RepairDate,
                                    ReportedDate = data.ReportedDate,
                                    RiskType = data.RiskType,
                                    SerialNumber = data.SerialNumber,
                                    ShippingAddress = data.ShippingAddress,
                                    StatusCode = data.StatusCode,
                                    VisitDate = data.VisitDate
                                };

                                listaresponse.Add(newdata);
                                loop++;
                            }

                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método SearchClaim exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            jsonresponse = JsonConvert.SerializeObject(listaresponse.ToArray());
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método SearchClaim exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<ClaimServiceWS.ClaimNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<ClaimServiceWS.ValidationFaultItem> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"), fault);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<ClaimServiceWS.ValidationFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"), fault);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"), fault);
                                return BadRequest();
                            }
                        }
                        catch (Exception ex)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                return Ok(GenerateJsonResponseSimulation(certificate));
                            }
                            else
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                return BadRequest();
                            }
                        }

                        return Ok(jsonresponse);
                    }
                    catch (Exception ex)
                    {
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonrequest},{jsonresponse},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return Ok(GenerateJsonResponseSimulation(certificate));
                        }
                        else
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonrequest},{jsonresponse},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return BadRequest();
                        }

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
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(idcliente));
                if (Fa.Enabled == true)
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
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
        public string GenerateJsonResponseSimulation(string certificado)
        {
            string data = "[{\"CustomerDescription\":\"PRUEB\",\"AuthorizationCount\":0,\"AuthorizationNumber\":null,\"AuthorizedAmount\":0.0,\"CallerEmail\":\"a@f.com\",\"CallerName\":\"JavierHernandez A\",\"CallerPhoneNumber\":\"\",\"CaseCreatedDate\":\"2022-11-30T19:09:27\",\"CaseDenialReason\":\"\",\"CaseNumber\":\"2022069717\",\"CaseStatus\":\"CASESTAT-OPEN\",\"CertificateNumber\":\"1\",\"ClaimCreatedBy\":\"\",\"ClaimEquipment\":null,\"ClaimEquipmentList\":[],\"ClaimId\":\"00000000-0000-0000-0000-000000000000\",\"ClaimNumber\":\"\",\"ClaimStatus\":\"\",\"ClosedReasonCode\":\"\",\"CompanyCode\":\"ASM\",\"CoverageType\":\"\",\"CoverageTypeCode\":\"\",\"DateOfLoss\":\"1900-01-01T00:00:00\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"Deductible\":0.0,\"DeviceNickname\":\"\",\"DeviceSerialNumber\":\"\",\"ExpectedRepairDate\":null,\"ExtensionData\":{},\"ExtStatusCode\":\"\",\"ExtStatusDescription\":\"\",\"FulfillmentOptionCode\":\"\",\"IssueStatus\":[],\"MethodOfRepair\":\"\",\"PaymentAmount\":0.0,\"PaymentAmountWithoutConseqDamage\":0.0,\"PickupDate\":null,\"RepairDate\":null,\"ReportedDate\":\"1900-01-01T00:00:00\",\"RiskType\":\"\",\"SerialNumber\":\"\",\"ShippingAddress\":{\"Address1\":\"\",\"Address2\":\"\",\"Address3\":\"\",\"City\":\"\",\"State\":\"\",\"Country\":\"\",\"PostalCode\":\"\"},\"StatusCode\":\"\",\"VisitDate\":null},{\"CustomerDescription\":\"PRUEB\",\"AuthorizationCount\":0,\"AuthorizationNumber\":null,\"AuthorizedAmount\":0.0,\"CallerEmail\":\"ester.santamaria@gmail.com\",\"CallerName\":\"JavierHernandez Prueba\",\"CallerPhoneNumber\":\"\",\"CaseCreatedDate\":\"2022-12-08T14:13:52\",\"CaseDenialReason\":\"\",\"CaseNumber\":\"2022069727\",\"CaseStatus\":\"CASESTAT-OPEN\",\"CertificateNumber\":\"98F0E491-8892-11E8-A78E-000000000000-417\",\"ClaimCreatedBy\":\"\",\"ClaimEquipment\":null,\"ClaimEquipmentList\":[],\"ClaimId\":\"00000000-0000-0000-0000-000000000000\",\"ClaimNumber\":\"\",\"ClaimStatus\":\"\",\"ClosedReasonCode\":\"\",\"CompanyCode\":\"ASM\",\"CoverageType\":\"\",\"CoverageTypeCode\":\"\",\"DateOfLoss\":\"1900-01-01T00:00:00\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"Deductible\":0.0,\"DeviceNickname\":\"\",\"DeviceSerialNumber\":\"\",\"ExpectedRepairDate\":null,\"ExtensionData\":{},\"ExtStatusCode\":\"\",\"ExtStatusDescription\":\"\",\"FulfillmentOptionCode\":\"\",\"IssueStatus\":[],\"MethodOfRepair\":\"\",\"PaymentAmount\":0.0,\"PaymentAmountWithoutConseqDamage\":0.0,\"PickupDate\":null,\"RepairDate\":null,\"ReportedDate\":\"1900-01-01T00:00:00\",\"RiskType\":\"\",\"SerialNumber\":\"\",\"ShippingAddress\":{\"Address1\":\"\",\"Address2\":\"\",\"Address3\":\"\",\"City\":\"\",\"State\":\"\",\"Country\":\"\",\"PostalCode\":\"\"},\"StatusCode\":\"\",\"VisitDate\":null},{\"CustomerDescription\":\"PRUEB\",\"AuthorizationCount\":0,\"AuthorizationNumber\":null,\"AuthorizedAmount\":0.0,\"CallerEmail\":\"ag@co.com\",\"CallerName\":\"JavierHernandez Salazar\",\"CallerPhoneNumber\":\"3432432432\",\"CaseCreatedDate\":\"2022-12-08T12:08:14\",\"CaseDenialReason\":\"DNOCO\",\"CaseNumber\":\"2022069725\",\"CaseStatus\":\"CASESTAT-CLOSED\",\"CertificateNumber\":\"98F0E491-8892-11E8-A78E-000000000000-417\",\"ClaimCreatedBy\":\"\",\"ClaimEquipment\":null,\"ClaimEquipmentList\":[],\"ClaimId\":\"00000000-0000-0000-0000-000000000000\",\"ClaimNumber\":\"\",\"ClaimStatus\":\"\",\"ClosedReasonCode\":\"\",\"CompanyCode\":\"ASM\",\"CoverageType\":\"\",\"CoverageTypeCode\":\"\",\"DateOfLoss\":\"1900-01-01T00:00:00\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"Deductible\":0.0,\"DeviceNickname\":\"\",\"DeviceSerialNumber\":\"\",\"ExpectedRepairDate\":null,\"ExtensionData\":{},\"ExtStatusCode\":\"\",\"ExtStatusDescription\":\"\",\"FulfillmentOptionCode\":\"\",\"IssueStatus\":[],\"MethodOfRepair\":\"\",\"PaymentAmount\":0.0,\"PaymentAmountWithoutConseqDamage\":0.0,\"PickupDate\":null,\"RepairDate\":null,\"ReportedDate\":\"1900-01-01T00:00:00\",\"RiskType\":\"\",\"SerialNumber\":\"\",\"ShippingAddress\":{\"Address1\":\"\",\"Address2\":\"\",\"Address3\":\"\",\"City\":\"\",\"State\":\"\",\"Country\":\"\",\"PostalCode\":\"\"},\"StatusCode\":\"\",\"VisitDate\":null},{\"CustomerDescription\":\"PRUEB\",\"AuthorizationCount\":0,\"AuthorizationNumber\":null,\"AuthorizedAmount\":0.0,\"CallerEmail\":\"ag@co.com\",\"CallerName\":\"JavierHernandez Salazar\",\"CallerPhoneNumber\":\"3432432432\",\"CaseCreatedDate\":\"2022-12-08T14:05:05\",\"CaseDenialReason\":\"DNOCO\",\"CaseNumber\":\"2022069726\",\"CaseStatus\":\"CASESTAT-CLOSED\",\"CertificateNumber\":\"98F0E491-8892-11E8-A78E-000000000000-417\",\"ClaimCreatedBy\":\"\",\"ClaimEquipment\":null,\"ClaimEquipmentList\":[],\"ClaimId\":\"00000000-0000-0000-0000-000000000000\",\"ClaimNumber\":\"\",\"ClaimStatus\":\"\",\"ClosedReasonCode\":\"\",\"CompanyCode\":\"ASM\",\"CoverageType\":\"\",\"CoverageTypeCode\":\"\",\"DateOfLoss\":\"1900-01-01T00:00:00\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"Deductible\":0.0,\"DeviceNickname\":\"\",\"DeviceSerialNumber\":\"\",\"ExpectedRepairDate\":null,\"ExtensionData\":{},\"ExtStatusCode\":\"\",\"ExtStatusDescription\":\"\",\"FulfillmentOptionCode\":\"\",\"IssueStatus\":[],\"MethodOfRepair\":\"\",\"PaymentAmount\":0.0,\"PaymentAmountWithoutConseqDamage\":0.0,\"PickupDate\":null,\"RepairDate\":null,\"ReportedDate\":\"1900-01-01T00:00:00\",\"RiskType\":\"\",\"SerialNumber\":\"\",\"ShippingAddress\":{\"Address1\":\"\",\"Address2\":\"\",\"Address3\":\"\",\"City\":\"\",\"State\":\"\",\"Country\":\"\",\"PostalCode\":\"\"},\"StatusCode\":\"\",\"VisitDate\":null}]";
           
            return data;
        }
    }
}