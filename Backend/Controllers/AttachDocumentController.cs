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
using System.Globalization;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class AttachDocumentController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Agrega el document en base 64
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        // GET: AddDocument
        //api/document/0000001300000445416769
        //[RequestSizeLimit()]
        [HttpPost]
        [Route("api/document")]
        public IHttpActionResult getDocument(dynamic documentReq)
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                string scanDateString = DateTime.Now.ToString("dd/MM/yyyy");
                if (DBOperations.GetToken(msspt))
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    string jsonresponse = string.Empty;
                    string jsonrequest = string.Empty;
                    string jsonConfig = string.Empty;

                    try
                    {
                        var randomNumber = new Random().Next(1000, 9999);
                        char[] separate = new char[] { '.' };
                        string[] subs = documentReq.FileName.Value.Split(separate);
                        documentReq.FileName.Value = subs[0] + "_" + randomNumber + "." + subs[1];
                        if (subs.Length > 2)
                        {
                            for (int i = 2; i < subs.Length; i++)
                            {
                                documentReq.FileName.Value = documentReq.FileName.Value + "." + subs[i];
                            }
                        }
                        var policyServiceClient = new CDS.CertificateDocumentServiceClient(Constants.ENPTNAME, Constants.URLDOCUMENT);
                        policyServiceClient.ClientCredentials.UserName.UserName = Constants.USERNAMEA1A2;
                        policyServiceClient.ClientCredentials.UserName.Password = Constants.PSWDA1A2;
                        Cliente cl = DBOperations.GetClientesById(Int32.Parse(documentReq.idCliente.Value)); //Constantes cuando haya mas clientes
                        List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(Int32.Parse(documentReq.idCliente.Value));
                        CDS.AttachCertificateDocumentResponse attachCertificateDocumenthResponse = null;
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        try
                        {
                            var search = new CDS.CertificateNumberLookup
                            {
                                DealerCode = dc.First().DealerCode
                                ,
                                CertificateNumber = documentReq.Certificate.Value
                            };

                            var document = new CDS.AttachCertificateDocumentRequest
                            {
                                CertificateSearch = search
                                ,
                                Comments = documentReq.Comments.Value
                                ,
                                DocumentType = null
                                ,
                                FileName = documentReq.FileName.Value
                                ,
                                ImageData = System.Convert.FromBase64String(documentReq.ImageData.Value)
                                ,
                                ScanDate = Convert.ToDateTime(DateHelper.GetDateString(DateTime.Now))
                                ,
                                UserName = documentReq.UserName.Value
                            };
                            jsonrequest = JsonConvert.SerializeObject(document);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método AttachDocument exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            attachCertificateDocumenthResponse = policyServiceClient.AttachDocument(document);
                            jsonresponse = JsonConvert.SerializeObject(attachCertificateDocumenthResponse);
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método AttachDocument exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
                                return Ok(GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                return BadRequest();
                            }
                        }
                        log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                        return Ok(jsonresponse);
                    }
                    catch (Exception ex)
                    {
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
                            return Ok(GenerateJsonResponseSimulation());
                        }
                        else
                        {
                            log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            return BadRequest();
                        }
                    }
                }
                else
                {
                    log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. AtacchDocument", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(documentReq.idCliente.Value));
                if (Fa.Enabled == true)
                {
                    log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                    DBOperations.UpdateClaimFolioFromAttachDocument(documentReq.ImageData.Value);
                    return Ok(GenerateJsonResponseSimulation());
                }
                else
                {
                    log.Error(string.Format("Error - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
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
            string data = "{\"ErrorCode\":\"0\",\"ErrorMessage\":\"null\",\"PolicyResponse\":[{\"CertificateNumber\":\"1\",\"CustomerName\":\"GOMEZ ZAMARRIPA SONIA KARINA\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"HomePhone\":\"08100943\",\"IdentificationNumber\":\"GOZS7003096M2\",\"ImeiNumber\":\"\",\"ItemDescription\":\"04 PIF PLUS\",\"Carrier\":null,\"ItemEffectiveDate\":\"2020-03-01T00:00:00\",\"Manufacturer\":\"\",\"Model\":\"\",\"ProductSalesDate\":\"2020-03-01T00:00:00\",\"ProductDescription\":\"Liverpool PIF\",\"ProductCode\":\"LVPIF\",\"SerialNumber\":\"\",\"ServiceLineNumber\":\"\",\"SKUNumber\":\"\",\"Statuscode\":\"A\",\"WarrantySalesDate\":\"2020-03-01T00:00:00\",\"WorkPhone\":\"\",\"MasterCertificateNumber\":\"\",\"PromoCodes\":\"\",\"ActivationDate\":\"2020-03-01T00:00:00\"},{\"CertificateNumber\":\"1\",\"CustomerName\":\"RAMIREZ MILLAN PERLA\",\"DealerCode\":\"EA13\",\"DealerGroup\":\"LIV\",\"HomePhone\":\"5554710406\",\"IdentificationNumber\":\"1\",\"ImeiNumber\":\"\",\"ItemDescription\":\"PS4 DS4 Midnight Blue\",\"Carrier\":null,\"ItemEffectiveDate\":\"2020-03-01T00:00:00\",\"Manufacturer\":\"MICROSOFT\",\"Model\":\"\",\"ProductSalesDate\":\"2020-03-01T00:00:00\",\"ProductDescription\":\"Liverpool PIF\",\"ProductCode\":\"LVPIF\",\"SerialNumber\":\"\",\"ServiceLineNumber\":\"\",\"SKUNumber\":\"\",\"Statuscode\":\"A\",\"WarrantySalesDate\":\"2020-03-01T00:00:00\",\"WorkPhone\":\"\",\"MasterCertificateNumber\":\"\",\"PromoCodes\":\"\",\"ActivationDate\":\"2020-03-01T00:00:00\"}]}";
            return data;
        }
    }
}