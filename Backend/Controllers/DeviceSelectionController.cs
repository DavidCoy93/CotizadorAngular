using System.Web.Http;
using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
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
    public class DeviceSelectionController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: DeviceSelection
        /// <summary>
        /// Obtiene el dispositivo
        /// </summary>
        /// <param name="jsonStrin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/deviceselection")]
        public IHttpActionResult GetDevice(DeviceInfo jsonStrin)

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

                    ClaimRecordingServiceWS.BaseClaimRecordingResponse deviceResponse = null;
                    ClaimRecordingServiceWS.QuestionResponse result = null;

                    try
                    {
                        var policyServiceClient = new ClaimRecordingServiceWS.ClaimRecordingServiceClient(Constants.ENPTNAMECLAIM2, Constants.URLCLAIM2);
                        Cliente cl = DBOperations.GetClientesById(jsonStrin.IdCliente); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];

                        List<DealerCodes> dco = DBOperations.GetDealerCodesByDealerCode(jsonStrin.DealerCode);
                        try
                        {
                            var claimdevice = new ClaimRecordingServiceWS.DeviceInfo
                            {
                                Manufacturer = jsonStrin.Manufacturer
                                ,
                                Model = jsonStrin.Model
                                ,
                                Capacity = jsonStrin.Capacity
                                ,
                                Color = jsonStrin.Color
                                ,
                                Carrier = jsonStrin.Carrier
                                ,
                                Description = jsonStrin.Description
                                ,
                                ModelFamily = jsonStrin.ModelFamily
                                ,
                                ManufacturerIdentifier = jsonStrin.ManufacturerIdentifier
                                ,
                                SerialNumber = jsonStrin.SerialNumber
                                ,
                                RegisteredItemName = jsonStrin.RegisteredItemName
                                ,
                                PurchasePrice = jsonStrin.PurchasePrice
                                ,
                                PurchasedDate = Convert.ToDateTime(DateHelper.GetDateString(jsonStrin.PurchasedDate))
                                ,
                                ExpirationDate = Convert.ToDateTime(DateHelper.GetDateString(jsonStrin.ExpirationDate))
                                ,
                                RiskTypeCode = jsonStrin.RiskTypeCode
                            };

                            var SubmitRequest = new ClaimRecordingServiceWS.DeviceSelectionRequest
                            {
                                CompanyCode = dco.First().CompanyCode,
                                CaseNumber = jsonStrin.CaseNumber,
                                InteractionNumber = jsonStrin.InteractionNumber,
                                ClaimedDevice = claimdevice
                            };
                            jsonrequest = JsonConvert.SerializeObject(SubmitRequest);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método DeviceSelection exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            deviceResponse = policyServiceClient.Submit(SubmitRequest);

                            result = (ClaimRecordingServiceWS.QuestionResponse)deviceResponse;
                            var data = result.Questions;

                            DataTable dt = DBOperations.GetQuestionTemplate(cl.IdCliente, result.QuestionSetCode);

                            foreach (var question in data)
                            {
                                string text = (from DataRow myRow in dt.Rows
                                               where myRow.Field<int>("id_q") == question.SequenceNumber
                                               select myRow.Field<string>("text_esp")).FirstOrDefault();

                                question.Text = text ?? question.Text; //.Select("text_esp");
                            }

                            jsonresponse = JsonConvert.SerializeObject(deviceResponse);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            DBOperations.UpdateClaimDetailsDevice(jsonStrin.SerialNumber, jsonStrin.CaseNumber, jsonStrin.Manufacturer + "," + jsonStrin.Model + "," + jsonStrin.Color + "," + jsonStrin.SerialNumber + "," + jsonStrin.Capacity + "," + jsonStrin.Description + "," + jsonStrin.PurchasePrice + "," + jsonStrin.PurchasedDate);
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método DeviceSelection exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
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
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
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
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
                            return Ok(GenerateJsonResponseSimulation());
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
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "No se pudo realizar la operación, Token invalido. AtacchDocument", Plataforma = "MSSP_ELITA" };
                    DBOperations.InsertBitacora(btresp);
                    return BadRequest("{'Error':'Token Invalido'}");
                }
            }
            catch (Exception ex)
            {
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(jsonStrin.IdCliente));
                if (Fa.Enabled == true)
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    DBOperations.UpdateClaimFolioFromDeviceSelection(jsonStrin.SerialNumber != "" ? jsonStrin.SerialNumber : jsonStrin.RegisteredItemName);
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
            string data = "{\"QuestionSetCode\":\"LIVERPOOL_MEX\",\"Version\":1,\"CompanyCode\":\"ASM\",\"CaseNumber\":\"2022083905\",\"SequenceNumber\":1,\"InteractionNumber\":\"2022122349\",\"Questions\":[{\"Code\":\"INIT_DOL\",\"Text\":\"Fecha de la pérdida\\r\\n\",\"AnswerType\":1,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":1,\"Answer\":{\"Answer\":null},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"INIT_DESC\",\"Text\":\"Cuéntanos qué pasó \\r\\n\",\"AnswerType\":2,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":500,\"SequenceNumber\":2,\"Answer\":{\"Answer\":null},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_PHN\",\"Text\":\"Número de teléfono adicional para contacto\\r\\n\",\"AnswerType\":2,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":20,\"SequenceNumber\":3,\"Answer\":{\"Answer\":null},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_POS\",\"Text\":\"¿Cuentas con el equipo en tu poder? \\r\\n\",\"AnswerType\":3,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":4,\"Answer\":{\"Answers\":[{\"Code\":\"LVRPOOL_MEX_INIT_POS_Y\",\"Text\":\"Yes\",\"SequenceNumber\":1},{\"Code\":\"LVRPOOL_MEX_INIT_POS_N\",\"Text\":\"No\",\"SequenceNumber\":2}],\"Answer\":{\"Code\":null,\"Text\":null,\"SequenceNumber\":0}},\"PreConditions\":[],\"Applicable\":true,\"ReEvaulateOnChange\":true,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_DAMAGE\",\"Text\":\"¿El equipo presenta fallas por daño físico?\\r\\n\",\"AnswerType\":3,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":5,\"Answer\":{\"Answers\":[{\"Code\":\"LVRPOOL_MEX_INIT_DAMAGE_Y\",\"Text\":\"Yes\",\"SequenceNumber\":1},{\"Code\":\"LVRPOOL_MEX_INIT_DAMAGE_N\",\"Text\":\"No\",\"SequenceNumber\":2}],\"Answer\":{\"Code\":null,\"Text\":null,\"SequenceNumber\":0}},\"PreConditions\":[{\"PreConditionType\":0,\"QuestionCode\":\"LVRPOOL_MEX_INIT_POS\",\"AnswerCode\":\"LVRPOOL_MEX_INIT_POS_Y\",\"ParameterCode\":null,\"ParameterValue\":\"\"}],\"Applicable\":false,\"ReEvaulateOnChange\":true,\"ChannelDisabled\":false,\"Validations\":[]},{\"Code\":\"LVRPOOL_MEX_INIT_ELECTRIC\",\"Text\":\" ¿El dispositivo ha sufrido una sobrecarga o variación de voltaje? \\r\\n\",\"AnswerType\":3,\"Mandatory\":true,\"Scale\":null,\"Precision\":null,\"Length\":null,\"SequenceNumber\":6,\"Answer\":{\"Answers\":[{\"Code\":\"LVRPOOL_MEX_INIT_ELECTRIC_Y\",\"Text\":\"Yes\",\"SequenceNumber\":1},{\"Code\":\"LVRPOOL_MEX_INIT_ELECTRIC_N\",\"Text\":\"No\",\"SequenceNumber\":2}],\"Answer\":{\"Code\":null,\"Text\":null,\"SequenceNumber\":0}},\"PreConditions\":[{\"PreConditionType\":0,\"QuestionCode\":\"LVRPOOL_MEX_INIT_DAMAGE\",\"AnswerCode\":\"LVRPOOL_MEX_INIT_DAMAGE_N\",\"ParameterCode\":null,\"ParameterValue\":\"\"}],\"Applicable\":false,\"ReEvaulateOnChange\":false,\"ChannelDisabled\":false,\"Validations\":[]}],\"ClaimNumber\":\"\",\"Parameters\":[],\"ClaimRecordingMessages\":null,\"Fields\":[],\"CustomerInfo\":{\"CustomerName\":\"GOMEZ ZAMARRIPA SONIA KARINA\",\"EmailAddress\":\"\",\"HomePhone\":\"08100943\",\"WorkPhone\":\"\\r\"},\"MethodOfRepairCode\":null}";
            return data;
        }
    }
}