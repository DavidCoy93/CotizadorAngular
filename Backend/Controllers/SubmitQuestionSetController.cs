using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using MSSPAPI.ClaimRecordingServiceWS;
using System.Collections;
using System.Linq;
using System.Data;
using System.Globalization;
using MSSPAPI.ClaimServiceWS;
using Microsoft.Ajax.Utilities;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class SubmitQuestionSetController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene la pregunta de Elita
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        // GET: Question
        [HttpPost]
        [Route("api/question")]
        public IHttpActionResult GetQuestion(dynamic jsonString)
        {
            var hreq = this.Request.Headers;
            string msspt_idcliente = hreq.GetValues("idcliente").First();
            try
            {
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                    log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                    string jsonresponse = string.Empty;
                    string jsonrequest = string.Empty;
                    string jsonConfig = string.Empty;

                    ClaimRecordingServiceWS.BaseClaimRecordingResponse questionResponse = null;
                    ClaimRecordingServiceWS.QuestionRequest questionRequest = null;
                    //ClaimRecordingServiceWS.BaseAnswer q = null;
                    try
                    {
                        var policyServiceClient = new ClaimRecordingServiceWS.ClaimRecordingServiceClient(Constants.ENPTNAMECLAIM2, Constants.URLCLAIM2);
                        Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(msspt_idcliente)); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        policyServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        policyServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];

                        List<BaseAnswer> qs = new List<BaseAnswer>();
                        List<Question> qstn = new List<Question>();
                        try
                        {
                            int loop = 0;
                            foreach (var item in jsonString.Questions)
                            {
                                Question newquestion = null;
                                int type = Convert.ToInt32(item.AnswerType);
                                switch (type)
                                {
                                    case 0:
                                        List<AnswerValue> av0 = new List<AnswerValue>();

                                        foreach (var itm in jsonString.Questions[loop]["Answer"]["Answers"])
                                        {
                                            var seq = VerifyIfNull(itm.SequenceNumber.Value);
                                            AnswerValue var3 = new AnswerValue { Code = itm.Code.Value, SequenceNumber = seq, Text = itm.Text.Value };
                                            av0.Add(var3);
                                        }
                                        int seqAns;
                                        var codeAns = jsonString.Questions[loop]["Answer"]["Answer"].Code.Value;
                                        var textAns = jsonString.Questions[loop]["Answer"]["Answer"].Text.Value;

                                        if (jsonString.Questions[loop]["Answer"]["Answer"].Code.Value == null)
                                        {
                                            seqAns = (int)av0[1].SequenceNumber;
                                            codeAns = (string)av0[1].Code;
                                            textAns = (string)av0[1].Text;
                                        }
                                        else
                                        {
                                            seqAns = (int)jsonString.Questions[loop]["Answer"]["Answer"].SequenceNumber.Value;
                                        }                                        
                                        var asnLst = new AnswerValue { Code = codeAns, SequenceNumber = seqAns, Text = textAns };

                                    

                                        ChoiceAnswer var0 = new ChoiceAnswer { Answer = asnLst, Answers = av0.ToArray() };
                                        //qs.Add(vara);

                                        List<BaseValidation> basevalidation4 = new List<BaseValidation>();
                                        foreach (var itm in jsonString.Questions[loop].Validations)
                                        {
                                            BaseValidation varv = new BaseValidation { ExtensionData = itm.ExtensionData.Value };
                                            basevalidation4.Add(varv);
                                        }

                                        List<PreCondition> precondition4 = new List<PreCondition>();
                                        foreach (var itm in jsonString.Questions[loop].PreConditions)
                                        {
                                            var pre = VerifyIfNull(itm.PreConditionType.Value);
                                            PreCondition var3 = new PreCondition { AnswerCode = itm.AnswerCode.Value, ParameterCode = itm.ParameterCode.Value, ParameterValue = itm.ParameterValue.Value, PreConditionType = (PreConditionTypes)itm.PreConditionType.Value, QuestionCode = itm.QuestionCode.Value };
                                            precondition4.Add(var3);
                                        }

                                        var scale3 = VerifyIfNull(jsonString.Questions[loop].Scale.Value);
                                        var lenght3 = VerifyIfNull(jsonString.Questions[loop].Length.Value);
                                        var sequence3 = VerifyIfNull(jsonString.Questions[loop].SequenceNumber.Value);
                                        var precision3 = VerifyIfNull(jsonString.Questions[loop].Precision.Value);

                                        newquestion = new Question
                                        {
                                            AnswerType = (AnswerType)jsonString.Questions[loop].AnswerType.Value,
                                            Answer = var0,
                                            Applicable = jsonString.Questions[loop].Applicable.Value,
                                            ChannelDisabled = jsonString.Questions[loop].ChannelDisabled.Value,
                                            Code = jsonString.Questions[loop].Code.Value,
                                            Length = lenght3,
                                            Mandatory = jsonString.Questions[loop].Mandatory.Value,
                                            Precision = precision3,
                                            PreConditions = precondition4.ToArray(),
                                            ReEvaulateOnChange = jsonString.Questions[loop].ReEvaulateOnChange.Value,
                                            Scale = scale3,
                                            SequenceNumber = sequence3,
                                            Text = jsonString.Questions[loop].Text.Value,
                                            Validations = basevalidation4.ToArray()
                                        };


                                        break;

                                    case 1:
                                        string j = jsonString.Questions[loop]["Answer"]["Answer"];
                                        DateAnswer da = new DateAnswer { Answer = Convert.ToDateTime(DateHelper.GetDateString(DateTime.ParseExact(j, "dd/MM/yyyy", CultureInfo.InvariantCulture))) };

                                        List<BaseValidation> basevalidation = new List<BaseValidation>();
                                        foreach (var itm in jsonString.Questions[loop].Validations)
                                        {
                                            BaseValidation var3 = new BaseValidation { ExtensionData = itm.ExtensionData };
                                            basevalidation.Add(var3);
                                        }

                                        List<PreCondition> precondition = new List<PreCondition>();
                                        foreach (var itm in jsonString.Questions[loop].PreConditions)
                                        {
                                            PreCondition var3 = new PreCondition { AnswerCode = itm.AnswerCode.Value, ParameterCode = itm.ParameterCode.Value, ParameterValue = itm.ParameterValue.Value, PreConditionType = (PreConditionTypes)itm.PreConditionType.Value, QuestionCode = itm.QuestionCode.Value };
                                            precondition.Add(var3);
                                        }

                                        var scale = VerifyIfNull(jsonString.Questions[loop].Scale.Value);
                                        var lenght = VerifyIfNull(jsonString.Questions[loop].Length.Value);
                                        var sequence = VerifyIfNull(jsonString.Questions[loop].SequenceNumber.Value);
                                        var precision = VerifyIfNull(jsonString.Questions[loop].Precision.Value);

                                        newquestion = new Question
                                        {
                                            AnswerType = (AnswerType)jsonString.Questions[loop].AnswerType.Value,
                                            Answer = da,
                                            Applicable = jsonString.Questions[loop].Applicable.Value,
                                            ChannelDisabled = jsonString.Questions[loop].ChannelDisabled.Value,
                                            Code = jsonString.Questions[loop].Code.Value,
                                            Length = lenght,
                                            Mandatory = jsonString.Questions[loop].Mandatory.Value,
                                            Precision = precision,
                                            PreConditions = precondition.ToArray(),
                                            ReEvaulateOnChange = jsonString.Questions[loop].ReEvaulateOnChange.Value,
                                            Scale = scale,
                                            SequenceNumber = sequence,
                                            Text = jsonString.Questions[loop].Text.Value,
                                            Validations = basevalidation.ToArray()
                                        };

                                        break;

                                    case 2:
                                        string k = jsonString.Questions[loop]["Answer"]["Answer"];
                                        TextAnswer var = new TextAnswer { Answer = k };

                                        List<BaseValidation> basevalidation2 = new List<BaseValidation>();
                                        foreach (var itm in jsonString.Questions[loop].Validations)
                                        {
                                            BaseValidation var3 = new BaseValidation { ExtensionData = itm.ExtensionData };
                                            basevalidation2.Add(var3);
                                        }

                                        List<PreCondition> precondition2 = new List<PreCondition>();
                                        foreach (var itm in jsonString.Questions[loop].PreConditions)
                                        {
                                            PreCondition var3 = new PreCondition { AnswerCode = itm.AnswerCode.Value, ParameterCode = itm.ParameterCode.Value, ParameterValue = itm.ParameterValue.Value, PreConditionType = (PreConditionTypes)itm.PreConditionType.Value, QuestionCode = itm.QuestionCode.Value };
                                            precondition2.Add(var3);
                                        }
                                        var scale2 = VerifyIfNull(jsonString.Questions[loop].Scale.Value);
                                        var lenght2 = VerifyIfNull(jsonString.Questions[loop].Length.Value);
                                        var sequence2 = VerifyIfNull(jsonString.Questions[loop].SequenceNumber.Value);
                                        var precision2 = VerifyIfNull(jsonString.Questions[loop].Precision.Value);

                                        newquestion = new Question
                                        {
                                            AnswerType = (AnswerType)jsonString.Questions[loop].AnswerType.Value,
                                            Answer = var,
                                            Applicable = jsonString.Questions[loop].Applicable.Value,
                                            ChannelDisabled = jsonString.Questions[loop].ChannelDisabled.Value,
                                            Code = jsonString.Questions[loop].Code.Value,
                                            Length = lenght2,
                                            Mandatory = jsonString.Questions[loop].Mandatory.Value,
                                            Precision = precision2,
                                            PreConditions = precondition2.ToArray(),
                                            ReEvaulateOnChange = jsonString.Questions[loop].ReEvaulateOnChange.Value,
                                            Scale = scale2,
                                            SequenceNumber = sequence2,
                                            Text = jsonString.Questions[loop].Text.Value,
                                            Validations = basevalidation2.ToArray()
                                        };

                                        break;

                                    case 3:

                                        List<AnswerValue> av = new List<AnswerValue>();
                                        foreach (var itm in jsonString.Questions[loop]["Answer"]["Answers"])
                                        {
                                            var seq = VerifyIfNull(itm.SequenceNumber.Value);
                                            AnswerValue var3 = new AnswerValue { Code = itm.Code.Value, SequenceNumber = seq, Text = itm.Text.Value };
                                            av.Add(var3);
                                        }
                                        int seqAns0;
                                        if (jsonString.Questions[loop]["Answer"]["Answer"].Code.Value == null)
                                        {
                                            seqAns0 = (int)jsonString.Questions[loop]["Answer"]["Answer"].SequenceNumber.Value;
                                        }
                                        else
                                        {
                                            seqAns0 = (int)jsonString.Questions[loop]["Answer"]["Answer"].SequenceNumber.Value;
                                        }
                                        var codeAns0 = jsonString.Questions[loop]["Answer"]["Answer"].Code.Value;
                                        var textAns0 = jsonString.Questions[loop]["Answer"]["Answer"].Text.Value;
                                        var asnLst0 = new AnswerValue { Code = codeAns0, SequenceNumber = seqAns0, Text = textAns0 };

                                        ListOfValuesAnswer var_0 = new ListOfValuesAnswer { Answer = asnLst0, Answers = av.ToArray() };
                                        //qs.Add(vara);

                                        List<BaseValidation> basevalidation0 = new List<BaseValidation>();
                                        foreach (var itm in jsonString.Questions[loop].Validations)
                                        {
                                            BaseValidation var3 = new BaseValidation { ExtensionData = itm.ExtensionData.Value };
                                            basevalidation0.Add(var3);
                                        }

                                        List<PreCondition> precondition0 = new List<PreCondition>();
                                        foreach (var itm in jsonString.Questions[loop].PreConditions)
                                        {
                                            var pre = VerifyIfNull(itm.PreConditionType.Value);
                                            PreCondition var3 = new PreCondition { AnswerCode = itm.AnswerCode.Value, ParameterCode = itm.ParameterCode.Value, ParameterValue = itm.ParameterValue.Value, PreConditionType = (PreConditionTypes)itm.PreConditionType.Value, QuestionCode = itm.QuestionCode.Value };
                                            precondition0.Add(var3);
                                        }

                                        var scale0 = VerifyIfNull(jsonString.Questions[loop].Scale.Value);
                                        var lenght0 = VerifyIfNull(jsonString.Questions[loop].Length.Value);
                                        var sequence0 = VerifyIfNull(jsonString.Questions[loop].SequenceNumber.Value);
                                        var precision0 = VerifyIfNull(jsonString.Questions[loop].Precision.Value);

                                        newquestion = new Question
                                        {
                                            AnswerType = (AnswerType)jsonString.Questions[loop].AnswerType.Value,
                                            Answer = var_0,
                                            Applicable = jsonString.Questions[loop].Applicable.Value,
                                            ChannelDisabled = jsonString.Questions[loop].ChannelDisabled.Value,
                                            Code = jsonString.Questions[loop].Code.Value,
                                            Length = lenght0,
                                            Mandatory = jsonString.Questions[loop].Mandatory.Value,
                                            Precision = precision0,
                                            PreConditions = precondition0.ToArray(),
                                            ReEvaulateOnChange = jsonString.Questions[loop].ReEvaulateOnChange.Value,
                                            Scale = scale0,
                                            SequenceNumber = sequence0,
                                            Text = jsonString.Questions[loop].Text.Value,
                                            Validations = basevalidation0.ToArray()
                                        };

                                        break;

                                    case 4:
                                        var l = jsonString.Questions[loop]["Answer"]["Answer"];
                                        BooleanAnswer ba = new BooleanAnswer { Answer = Convert.ToBoolean(l), };

                                        List<BaseValidation> basevalidation3 = new List<BaseValidation>();
                                        foreach (var itm in jsonString.Questions[loop].Validations)
                                        {
                                            BaseValidation var3 = new BaseValidation { ExtensionData = itm.ExtensionData };
                                            basevalidation3.Add(var3);
                                        }

                                        List<PreCondition> precondition3 = new List<PreCondition>();
                                        foreach (var itm in jsonString.Questions[loop].PreConditions)
                                        {
                                            PreCondition var3 = new PreCondition { AnswerCode = itm.AnswerCode, ParameterCode = itm.ParameterCode, ParameterValue = itm.ParameterValue, PreConditionType = itm.PreConditionType, QuestionCode = itm.QuestionCode };
                                            precondition3.Add(var3);
                                        }

                                        newquestion = new Question
                                        {
                                            AnswerType = (AnswerType)jsonString.Questions[loop].AnswerType.Value,
                                            Answer = ba,
                                            Applicable = jsonString.Questions[loop].Applicable.Value,
                                            ChannelDisabled = jsonString.Questions[loop].ChannelDisabled.Value,
                                            Code = jsonString.Questions[loop].Code.Value,
                                            Length = jsonString.Questions[loop].Length.Value,
                                            Mandatory = jsonString.Questions[loop].Mandatory.Value,
                                            Precision = jsonString.Questions[loop].Precision.Value,
                                            PreConditions = precondition3.ToArray(),
                                            ReEvaulateOnChange = jsonString.Questions[loop].ReEvaulateOnChange.Value,
                                            Scale = jsonString.Questions[loop].Scale.Value,
                                            SequenceNumber = (int)jsonString.Questions[loop].SequenceNumber.Value,
                                            Text = jsonString.Questions[loop].Text.Value,
                                            Validations = basevalidation3.ToArray()
                                        };
                                        break;
                                }

                                qstn.Add(newquestion);
                                loop++;
                            }

                            var ver = VerifyIfNull(jsonString.Version.Value);

                           

                            questionRequest = new ClaimRecordingServiceWS.QuestionRequest
                            {
                                CompanyCode = jsonString.CompanyCode
                                  ,
                                CaseNumber = jsonString.CaseNumber.Value
                                  ,
                                InteractionNumber = jsonString.InteractionNumber.Value
                                  ,
                                QuestionSetCode = jsonString.QuestionSetCode.Value
                                  ,
                                Version = ver
                                  ,
                                Questions = qstn.ToArray()
                            };

                            jsonrequest = JsonConvert.SerializeObject(questionRequest);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método SubmitQuestion exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);

                            var result = questionRequest.Questions;

                            DataTable dt = DBOperations.GetQuestionTemplate(cl.IdCliente, jsonString.QuestionSetCode.Value);

                            foreach (var question in result)
                            {
                                string text = (from DataRow myRow in dt.Rows
                                               where myRow.Field<int>("id_q") == question.SequenceNumber
                                               select myRow.Field<string>("text")).FirstOrDefault();

                                question.Text = text ?? question.Text; //.Select("text_esp");
                            }
                            string questions = JsonConvert.SerializeObject(questionRequest);
                            questionResponse = policyServiceClient.Submit(questionRequest);
                            
                            jsonresponse = JsonConvert.SerializeObject(questionResponse);
                            log4net.ThreadContext.Properties["Response"] = jsonresponse;
                            log4net.ThreadContext.Properties["Request"] = jsonrequest;
                            dynamic se = JsonConvert.DeserializeObject(jsonresponse);
                            string cnumber = se["ClaimNumber"];
                            dynamic jObj = JsonConvert.DeserializeObject(jsonresponse);

                            string casos = jObj["CaseNumber"];
                            string desc = jsonString.Questions[1]["Answer"]["Answer"];
                            string descs = jsonString.Questions[1]["Answer"]["Answer"];
                            DBOperations.UpdateClaimDetailsQuestions(questions, cnumber, jsonString.Certificate.Value, casos, desc);

                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método SubmitQuestion exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                                DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                                return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("CertificateNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                                DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                                return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("EnrollFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                                DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                                return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("DealerNotFoundFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                                DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                                return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
                            }
                            else
                            {
                                log.Error(string.Format("RegItemFault - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                                return BadRequest();
                            }
                        }
                        catch (Exception ex)
                        {
                            FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                            if (Fa.Enabled == true)
                            {
                                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                                string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                                DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                                return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
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
                        FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                        if (Fa.Enabled == true)
                        {
                            log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonrequest},{jsonresponse},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                            string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                            DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                            return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
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
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Convert.ToInt32(Convert.ToInt32(msspt_idcliente)));
                if (Fa.Enabled == true)
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    string preguntas = ClaimFolioHelper.Preguntas(jsonString);
                    DBOperations.UpdateClaimFolioFromSubmitQuestion(preguntas);
                    return Ok(ClaimFolioHelper.GenerateJsonResponseSimulation());
                }
                else
                {
                    log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    return BadRequest();
                }
            }
        }

        public Nullable<int> VerifyIfNull(Nullable<long> number)
        {
            if (number == null)
                return null;
            return (int)number;
        }
    }
}