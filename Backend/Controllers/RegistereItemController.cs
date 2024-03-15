using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class RegistereItemController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates an Employee.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/Employee
        ///     {
        ///       "firstName": "Mike",
        ///       "lastName": "Andrew",
        ///       "emailId": "Mike.Andrew@gmail.com"
        ///     }
        /// </remarks>
        /// <param></param>
        /// <returns>A newly created employee</returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>

        [HttpPost]
        [Route("api/registeritem")]
        public IHttpActionResult GetRegistereItem(Models.Item jsonString)
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    PolicyServiceWS.RegisterItemRespose responsePolicy = null;

                    try
                    {
                        Models.Item oItem = jsonString;
                        log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                        log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                        var policyServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                        string jsonresponse = string.Empty;
                        string jsonConfig = string.Empty;
                        string jsonrequest = string.Empty;
                        Cliente cl = DBOperations.GetClientesById(oItem.IdCliente); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];
                        List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(Convert.ToInt32(oItem.IdCliente));
                        List<DealerCodes> dco = DBOperations.GetDealerCodesByDealerCode(jsonString.DealerCode);
                        try
                        {
                            if (cl.Multiple == true)
                            {

                                var claimlookup = new PolicyServiceWS.SearchPolicyByMembershipNumber
                                {
                                    MembershipNumber = jsonString.Poliza,
                                    CompanyCode = dco.First().CompanyCode,
                                    DealerCode = dco.First().DealerCode,
                                };
                                var searchRequest = new PolicyServiceWS.SearchRequest
                                {
                                    PolicyLookup = claimlookup
                                };

                                // Extraigo el metodo el certificado
                                jsonrequest = JsonConvert.SerializeObject(searchRequest);
                                var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
                                claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                                claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];

                                var response = claimServiceClient.Search(searchRequest);

                                jsonresponse = JsonConvert.SerializeObject(response);
                                dynamic ElitaJson = JsonConvert.DeserializeObject(jsonresponse);
                                dynamic dynamic = ElitaJson.PolicyResponse;

                                foreach (var row in dynamic)
                                {
                                    string a = row.WarrantySalesDate.ToString();

                                    string fechaformato = DateHelper.GetDateParse(a);

                                    if (fechaformato == jsonString.R3Fechaservicio)
                                    {
                                        jsonString.CertificateNumber = row.CertificateNumber.ToString();
                                    }
                                }

                                if (jsonString.CertificateNumber == "1")
                                {
                                    string arrystring = ElitaJson.PolicyResponse.ToString();
                                    JArray jsonVal = JArray.Parse(arrystring);

                                    jsonString.CertificateNumber = new JArray(jsonVal.OrderByDescending(obj => obj["WarrantySalesDate"])).FirstOrDefault().Value<string>("CertificateNumber");

                                }

                            }

                            var requestPolicy = new PolicyServiceWS.RegisterItemRequest
                            {
                                DealerCode = dc.First().DealerCode,
                                CertificateNumber = jsonString.CertificateNumber,
                                DeviceTypeCode = jsonString.DeviceTypeCode,
                                IndixID = "true",
                                ItemDescription = jsonString.ItemDescription,
                                Manufacturer = jsonString.Manufacturer,
                                Model = jsonString.Model,
                                PurchasePrice = jsonString.PurchasePrice,
                                PurchasedDate = Convert.ToDateTime(DateHelper.GetDateString(DateTime.ParseExact(jsonString.PurchasedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))),
                                RegisteredItemName = jsonString.RegisteredItemName,
                                RegistrationDate = Convert.ToDateTime(DateHelper.GetDateString(DateTime.Now)),
                                RetailPrice = jsonString.RetailPrice,
                                //SerialNumber = jsonString.SerialNumber,
                            };
                            jsonrequest = JsonConvert.SerializeObject(requestPolicy);
                            responsePolicy = policyServiceClient.RegisterItem(requestPolicy);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método RegistereItem exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            jsonresponse = JsonConvert.SerializeObject(responsePolicy);
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método RegistereItem exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                                return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
                            }
                            else
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                                return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
                            }
                            else
                            {
                                log.Error(string.Format("EnrollFault - {0}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                                return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
                            }
                            else
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                                return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (Exception ex)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", ex.Message), ex);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                                return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
                            }
                            else
                            {
                                log.Error(string.Format("Exception - {0}", ex.Message), ex);
                                return BadRequest();
                            }
                        }
                        log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                        return Ok(jsonresponse);
                    }
                    catch (Exception ex)
                    {
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{ex.Message},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                            return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
                        }
                        else
                        {
                            log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{ex.Message},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            return BadRequest();
                        }
                    }
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", "Error al obtener token"), null);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. RegisteredItem", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                if (Fa.Enabled == true)
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    DBOperations.UpdateClaimFolioFromDeviceSelection(jsonString.SerialNumber != "" ? jsonString.SerialNumber : jsonString.RegisteredItemName);
                    return Ok(GenerateJsonResponseSimulation(jsonString.CertificateNumber));
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
            string data = "{\"ErrorCode\":\"0\",\"ErrorMessage\":\"null\",\"PolicyResponse\":[{\"CertificateNumber\":\"1\",\"CustomerName\":\"GOMEZ ZAMARRIPA SONIA KARINA\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"HomePhone\":\"08100943\",\"IdentificationNumber\":\"GOZS7003096M2\",\"ImeiNumber\":\"\",\"ItemDescription\":\"04 PIF PLUS\",\"Carrier\":null,\"ItemEffectiveDate\":\"2020-03-01T00:00:00\",\"Manufacturer\":\"\",\"Model\":\"\",\"ProductSalesDate\":\"2020-03-01T00:00:00\",\"ProductDescription\":\"Liverpool PIF\",\"ProductCode\":\"LVPIF\",\"SerialNumber\":\"\",\"ServiceLineNumber\":\"\",\"SKUNumber\":\"\",\"Statuscode\":\"A\",\"WarrantySalesDate\":\"2020-03-01T00:00:00\",\"WorkPhone\":\"\",\"MasterCertificateNumber\":\"\",\"PromoCodes\":\"\",\"ActivationDate\":\"2020-03-01T00:00:00\"},{\"CertificateNumber\":\"1\",\"CustomerName\":\"RAMIREZ MILLAN PERLA\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"HomePhone\":\"5554710406\",\"IdentificationNumber\":\"RAMP760413LU2\",\"ImeiNumber\":\"\",\"ItemDescription\":\"PS4 DS4 Midnight Blue\",\"Carrier\":null,\"ItemEffectiveDate\":\"2020-03-01T00:00:00\",\"Manufacturer\":\"MICROSOFT\",\"Model\":\"\",\"ProductSalesDate\":\"2020-03-01T00:00:00\",\"ProductDescription\":\"Liverpool PIF\",\"ProductCode\":\"LVPIF\",\"SerialNumber\":\"\",\"ServiceLineNumber\":\"\",\"SKUNumber\":\"\",\"Statuscode\":\"A\",\"WarrantySalesDate\":\"2020-03-01T00:00:00\",\"WorkPhone\":\"\",\"MasterCertificateNumber\":\"\",\"PromoCodes\":\"\",\"ActivationDate\":\"2020-03-01T00:00:00\"}]}";
            return data;
        }
    }
}