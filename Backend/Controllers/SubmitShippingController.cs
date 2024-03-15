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
    public class SubmitShippingController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el envío
        /// </summary>
        /// <returns></returns>
        // GET: SubmitShipping
        [HttpPost]
        [Route("api/shipping")]
        public IHttpActionResult GetShipping(submitShippingItem jsonString)
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

                    //MSSPElita3.ShippingAddressResponse shippingResponse = null;
                    ClaimRecordingServiceWS.BaseClaimRecordingResponse shippingResponse = null;
                    ClaimRecordingServiceWS.ShippingAddressRequest shippingRequest = null;
                    ClaimRecordingServiceWS.AddressInfo address;
                    Cliente cl = DBOperations.GetClientesById((int)jsonString.IdCliente);
                    dynamic country = JsonConvert.DeserializeObject(cl.Configuraciones);
                    jsonConfig = cl.Configuraciones;
                    dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                    string countr = country["Country"];
                    //Constantes cuando haya mas clientes

                    List<DealerCodes> dco = DBOperations.GetDealerCodesByDealerCode(jsonString.DealerCode);
                    try
                    {
                        var policyServiceClient = new ClaimRecordingServiceWS.ClaimRecordingServiceClient(Constants.ENPTNAMECLAIM2, Constants.URLCLAIM2);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];
                        
                       
                        try
                        {
                            address = new ClaimRecordingServiceWS.AddressInfo
                            {
                                Address1 = jsonString.Address1,
                                Address2 = jsonString.Address2,
                                Address3 = jsonString.Address3,
                                City = jsonString.City,
                                State = jsonString.State,
                                Country = countr,
                                PostalCode = jsonString.PostalCode
                            };

                            shippingRequest = new ClaimRecordingServiceWS.ShippingAddressRequest
                            {
                                CompanyCode = dco.First().CompanyCode,
                                CaseNumber = jsonString.CaseNumber,
                                InteractionNumber = jsonString.InteractionNumber,
                                ShippingAddress = address,
                                FirstName = jsonString.FirstName,
                                LastName = jsonString.LastName,
                                EmailAddress = jsonString.EmailAddress,
                                PhoneNumber = jsonString.PhoneNumber,
                                
                               
                            };
                            jsonrequest = JsonConvert.SerializeObject(shippingRequest);
                            shippingResponse = policyServiceClient.Submit(shippingRequest);
                            jsonresponse = JsonConvert.SerializeObject(shippingResponse);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método SubmitShipping exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;

                            dynamic jrponse = JsonConvert.DeserializeObject(jsonresponse);

                            DBOperations.UpdateClaimDetailsShipping(jsonString.CaseNumber, jsonString.Address1 + "," + jsonString.Address2 + "," + jsonString.City + "," + jsonString.State + "," + jsonString.Country + "," + jsonString.PostalCode);
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método SubmitShipping exitosamente ." + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                                log.Info(string.Format("Se generó el folio: "+ cfolio.Folio+" - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                                DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                                ClaimFolio cf = DBOperations.GetLastClaimFolio();

                                if (ValidateB64(cf.Mail) != true)
                                    EncDec.Encript(cf.Mail);
                                if (ValidateB64(cf.Nombre) != true)
                                    EncDec.Encript(cf.Nombre);
                                if (ValidateB64(cf.Direccion) != true)
                                    EncDec.Encript(cf.Direccion);
                                if (ValidateB64(cf.CP) != true)
                                    EncDec.Encript(cf.CP);
                                if (ValidateB64(cf.Delegacion) != true)
                                    EncDec.Encript(cf.Delegacion);
                                if (ValidateB64(cf.Estado) != true)
                                    EncDec.Encript(cf.Estado);
                                if (ValidateB64(cf.Telefono) != true)
                                    EncDec.Encript(cf.Telefono);

                                MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                                MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
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
                                ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                                log.Info(string.Format("Se generó el folio: "+ cfolio.Folio+" - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                                DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                                ClaimFolio cf = DBOperations.GetLastClaimFolio();

                                if (ValidateB64(cf.Mail) != true)
                                    EncDec.Encript(cf.Mail);
                                if (ValidateB64(cf.Nombre) != true)
                                    EncDec.Encript(cf.Nombre);
                                if (ValidateB64(cf.Direccion) != true)
                                    EncDec.Encript(cf.Direccion);
                                if (ValidateB64(cf.CP) != true)
                                    EncDec.Encript(cf.CP);
                                if (ValidateB64(cf.Delegacion) != true)
                                    EncDec.Encript(cf.Delegacion);
                                if (ValidateB64(cf.Estado) != true)
                                    EncDec.Encript(cf.Estado);
                                if (ValidateB64(cf.Telefono) != true)
                                    EncDec.Encript(cf.Telefono);

                                MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                                MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
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
                                ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                                log.Info(string.Format("Se generó el folio: " + cfolio.Folio + " - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                                DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                                ClaimFolio cf = DBOperations.GetLastClaimFolio();

                                if (ValidateB64(cf.Mail) != true)
                                    EncDec.Encript(cf.Mail);
                                if (ValidateB64(cf.Nombre) != true)
                                    EncDec.Encript(cf.Nombre);
                                if (ValidateB64(cf.Direccion) != true)
                                    EncDec.Encript(cf.Direccion);
                                if (ValidateB64(cf.CP) != true)
                                    EncDec.Encript(cf.CP);
                                if (ValidateB64(cf.Delegacion) != true)
                                    EncDec.Encript(cf.Delegacion);
                                if (ValidateB64(cf.Estado) != true)
                                    EncDec.Encript(cf.Estado);
                                if (ValidateB64(cf.Telefono) != true)
                                    EncDec.Encript(cf.Telefono);

                                MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                                MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
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
                                ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                                log.Info(string.Format("Se generó el folio: " + cfolio.Folio + " - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                                DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                                ClaimFolio cf = DBOperations.GetLastClaimFolio();

                                if (ValidateB64(cf.Mail) != true)
                                    EncDec.Encript(cf.Mail);
                                if (ValidateB64(cf.Nombre) != true)
                                    EncDec.Encript(cf.Nombre);
                                if (ValidateB64(cf.Direccion) != true)
                                    EncDec.Encript(cf.Direccion);
                                if (ValidateB64(cf.CP) != true)
                                    EncDec.Encript(cf.CP);
                                if (ValidateB64(cf.Delegacion) != true)
                                    EncDec.Encript(cf.Delegacion);
                                if (ValidateB64(cf.Estado) != true)
                                    EncDec.Encript(cf.Estado);
                                if (ValidateB64(cf.Telefono) != true)
                                    EncDec.Encript(cf.Telefono);

                                MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                                MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
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
                                ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                                log.Info(string.Format("Se generó el folio: " + cfolio.Folio + " - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                                DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                                ClaimFolio cf = DBOperations.GetLastClaimFolio();

                                if (ValidateB64(cf.Mail) != true)
                                    EncDec.Encript(cf.Mail);
                                if (ValidateB64(cf.Nombre) != true)
                                    EncDec.Encript(cf.Nombre);
                                if (ValidateB64(cf.Direccion) != true)
                                    EncDec.Encript(cf.Direccion);
                                if (ValidateB64(cf.CP) != true)
                                    EncDec.Encript(cf.CP);
                                if (ValidateB64(cf.Delegacion) != true)
                                    EncDec.Encript(cf.Delegacion);
                                if (ValidateB64(cf.Estado) != true)
                                    EncDec.Encript(cf.Estado);
                                if (ValidateB64(cf.Telefono) != true)
                                    EncDec.Encript(cf.Telefono);

                                MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                                MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
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
                            ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                            log.Info(string.Format("Se generó el folio: " + cfolio.Folio + " - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                            DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                            ClaimFolio cf = DBOperations.GetLastClaimFolio();

                            if (ValidateB64(cf.Mail) != true)
                                EncDec.Encript(cf.Mail);
                            if (ValidateB64(cf.Nombre) != true)
                                EncDec.Encript(cf.Nombre);
                            if (ValidateB64(cf.Direccion) != true)
                                EncDec.Encript(cf.Direccion);
                            if (ValidateB64(cf.CP) != true)
                                EncDec.Encript(cf.CP);
                            if (ValidateB64(cf.Delegacion) != true)
                                EncDec.Encript(cf.Delegacion);
                            if (ValidateB64(cf.Estado) != true)
                                EncDec.Encript(cf.Estado);
                            if (ValidateB64(cf.Telefono) != true)
                                EncDec.Encript(cf.Telefono);

                            MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                            MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
                            return Ok(GenerateJsonResponseSimulation());
                        }
                        else
                        {
                            log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            return BadRequest();
                        }
                    }
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", "Error al obtener token"), null);
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. SubmitShipping", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonString.IdCliente));
                if (Fa.Enabled == true)
                {
                    ClaimFolio cfolio = DBOperations.GetLastClaimFolio();
                    log.Info(string.Format("Se generó el folio: " + cfolio.Folio + " - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    string art = ClaimFolioHelper.getArticuloBySessionId((jsonString.SessionId), cfolio.Articulo);
                    DBOperations.UpdateClaimFolioFromSubmitShipping(jsonString.Address1, jsonString.City, jsonString.State, jsonString.PostalCode, jsonString.FirstName, jsonString.LastName, jsonString.EmailAddress, jsonString.PhoneNumber, art);
                    ClaimFolio cf = DBOperations.GetLastClaimFolio();

                    if (ValidateB64(cf.Mail) != true)
                        EncDec.Encript(cf.Mail);
                    if (ValidateB64(cf.Nombre) != true)
                        EncDec.Encript(cf.Nombre);
                    if (ValidateB64(cf.Direccion) != true)
                        EncDec.Encript(cf.Direccion);
                    if (ValidateB64(cf.CP) != true)
                        EncDec.Encript(cf.CP);
                    if (ValidateB64(cf.Delegacion) != true)
                        EncDec.Encript(cf.Delegacion);
                    if (ValidateB64(cf.Estado) != true)
                        EncDec.Encript(cf.Estado);
                    if (ValidateB64(cf.Telefono) != true)
                        EncDec.Encript(cf.Telefono);

                    MailHelper.SendEmailAlternoComercial("NEWCLIENTE", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio, cf.Articulo, EncDec.Decript(cf.Direccion), EncDec.Decript(cf.CP), EncDec.Decript(cf.Delegacion), EncDec.Decript(cf.Estado), EncDec.Decript(cf.Telefono), cf.Poliza, cf.Preguntas, cf.Documento, cf.ClaimNumber, cf.Certificado);
                    MailHelper.SendEmailAlternoCliente("NEWCLIENTES", Convert.ToInt32(jsonString.IdCliente), EncDec.Decript(cf.Mail), EncDec.Decript(cf.Nombre), cf.Folio);
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
            ClaimFolio folio = DBOperations.GetLastClaimFolio();
            //string data = "{\"QuestionSetCode\":\"LIVERPOOL_MEX\",\"Version\":1,\"CompanyCode\":\"ASM\",\"CaseNumber\":\"2022003676\",\"SequenceNumber\":1,\"InteractionNumber\":\"2022015466\",\"Questions\":[{\"Code\":\"INIT_DOL\",\"Text\":\"Date of the loss\",\"AnswerType\":1,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":1,\"Answer\":{\"Answer\":null},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"INIT_DESC\",\"Text\":\"Tell us what happened\",\"AnswerType\":2,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":500,\"SequenceNumber\":2,\"Answer\":{\"Answer\":null},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_PHN\",\"Text\":\"Additional phone number for contact\",\"AnswerType\":2,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":20,\"SequenceNumber\":3,\"Answer\":{\"Answer\":null},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_POS\",\"Text\":\"Do you have possession of the device?\",\"AnswerType\":3,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":4,\"Answer\":{\"Answers\":[{\"Code\":\"LVRPOOL_MEX_INIT_POS_Y\",\"Text\":\"Yes\",\"SequenceNumber\":1},{\"Code\":\"LVRPOOL_MEX_INIT_POS_N\",\"Text\":\"No\",\"SequenceNumber\":2}],\"Answer\":{\"Code\":null,\"Text\":null,\"SequenceNumber\":0}},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":true,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_DAMAGE\",\"Text\":\"Has the product failed due to physical damage?\",\"AnswerType\":3,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":5,\"Answer\":{\"Answers\":[{\"Code\":\"LVRPOOL_MEX_INIT_DAMAGE_Y\",\"Text\":\"Yes\",\"SequenceNumber\":1},{\"Code\":\"LVRPOOL_MEX_INIT_DAMAGE_N\",\"Text\":\"No\",\"SequenceNumber\":2}],\"Answer\":{\"Code\":null,\"Text\":null,\"SequenceNumber\":0}},\"PreConditions\":[{\"PreConditionType\":0,\"QuestionCode\":\"LVRPOOL_MEX_INIT_POS\",\"AnswerCode\":\"LVRPOOL_MEX_INIT_POS_Y\",\"ParameterCode\":null,\"ParameterValue\":\"\"}],\"Applicable\":false,\"ReEvaulateOnChange\":true,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_ELECTRIC\",\"Text\":\"Has the device suffered an overload or voltage variation?\",\"AnswerType\":3,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":6,\"Answer\":{\"Answers\":[{\"Code\":\"LVRPOOL_MEX_INIT_ELECTRIC_Y\",\"Text\":\"Yes\",\"SequenceNumber\":1},{\"Code\":\"LVRPOOL_MEX_INIT_ELECTRIC_N\",\"Text\":\"No\",\"SequenceNumber\":2}],\"Answer\":{\"Code\":null,\"Text\":null,\"SequenceNumber\":0}},\"PreConditions\":[{\"PreConditionType\":0,\"QuestionCode\":\"LVRPOOL_MEX_INIT_DAMAGE\",\"AnswerCode\":\"LVRPOOL_MEX_INIT_DAMAGE_N\",\"ParameterCode\":null,\"ParameterValue\":\"\"}],\"Applicable\":false,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]}],\"Parameters\":[],\"ClaimRecordingMessages\":null,\"Fields\":[],\"CustomerInfo\":{\"CustomerName\":\"LIVERPOOLFIRSTNAME 8MIDDLENAME 8LASTNAME\",\"EmailAddress\":\"venugopal.teepireddy@assurant.com\",\"HomePhone\":\"9234567899\",\"WorkPhone\":\"9234560099\"},\"MethodOfRepairCode\":null,\"DecisionType\":\"5\",\"ClaimStatus\":\"A\", \"ClaimNumber\":\"" + folio.Folio + "\"}";
            string data = "{\"QuestionSetCode\": \"LIVERPOOL_MEX\",\"Version\": 1, \"CompanyCode\": \"ASM\",\"CaseNumber\": \"2022069690\",\"SequenceNumber\": 1, \"InteractionNumber\": \"2022108124\",\"Questions\": [{ \"Code\": \"INIT_DOL\", \"Text\": \"¿Cuál es la fecha de ocurrencia del siniestro?\",  \"AnswerType\": 1,\"Mandatory\": true, \"Scale\": null, \"Precision\": null,\"Length\": null, \"SequenceNumber\": 1, \"Answer\": { \"Answer\": \"2022-09-02T00:00:00\"  },\"PreConditions\": [],\"Applicable\": true,\"ReEvaulateOnChange\": false,\"ChannelDisabled\": false,\"Validations\": []},{\"Code\": \"INIT_DESC\",\"Text\": \"Describe brevemente lo que sucedió\",\"AnswerType\": 2,\"Mandatory\": true,\"Scale\": null,\"Precision\": null,\"Length\": 500,\"SequenceNumber\": 2,\"Answer\": {\"Answer\": \"Prueba\"},\"PreConditions\": [],\"Applicable\": true,\"ReEvaulateOnChange\": false,\"ChannelDisabled\": false,\"Validations\": []},{\"Code\": \"LVRPOOL_MEX_INIT_PHN\",\"Text\": \"Proporciona un número telefónico adicional de contacto\",\"AnswerType\": 2,\"Mandatory\": true,\"Scale\": null,\"Precision\": null,\"Length\": 20,\"SequenceNumber\": 3,\"Answer\": {\"Answer\": \"3432432432\"},\"PreConditions\": [],\"Applicable\": true,\"ReEvaulateOnChange\": false,\"ChannelDisabled\": false,\"Validations\": []},{\"Code\": \"LVRPOOL_MEX_INIT_POS\",\"Text\": \"¿Tienes el equipo en tu poder?\",\"AnswerType\": 3,\"Mandatory\": true,\"Scale\": null,\"Precision\": null,\"Length\": null,\"SequenceNumber\": 4,\"Answer\": {\"Answers\": [{\"Code\": \"LVRPOOL_MEX_INIT_POS_Y\",\"Text\": \"Sí\",\"SequenceNumber\": 1},{\"Code\": \"LVRPOOL_MEX_INIT_POS_N\",\"Text\": \"No\",\"SequenceNumber\": 2}],\"Answer\": {\"Code\": \"LVRPOOL_MEX_INIT_POS_Y\",\"Text\": \"Sí\",\"SequenceNumber\": 1}},\"PreConditions\": [],\"Applicable\": true,\"ReEvaulateOnChange\": true,\"ChannelDisabled\": false,\"Validations\": []},{\"Code\": \"LVRPOOL_MEX_INIT_DAMAGE\",\"Text\": \"¿El producto presenta fallas por un daño físico?\",\"AnswerType\": 3,\"Mandatory\": true,\"Scale\": null,\"Precision\": null,\"Length\": null,\"SequenceNumber\": 5,\"Answer\": {\"Answers\": [{\"Code\": \"LVRPOOL_MEX_INIT_DAMAGE_Y\",\"Text\": \"Sí\",\"SequenceNumber\": 1},{\"Code\": \"LVRPOOL_MEX_INIT_DAMAGE_N\",\"Text\": \"No\",\"SequenceNumber\": 2}],\"Answer\": {\"Code\": \"LVRPOOL_MEX_INIT_DAMAGE_N\",\"Text\": \"No\",\"SequenceNumber\": 0}},\"PreConditions\": [{\"PreConditionType\": 0,\"QuestionCode\": \"LVRPOOL_MEX_INIT_POS\",\"AnswerCode\": \"LVRPOOL_MEX_INIT_POS_Y\",\"ParameterCode\": null,\"ParameterValue\": \"\"}],\"Applicable\": true,\"ReEvaulateOnChange\": true,\"ChannelDisabled\": false,\"Validations\": []},{\"Code\": \"LVRPOOL_MEX_INIT_ELECTRIC\",\"Text\": \"¿El dispositivo estuvo expuesto a sobrecarga o variación de voltaje?\",\"AnswerType\": 3,\"Mandatory\": true,\"Scale\": null,\"Precision\": null,\"Length\": null,\"SequenceNumber\": 6,\"Answer\": {\"Answers\": [{\"Code\": \"LVRPOOL_MEX_INIT_ELECTRIC_Y\",\"Text\": \"Sí\",\"SequenceNumber\": 1},{\"Code\": \"LVRPOOL_MEX_INIT_ELECTRIC_N\",\"Text\": \"No\",\"SequenceNumber\": 2}],\"Answer\": {\"Code\": \"LVRPOOL_MEX_INIT_ELECTRIC_N\",\"Text\": \"No\",\"SequenceNumber\": 0}},\"PreConditions\": [{\"PreConditionType\": 0,\"QuestionCode\": \"LVRPOOL_MEX_INIT_DAMAGE\",\"AnswerCode\": \"LVRPOOL_MEX_INIT_DAMAGE_N\",\"ParameterCode\": null,\"ParameterValue\": \"\"}],\"Applicable\": false,\"ReEvaulateOnChange\": false,\"ChannelDisabled\": false,\"Validations\": []}],\"Parameters\": [],\"ClaimRecordingMessages\": null,\"Fields\": [],\"CustomerInfo\": {\"CustomerName\": \"GOMEZ Z ROBERTO\",\"EmailAddress\": \"\",\"HomePhone\": \"02270000\",\"WorkPhone\": \"\"},\"MethodOfRepairCode\": null, \"DecisionType\":\"1\",\"ClaimStatus\":\"A\", \"ClaimNumber\":\"" + folio.Folio + "\"}";
            return data;
        }
    }
}