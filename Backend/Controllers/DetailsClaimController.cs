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
using System.Data;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class DetailsClaimController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene los detalles del reclamo
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/claim")]
        public IHttpActionResult GetDetailsClaim(DetailsClaim jsonString)
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").FirstOrDefault()/*.Split(',')[0]*/;
                if (DBOperations.GetToken(msspt))
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    string jsonresponse = string.Empty;
                    string jsonrequest = string.Empty;
                    string jsonConfig = string.Empty;

                    ClaimServiceWS.GetClaimDetailsResponse searchResponse = null;
                    List<DealerCodes> dco = DBOperations.GetDealerCodesByDealerCode(jsonString.DealerCode);
                    try
                    {
                        var policyServiceClient = new ClaimServiceWS.ClaimServiceClient(Constants.ENPTNAMECLAIM, Constants.URLCLAIM3);
                        Cliente cl = DBOperations.GetClientesById(jsonString.IdCliente); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];

                        try
                        {
                            var claimrequest = new ClaimServiceWS.GetClaimDetailsRequest
                            {
                                CompanyCode = jsonString.CompanyCode
                                ,
                                ClaimNumber = jsonString.ClaimNumber
                                ,
                                CultureCode = jsonObj["CultureCode"]
                                ,
                                ClaimDetailsRequest = new ClaimServiceWS.ClaimDetailType[2]
                            };
                            jsonrequest = JsonConvert.SerializeObject(claimrequest);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = DateTime.Now, Usuario = Environment.UserName, Descripcion = "Request a método DetailsClaim exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            searchResponse = policyServiceClient.GetDetails(claimrequest);

                            var data = searchResponse.ExtendedStatus;

                            DataTable dt = DBOperations.GetCodeTraslate(cl.IdCliente);

                            foreach (var tracking in data)
                            {
                                string text = (from DataRow myRow in dt.Rows
                                               where myRow.Field<string>("Code") == tracking.StatusCode
                                               select myRow.Field<string>("text_esp_short")).FirstOrDefault();

                                tracking.StatusDescription = text ?? tracking.StatusDescription; //.Select("text_esp");
                            }

                            jsonresponse = JsonConvert.SerializeObject(searchResponse);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            dynamic jObj = JsonConvert.DeserializeObject(jsonresponse);
                            List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(jsonString.IdCliente);
                            //int dd = Convert.ToInt32(jObj["ClaimNumber"].Value);
                            ClaimDetails cds = DBOperations.GetOneClaimDetails(jObj["ClaimNumber"].Value);//Antes de enviar correo por flujo normal consulta los detalles
                            if(cds != null)
                            {
                                if (cds.RespInfoCLaimDetails == "" || cds.RespInfoCLaimDetails == null)
                                {
                                    if (ValidateB64(cds.CorreoCliente) != true)
                                        EncDec.Encript(cds.CorreoCliente);
                                    if (ValidateB64(cds.NombreCliente) != true)
                                        EncDec.Encript(cds.NombreCliente);

                                    MailHelper.SendEmail("New", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cds.CorreoCliente), EncDec.Decript(cds.NombreCliente), jObj["ClaimNumber"].Value);
                                }
                                    
                            }
                            var result = jObj["ExtendedStatus"].ToString();

                            if (result != "[]")

                                DBOperations.UpdateClaimDetails(jObj["ClaimStatus"].Value, jObj["ExtendedStatus"][0]["StatusCode"].Value, jObj["ClaimNumber"].Value);
                            else
                                DBOperations.UpdateClaimDetails(jObj["ClaimStatus"].Value, jObj["ExtendedStatus"].ToString(), jObj["ClaimNumber"].Value);
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            log.Info(string.Format("Info Se ha creado el claim: " + jObj["ClaimNumber"].Value + " - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método DetailsClaim exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                            ClaimData cda =  DBOperations.GetCertificateClaim(jObj["CertificateNumber"].Value);
                            if (cda != null && cda.IdClaim != 0)
                                DBOperations.UpdateClaimData(cda.IdClaim,jObj["CertificateNumber"].Value, jsonString.IdCliente, jsonresponse, cda.ClaimHistoryList + jsonresponse, true, dc.First().DealerCode);
                            else
                                DBOperations.InsertClaimData(jObj["CertificateNumber"].Value, jsonString.IdCliente, jsonresponse, jsonresponse, true, dc.First().DealerCode);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
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
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = DateTime.Now, Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. DetailsClaim", Plataforma = "MSSP_ELITA" };
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
                    return Ok(GenerateJsonResponseSimulation());
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    return BadRequest();
                }
            }
        }

        public static bool ValidateB64(string cadena)
        {
            try
            {
                if (!string.IsNullOrEmpty(cadena))
                {
                    byte[] data = Convert.FromBase64String(cadena);
                    return true;
                }
                else
                {
                    cadena = "";
                    return true;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GenerateJsonResponseSimulation()
        {
            string data = "{\"CallerName\":\"GABRIELA INES SOTO MORA\",\"CertificateNumber\":\"1111200320040024151A\",\"ClaimNumber\":\"22038402\",\"ClaimStatus\":\"A\",\"Comments\":[],\"CoverageType\":\"E\"}";
            return data;
        }
    }
}