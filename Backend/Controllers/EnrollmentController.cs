using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Controlador que contiene todos los endpoints para realizar el enrollment de un certificado
    /// </summary>
    [RoutePrefix("Enrollment")]
    public class EnrollmentController : ApiController
    {
        /// <summary>
        /// Endpoint para enrolar un vehiculo en Elita
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("")]
        public async Task<HttpResponseMessage> Enrollment(Models.VSCEnrollmentDs request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                ServiceProviderConfiguration providerConfig = DBOperations.GetServiceProviderConfiguration(null, "ElitaEnrollmentService").ElementAt(0);

                string requestCDataXml = await EnrollmentHelper<Models.VSCEnrollmentDs>.CreateXmlRequest(request);

                string token = await GetTokenAsync(request.VSCEnrollment.Vehicle_Make, providerConfig.BaseUrl);

                string resultXml = await ProccessRequestAsync(providerConfig.BaseUrl, token, "Enroll", requestCDataXml);

                if (resultXml.Contains("<Error>"))
                {
                    ElitaProcessRequestResponseError responseError = EnrollmentHelper<ElitaProcessRequestResponseError>.GetDeserializedObject<ElitaProcessRequestResponseError>(resultXml);
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new ObjectContent<ElitaProcessRequestResponseError>(responseError, formatter);
                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.NoContent;
                }
            }
            catch(Exception ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new ObjectContent<Exception>(ex, formatter);
            }


            return httpResponse;
        }

        /// <summary>
        /// Enpoint para realizar una cotización de un vehiculo en elita
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Quote")]
        [ResponseType(typeof(Models.VSC_QUOTE_ENGINE))]
        public async Task<HttpResponseMessage> Quote(Models.VSCQuoteDs request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                ServiceProviderConfiguration providerConfig = DBOperations.GetServiceProviderConfiguration(null, "ElitaEnrollmentService").ElementAt(0);

                string requestQuoteXml = await EnrollmentHelper<Models.VSCQuoteDs>.CreateXmlRequest(request);

                string token = await GetTokenAsync(request.VSCQuote.Make, providerConfig.BaseUrl);

                //string vscServiceResponse = await vscWcfClient.ProcessRequestAsync(make.TokenEnrollment, "GetQuote", requestQuoteXml);

                string resultXml = await ProccessRequestAsync(providerConfig.BaseUrl, token, "GetQuote", requestQuoteXml);

                if (resultXml.Contains("<Error>"))
                {
                    ElitaProcessRequestResponseError responseError = EnrollmentHelper<ElitaProcessRequestResponseError>.GetDeserializedObject<ElitaProcessRequestResponseError>(resultXml);
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new ObjectContent<ElitaProcessRequestResponseError>(responseError, formatter);
                }
                else
                {
                    Models.VSC_QUOTE_ENGINE VscQuoteEngine = EnrollmentHelper<Models.VSC_QUOTE_ENGINE>.GetDeserializedObject<Models.VSC_QUOTE_ENGINE>(resultXml);
                    httpResponse.StatusCode = HttpStatusCode.OK;
                    httpResponse.Content = new ObjectContent<Models.VSC_QUOTE_ENGINE>(VscQuoteEngine, formatter);
                }
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new ObjectContent<Exception>(ex, formatter); 
            }

            return httpResponse;
        }

        private async Task<string> ProccessRequestAsync(string seviceUrl, string token, string functionToProcess, string requestXml)
        {
            string resultXml = "";

            try
            {
                WebRequest webRequest = WebRequest.Create(seviceUrl);
                HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/xml; charset=utf-8";
                httpWebRequest.Headers.Add("SOAPAction: http://elita.assurant.com/vsc/IVscWcf/ProcessRequest");
                httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                Stream requestStream = httpWebRequest.GetRequestStream();
                StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.ASCII);
                StringBuilder soapRequest = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:vsc=\"http://elita.assurant.com/vsc\">\r\n   <soapenv:Header/>\r\n <soapenv:Body>\r\n <vsc:ProcessRequest>\r\n");

                soapRequest.Append("<vsc:token>" + token + "</vsc:token>\r\n");
                soapRequest.Append("<vsc:functionToProcess>" + functionToProcess +   "</vsc:functionToProcess>\r\n");
                soapRequest.Append("<vsc:xmlStringDataIn>" + requestXml + "</vsc:xmlStringDataIn>\r\n");
                soapRequest.Append("</vsc:ProcessRequest>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>");

                streamWriter.Write(soapRequest.ToString());
                streamWriter.Close();

                HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());

                resultXml = await streamReader.ReadToEndAsync();

                resultXml = resultXml.Replace("&lt;", "<").Replace("&gt;", ">");

                int startResult = resultXml.LastIndexOf("<ProcessRequestResult>");
                int endResult = resultXml.IndexOf("</ProcessRequestResult>");
                int lengthStr = endResult - startResult;

                resultXml = resultXml.Substring(startResult, lengthStr).Replace("<ProcessRequestResult>", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultXml;
        }

        private async Task<string> GetTokenAsync(string make, string serviceUrl)
        {
            string token = "";
            try
            {
                Marcas m = DBOperations.GetOneBranch(make);

                VscServiceModelA1.VscWcfClient vscWcfClient = new VscServiceModelA1.VscWcfClient("BasicHttpBinding_IVscWcf", serviceUrl);

                vscWcfClient.Open();

                if (string.IsNullOrEmpty(m.TokenEnrollment) || (!string.IsNullOrEmpty(m.TokenEnrollment) && (m.FechaFin.HasValue && DateTime.Now > m.FechaFin)))
                {
                    string VscToken = await vscWcfClient.LoginBodyAsync(m.Configuration.UsrServiceConsumer, m.Configuration.PwdServiceConsumer, m.Configuration.GroupServiceConsumer);

                    DBOperations.UpdateTokenEnrollment(VscToken, DateTime.Now, DateTime.Now.AddDays(1));

                    m.TokenEnrollment = VscToken;
                }

                token = m.TokenEnrollment;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return token;
        }
    }
}
