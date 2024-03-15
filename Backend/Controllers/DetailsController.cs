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
    public class DetailsController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene los detalles de la póliza
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/details/0000013000021015917/1
        [Route("api/details/{certificate}/{dealercode}/{serialnumber}/{id}/{poliza}")]
        public IHttpActionResult GetDetails(string certificate, string dealerCode, string serialnumber, int id, string poliza)
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    PolicyServiceWS.GetDetailsResponse responsePolicy = null;

                    try
                    {
                        log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                        log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                        var policyServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                        var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);

                        string jsonresponse = string.Empty;
                        string jsonConfig = string.Empty;
                        string jsonrequest = string.Empty;
                        Cliente cl = DBOperations.GetClientesById(id); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);

                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];



                       
                        try
                        {
                            var requestPolicy = new PolicyServiceWS.GetDetailsRequest
                            {
                                DealerCode = dealerCode,
                                CertificateNumber = certificate
                            };
                            jsonrequest = JsonConvert.SerializeObject(requestPolicy);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método Details exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            responsePolicy = policyServiceClient.GetDetails(requestPolicy);
                            jsonresponse = JsonConvert.SerializeObject(responsePolicy);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método Details exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(serialnumber));
                            }
                            else
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(serialnumber));
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
                                log.Error(string.Format("DealerNotFoundFault- {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(serialnumber));
                            }
                            else
                            {
                                log.Error(string.Format("DealerNotFoundFault- {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return Ok(GenerateJsonResponseSimulation(serialnumber));
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
                                return Ok(GenerateJsonResponseSimulation(serialnumber));
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
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(id));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return Ok(GenerateJsonResponseSimulation(serialnumber));
                        }
                        else
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return BadRequest();
                        }
                    }
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", "Error al obtener token"), null);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. PolicyDetails", Plataforma = "MSSP_ELITA" };
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
                    return Ok(GenerateJsonResponseSimulation(serialnumber));
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
        public string GenerateJsonResponseSimulation(string SerialNumber)
        {
            string data = "{\"Bank\":{\"AccountNumber\":null,\"AccountOwnerName\":null,\"AccountType\":null,\"BankLookupCode\":null,\"BankName\":null,\"BankSortCode\":null,\"BranchName\":null,\"BranchNumber\":0,\"IbanCode\":null,\"IbanCode_Last4Digits\":null,\"RoutingNumber\":null,\"SwiftCode\":null},\"CertificateDuration\":\"576\",\"CertificateNumber\":\"1\",\"CompanyCode\":\"ASM\",\"Coverages\":[{\"CoverageTypeCode\":\"Extended\",\"BeginDate\":\"11/1/2020 12:00:00 AM\",\"EndDate\":\"5/31/2022 12:00:00 AM\",\"CoverageLiablityLimit\":\"0\",\"RemainingCoverageLiablityLimit\":null,\"CoverageClaimLiabilityLimit\":null,\"RemainingCoverageClaimLiablityLimit\":null,\"CoverageDuration\":\"1\"},{\"CoverageTypeCode\":\"Power Surge\",\"BeginDate\":\"11/1/2020 12:00:00 AM\",\"EndDate\":\"5/31/2022 12:00:00 AM\",\"CoverageLiablityLimit\":\"0\",\"RemainingCoverageLiablityLimit\":null,\"CoverageClaimLiabilityLimit\":null,\"RemainingCoverageClaimLiablityLimit\":null,\"CoverageDuration\":\"1\"}],\"CurrencyCode\":\"MXN\",\"Customers\":[{\"Address\":{\"Address1\":\"\",\"Address2\":\"\",\"Address3\":\"\",\"AddressType\":0,\"City\":\"\",\"Country\":\"ASM\",\"CurrencyCode\":null,\"PostalCode\":\"\",\"State\":\"\",\"Type\":0},\"CustomerId\":null,\"CustomerName\":\"TESTA ALARCON ANGEL RENE\",\"CustomerType\":1,\"FirstName\":null,\"IdentificationNumber\":\"TEAA9705144D7\",\"LastName\":null,\"MiddleName\":null,\"Phone\":[{\"PhoneNumber\":\"04457962\",\"Type\":1}]}],\"DealerCode\":\"EA13\",\"DealerCodeDescription\":\"Liverpool - PIF\",\"ItemInfo\":[{\"Color\":\"\",\"Description\":\"\",\"ImeiNumber\":\"\",\"ItemEffectiveDate\":\"2020-11-01T00:00:00\",\"ItemExpirationDate\":\"2022-05-31T00:00:00\",\"Manufacturer\":\"\",\"Memory\":\"\",\"Model\":\"\",\"PendingApprovalManufacturer\":\"N\",\"RegisteredDevices\":[{\"DeviceType\":\"REFRI\",\"RegisteredItemIdentifier\":\"HS.REF.ROJ.7FT.RR63D6WRX   (1 PZA)\",\"RegisteredDeviceName\":\"" + SerialNumber + "\",\"Make\":\"\",\"Model\":\"Refrigerador\",\"SerialNumber\":\"" + SerialNumber + "\",\"DevicePurChasePrice\":\"20000\",\"DevicePurchaseDate\":\"5/1/2021 12:00:00 AM\",\"ItemDescription\":\"LINEA BLANCA +G\",\"RegisteredRetailPrice\":\"0\",\"RegistrationDate\":\"8/3/2022 12:00:00 AM\",\"RegisteredItemIndixID\":\"true\",\"RegisteredItemStatusCode\":\"A\",\"ExpirationDate\":\"\",\"Color\":\"\",\"MemorySize\":\"\",\"SkuNumber\":\"\",\"DualSim\":\"\",\"RamSize\":\"\"}],\"RiskTypeCode\":\"Gadgets\",\"RiskTypeDescription\":\"Gadgets\",\"SerialNumber\":\"\",\"SimCardTypeCode\":\"UNKNOWN\",\"SkuNumber\":\"\"}],\"ItemRetailPrice\":0.0,\"LastDateOfPayment\":\"0001-01-01T00:00:00\",\"MaxNumberOfClaims\":\"9999\",\"MaxNumberOfRepairClaims\":\"9999\",\"MaxNumberOfReplacementClaims\":\"9999\",\"NextBillingDate\":\"0001-01-01T00:00:00\",\"PaidThroughDate\":\"0001-01-01T00:00:00\",\"Payment\":{\"BillingCycle\":null,\"BillingDocumentType\":null,\"BillingFrequency\":0,\"BillingFrequencyDescription\":null,\"BillingPlan\":null,\"InstallmentAmount\":null,\"InvoiceNumber\":null,\"LastBilledAmount\":null,\"LastBilledAmtDate\":\"0001-01-01T00:00:00\",\"LastFailedTransactionAmount\":null,\"LastFailedTransactionDate\":\"0001-01-01T00:00:00\",\"LastFailedTransactionReason\":null,\"NoOfInstallments\":null,\"PaymentInstrumentDescription\":\"CASH\",\"PaymentInstrumentType\":3,\"PaymentReferenceNumber\":\"\",\"PaymentType\":1,\"PostPrePaid\":null},\"PaymentAmount\":null,\"PaymentInvoiceNumber\":\"0\",\"PaymentReferenceNumber\":\"\",\"PolicyClaimLiabilityLimit\":null,\"PolicyLiabilityLimit\":\"0\",\"ProductInfo\":{\"PremiumType\":1,\"ProductCode\":\"LVPIF\",\"ProductDescription\":\"Liverpool PIF\",\"ProductSalesDate\":\"2020-11-01T00:00:00\",\"SalesPrice\":0.0,\"WarrantyDurationMonths\":null,\"WarrantySalesDate\":\"2020-11-01T00:00:00\"},\"RemainingPolicyClaimLiabilityLimit\":null,\"RemainingPolicyLiabilityLimit\":null,\"Statuscode\":\"A\",\"TotalGrossAmount\":75.24,\"TotalTax\":10.44,\"ClaimWaitingPeriodDays\":0,\"LoanCode\":null,\"ServiceLineNumber\":null,\"CertificateExtendedItems\":[{\"FieldName\":\"CERTIFICATE_SIGNED\",\"FieldValue\":\"DOCUMENT_STATUS-NV\"},{\"FieldName\":\"CHECK_SIGNED\",\"FieldValue\":\"DOCUMENT_STATUS-NV\"},{\"FieldName\":\"SEPA_MANDATE_SIGNED\",\"FieldValue\":\"DOCUMENT_STATUS-NV\"}],\"MembershipNumber\":\"1-JF3LXUO\",\"SubscriberStatusCode\":null,\"SubscriberStatusDescription\":null,\"SuspendedReasonCode\":null,\"SuspendedReasonDescription\":null,\"RefundAmount\":0.0}";
       
            return data;
        }
    }
}