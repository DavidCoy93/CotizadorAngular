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
using MSSPAPI.ClaimServiceWS;
using Newtonsoft.Json.Linq;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class BeginClaimController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: BeginClaim
        /// <summary>
        /// Registra el claim hacia Elita
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/registerclaim")]
        public IHttpActionResult GetClaim(DataBeginClaim jsonString)
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


                        var policyServiceClient = new ClaimRecordingServiceWS.ClaimRecordingServiceClient(Constants.ENPTNAMECLAIM2, Constants.URLCLAIM2);

                        ClaimRecordingServiceWS.BaseClaimRecordingResponse claimReponse = null;
                        Cliente cl = DBOperations.GetClientesById(jsonString.IdCliente); //Constantes cuando haya mas clientes
                        List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(jsonString.IdCliente);
                        List<DealerCodes> dco = DBOperations.GetDealerCodesByDealerCode(jsonString.DealerCode);
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];


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

                        try
                        {
                            var reference = new ClaimRecordingServiceWS.CertificateReference
                            {
                                CompanyCode = jsonString.CompanyCode,
                                DealerCode = jsonString.DealerCode                                ,
                                CertificateNumber = jsonString.CertificateNumber
                            };

                            var caller = new ClaimRecordingServiceWS.WebCaller
                            {
                                FirstName = jsonString.FirstName,
                                LastName = jsonString.LastName,
                                RelationshipTypeCode = jsonString.RelationshipTypeCode,
                                ChannelCode = jsonString.ChannelCode,
                                EmailAddress = jsonString.EmailAddress,
                                CultureCode = jsonObj["CultureCode"],
                                IsAuthenticated = false,
                                ClientIpAddress = jsonString.ClientIpAddress
                            };

                            var Claim = new ClaimRecordingServiceWS.CreateCaseRequest
                            {
                                Reference = reference,
                                Caller = caller,
                                PurposeCode = jsonString.PurposeCode
                            };

                            jsonrequest = JsonConvert.SerializeObject(Claim);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método BeginClaim exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            claimReponse = policyServiceClient.BeginClaim(Claim);
                            jsonresponse = JsonConvert.SerializeObject(claimReponse);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;

                            dynamic jObj = JsonConvert.DeserializeObject(jsonresponse);
                            string certificadopadre = "";
                            string caso = jObj["CaseNumber"];
                            string personal = jObj["CustomerInfo"]["CustomerName"] + "," + jObj["CustomerInfo"]["EmailAddress"] + "," + jObj["CustomerInfo"]["HomePhone"] + "," + jObj["CustomerInfo"]["WorkPhone"];
                            DBOperations.InsertClaim(caso, personal, EncDec.Encript(jsonString.FirstName + " " + jsonString.LastName), EncDec.Encript(jsonString.EmailAddress), Constants.IdClienteLiverpool, certificadopadre);
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método BeginClaim exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                                return Ok(GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                                return Ok(GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                                return Ok(GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                                return Ok(GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (Exception ex)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                                return Ok(GenerateJsonResponseSimulation());
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
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                            return Ok(GenerateJsonResponseSimulation());
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
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. BeginClaim", Plataforma = "MSSP_ELITA" };
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
                    //DBOperations.UpdateClaimFolioFromBeginClaim(jsonString.CertificateNumber);
                    return Ok(GenerateJsonResponseSimulation());
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
        public string GenerateJsonResponseSimulation()
        {
            string data = "{\"RegisteredItems\":[{\"ImeiNumber\":\"\",\"SerialNumber\":\"\",\"Manufacturer\":\"MAYTAG\",\"Model\":\"LINEA BLANCA +G//Lavadora\",\"RegisteredItemName\":\"MY.LAV.20.BLANCO.7MMVWC465JW   (1 PZA)\",\"Capacity\":null,\"PurchasePrice\":13500.0,\"Color\":null,\"PurchasedDate\":\"2021-03-01T00:00:00\",\"Carrier\":null,\"DeviceType\":\"Lavadora\",\"Description\":null,\"ExpirationDate\":\"2022-10-31T00:00:00\",\"ModelFamily\":null,\"RiskTypeCode\":\"Gadgets\",\"ManufacturerIdentifier\":null,\"SkuNumber\":null,\"SimCardTypeXcd\":null},{\"ImeiNumber\":\"\",\"SerialNumber\":\"\",\"Manufacturer\":\"Cinepolis\",\"Model\":\"Refrigerador\",\"RegisteredItemName\":\"MSG20220301300921010010\",\"Capacity\":null,\"PurchasePrice\":0.0,\"Color\":null,\"PurchasedDate\":\"2021-08-12T00:00:00\",\"Carrier\":null,\"DeviceType\":\"Refrigerador\",\"Description\":null,\"ExpirationDate\":\"2022-10-31T00:00:00\",\"ModelFamily\":null,\"RiskTypeCode\":\"Gadgets\",\"ManufacturerIdentifier\":null,\"SkuNumber\":null,\"SimCardTypeXcd\":null},{\"ImeiNumber\":\"\",\"SerialNumber\":\"\",\"Manufacturer\":\"LG LinBl\",\"Model\":\"LINEA BLANCA +G//Refrigerador\",\"RegisteredItemName\":\"LG.REF.25.ACERO.LM65BGS/RAPTOR2/FD   (1 PZA)\",\"Capacity\":null,\"PurchasePrice\":20000.0,\"Color\":null,\"PurchasedDate\":\"2021-03-01T00:00:00\",\"Carrier\":null,\"DeviceType\":\"Refrigerador\",\"Description\":null,\"ExpirationDate\":\"2022-10-31T00:00:00\",\"ModelFamily\":null,\"RiskTypeCode\":\"Gadgets\",\"ManufacturerIdentifier\":null,\"SkuNumber\":null,\"SimCardTypeXcd\":null},{\"ImeiNumber\":\"\",\"SerialNumber\":\"\",\"Manufacturer\":\"OSTER\",\"Model\":\"Plancha\",\"RegisteredItemName\":\"MSG012208011000000011\",\"Capacity\":null,\"PurchasePrice\":1500.0,\"Color\":null,\"PurchasedDate\":\"2021-03-01T00:00:00\",\"Carrier\":null,\"DeviceType\":\"Electrodoméstico\",\"Description\":null,\"ExpirationDate\":\"2022-10-31T00:00:00\",\"ModelFamily\":null,\"RiskTypeCode\":\"Gadgets\",\"ManufacturerIdentifier\":null,\"SkuNumber\":null,\"SimCardTypeXcd\":null},{\"ImeiNumber\":\"\",\"SerialNumber\":\"\",\"Manufacturer\":\"MAYTAG\",\"Model\":\"Lavadora\",\"RegisteredItemName\":\"MSG012208011000000043\",\"Capacity\":null,\"PurchasePrice\":13500.0,\"Color\":null,\"PurchasedDate\":\"2021-07-31T00:00:00\",\"Carrier\":null,\"DeviceType\":\"Lavadora\",\"Description\":null,\"ExpirationDate\":\"2022-10-31T00:00:00\",\"ModelFamily\":null,\"RiskTypeCode\":\"Gadgets\",\"ManufacturerIdentifier\":null,\"SkuNumber\":null,\"SimCardTypeXcd\":null},{\"ImeiNumber\":\"\",\"SerialNumber\":\"\",\"Manufacturer\":\"LG LinBl\",\"Model\":\"Refrigerador\",\"RegisteredItemName\":\"MSG012208011000000050\",\"Capacity\":null,\"PurchasePrice\":20000.0,\"Color\":null,\"PurchasedDate\":\"2021-07-01T00:00:00\",\"Carrier\":null,\"DeviceType\":\"Refrigerador\",\"Description\":null,\"ExpirationDate\":\"2022-10-31T00:00:00\",\"ModelFamily\":null,\"RiskTypeCode\":\"Gadgets\",\"ManufacturerIdentifier\":null,\"SkuNumber\":null,\"SimCardTypeXcd\":null}],\"CompanyCode\":\"ASM\",\"CaseNumber\":\"2022069727\",\"InteractionNumber\":\"2022108161\",\"ClaimNumber\":null,\"ClaimRecordingMessages\":null,\"Fields\":[],\"CustomerInfo\":{\"CustomerName\":\"GOMEZ ZAMARRIPA SONIA KARINA\",\"EmailAddress\":\"\",\"HomePhone\":\"08100943\",\"WorkPhone\":\"\"},\"MethodOfRepairCode\":null}";
            return data;
        }
    }
}