using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    public class BDEOController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected string url = "https://api.bdeo.io/prod/v2";


        /// <summary>
        /// Endpoint para crear un nuevo token de acceso para realizar paticiones hacia el api de BDEO
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("BDEO/Login")]
        public async Task<HttpResponseMessage> Login(BDEOLoginModel requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                ServiceProviderConfiguration configuration = providerConfigurationList[0];

                var client = new RestClient(configuration.BaseUrl);
                var request = new RestRequest("/login", Method.Post);
                request.AddBody(requestModel);
                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

                    httpResponse.StatusCode = HttpStatusCode.OK;
                    httpResponse.Content = new ObjectContent<LoginResponse>(loginResponse, formatter);

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(response.Content);
                }
            }
            catch (Exception ex)
            {

                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new ObjectContent<Exception>(ex, formatter);
            }

            return httpResponse;
        }


        /// <summary>
        /// Endpoint para a crear un nuevo autoajuste en BDEO
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="BDEOToken" in="header"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("BDEO/selfadjust")]
        public async Task<HttpResponseMessage> SelfAdjust(BDEOSelfAdjustCreateRequest requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];


                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("access_token", BDEOToken);


                    HttpResponseMessage bdeoResponse = await client.PostAsJsonAsync(providerConfiguration.BaseUrl +  "/selfadjust", requestModel);

                    if (bdeoResponse.IsSuccessStatusCode)
                    {
                        BDEOSelfAdjustCreateResponse selfAdjustResponse = await bdeoResponse.Content.ReadAsAsync<BDEOSelfAdjustCreateResponse>();
                        //BDEOSelfAdjustCreateResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOSelfAdjustCreateResponse>(response.Content);



                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOSelfAdjustCreateResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent("Ocurrio un error en BDEO");
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a crear un nuevo autoajuste externo en BDEO
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="BDEOToken" in="header"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("BDEO/extrnalselfadjust")]
        public async Task<HttpResponseMessage> ExternalSelfAdjust(BDEOExternalSelfAdjustCreateRequest requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/external-selfadjust", Method.Post);

                    request.AddHeader("access_token", BDEOToken);
                    request.AddBody(requestModel);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        BDEOExternalSelfAdjustCreateResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOExternalSelfAdjustCreateResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOExternalSelfAdjustCreateResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint updload url autoajuste externo en BDEO
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="BDEOToken" in="header"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("BDEO/uploadurl")]
        public async Task<HttpResponseMessage> UploadUrlSelfAdjust(BDEOUploadUrlRequest requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    providerConfiguration.BaseUrl = providerConfiguration.BaseUrl.Substring(0, providerConfiguration.BaseUrl.LastIndexOf("v2"));

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/files-system/v2/upload_url", Method.Post);

                    request.AddHeader("access_token", BDEOToken);
                    request.AddBody(requestModel);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        httpResponse.StatusCode = HttpStatusCode.OK;
                        object bdeoResponse = JsonConvert.DeserializeObject<object>(response.Content);
                        httpResponse.Content = new ObjectContent<object>(bdeoResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a obtener autoajuste en BDEO
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="BDEOToken" in="header"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("BDEO/selfadjust/{id}")]
        [ResponseType(typeof(BDEOSelfAdjustGetResponse))]
        public async Task<HttpResponseMessage> SelfAdjust(string id)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("access_token", BDEOToken);

                    HttpResponseMessage bdeoResponse = await httpClient.GetAsync(providerConfiguration.BaseUrl + "/selfadjust/" + id);

                    if (bdeoResponse.IsSuccessStatusCode)
                    {
                        string bdeoResponseStr = await bdeoResponse.Content.ReadAsStringAsync();

                        char startJson = bdeoResponseStr.ElementAt(0);
                        char endJson = bdeoResponseStr.ElementAt(bdeoResponseStr.Length - 1);

                        if (startJson == '{' && endJson == '}')
                        {
                            JObject bdeoObject = JObject.Parse(bdeoResponseStr);
                            httpResponse.StatusCode = HttpStatusCode.OK;
                            httpResponse.Content = new ObjectContent<JObject>(bdeoObject, jsonMediaType);
                        } 
                        else
                        {
                            httpResponse.StatusCode = HttpStatusCode.BadRequest;
                            httpResponse.Content = new StringContent("Ocurrio un error inesperado al obtener la información de BDEO"); 
                        }
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        string message = await bdeoResponse.Content.ReadAsStringAsync();
                        httpResponse.Content = new StringContent(message);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a actualizar autoajuste en BDEO
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="BDEOToken" in="header"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("BDEO/ModifySelfadjust/{id}")]
        public async Task<HttpResponseMessage> ModifySelfAdjust(BDEOSelfAdjustModifyRequest requestModel, string id)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/selfadjust/" + id, Method.Put);

                    request.AddHeader("access_token", BDEOToken);
                    request.AddBody(requestModel);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        BDEOSelfAdjustCreateResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOSelfAdjustCreateResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOSelfAdjustCreateResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a eliminar autoajuste en BDEO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("BDEO/DeleteSelfadjust/{id}")]
        public async Task<HttpResponseMessage> DeleteSelfAdjust(string id)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/selfadjust/" + id, Method.Delete);

                    request.AddHeader("access_token", BDEOToken);
                    request.AddBody(id);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        BDEOSelfAdjustCreateResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOSelfAdjustCreateResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOSelfAdjustCreateResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        /*
         * 
         * I N T E R V E N T I O N   S E C T I O N
         * 
         */
        [HttpPost]
        [Authorize]
        [Route("BDEO/intervention")]
        public async Task<HttpResponseMessage> Intervention(BDEOInterventionCreateRequest body)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/intervention", Method.Post);

                    request.AddHeader("access_token", BDEOToken);
                    request.AddBody(body);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        BDEOSelfAdjustCreateResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOSelfAdjustCreateResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOSelfAdjustCreateResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }
            return httpResponse;
        }


        [HttpGet]
        [Authorize]
        [Route("BDEO/intervention")]
        public async Task<HttpResponseMessage> GetAllInterventions()
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/intervention", Method.Get);

                    request.AddHeader("access_token", BDEOToken);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        BDEOInterventionListResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOInterventionListResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOInterventionListResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }
            return httpResponse;
        }


        [HttpGet]
        [Authorize]
        [Route("BDEO/intervention/{interventionId}")]
        public async Task<HttpResponseMessage> GetIntervention(string interventionId)
        {

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/intervention/"+interventionId, Method.Get);

                    request.AddHeader("access_token", BDEOToken);

                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        BDEOInterventionResponse selfAdjustResponse = JsonConvert.DeserializeObject<BDEOInterventionResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOInterventionResponse>(selfAdjustResponse, jsonMediaType);
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }
            return httpResponse;
        }

        [HttpPut]
        [Authorize]
        [Route("BDEO/intervention/{interventionId}")]
        [ResponseType(typeof(BDEOInterventionCreateResponse))]
        public async Task<HttpResponseMessage> UpdateIntervention(BDEOInterventionUpdateRequest body, string interventionId)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/intervention/" + interventionId, Method.Put);

                    request.AddHeader("access_token", BDEOToken);
                    request.AddBody(body);
                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        BDEOInterventionCreateResponse bDEOInterventionResp = JsonConvert.DeserializeObject<BDEOInterventionCreateResponse>(response.Content);

                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOInterventionCreateResponse>(bDEOInterventionResp, jsonMediaType);
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent("Please check the \"BDEOToken\" maybe it has expired");
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }
            return httpResponse;
        }

        [HttpDelete]
        [Authorize]
        [Route("BDEO/intervention/{interventionId}")]
        public async Task<HttpResponseMessage> DeleteIntervention(string interventionId)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> HeadersRequest;

            if (this.Request.Headers.TryGetValues("BDEOToken", out HeadersRequest))
            {
                string BDEOToken = HeadersRequest.ElementAt(0);
                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];

                    var client = new RestClient(providerConfiguration.BaseUrl);
                    var request = new RestRequest("/intervention/"+interventionId, Method.Delete);

                    request.AddHeader("access_token", BDEOToken);
                    RestResponse response = await client.ExecuteAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new StringContent(interventionId + " Deleted");
                    }
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        httpResponse.Content = new StringContent(response.Content);
                    }

                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent(ex.Message);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent("Header \"BDEOToken\" was not provided");
            }

            return httpResponse;
        }

        [HttpPost]
        [Authorize]
        [Route("BDEO/generate-agent-url")]
        [ResponseType(typeof(BDEOGenerateAgentUrlResponse))]
        public async Task<HttpResponseMessage> GenerateAgentUrl(BDEOGenerateAgentUrlRequest request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> headers;

            if (this.Request.Headers.TryGetValues("BDEOToken", out headers))
            {
                string BDEOToken = headers.ElementAt(0);

                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("access_token", BDEOToken);

                    HttpResponseMessage responseBDEO = await client.PostAsJsonAsync<BDEOGenerateAgentUrlRequest>(providerConfiguration.BaseUrl + "/generate-agent-url", request);

                    if (responseBDEO.IsSuccessStatusCode)
                    {
                        BDEOGenerateAgentUrlResponse agentUrlResponse = await responseBDEO.Content.ReadAsAsync<BDEOGenerateAgentUrlResponse>();
                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOGenerateAgentUrlResponse>(agentUrlResponse, formatter);
                    } 
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        string responseMessage = await responseBDEO.Content.ReadAsStringAsync();
                        httpResponse.Content = new ObjectContent<object>(new { Message = responseMessage }, formatter);
                    }
                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                    httpResponse.Content = new ObjectContent<object>(new { Message = ex.Message}, formatter);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new ObjectContent<object>(new { Message = "Header \"BDEOToken\" not found" }, formatter);
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para obtener los usuarios 
        /// </summary>
        /// <param name="type">Tipo de usuario el valor por defecto es agent</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("BDEO/users")]
        [ResponseType(typeof(BDEOUsersResponse))]
        public async Task<HttpResponseMessage> GetBDEOUsers(string type = "agent")
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            IEnumerable<string> headers;

            if (this.Request.Headers.TryGetValues("BDEOToken", out headers))
            {
                string BDEOToken = headers.ElementAt(0);

                try
                {
                    List<ServiceProviderConfiguration> providerConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "BDEO");
                    ServiceProviderConfiguration providerConfiguration = providerConfigurationList[0];
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("access_token", BDEOToken);

                    string requestUrl = $"{providerConfiguration.BaseUrl}/users?type={type}";

                    HttpResponseMessage responseBDEO = await client.GetAsync(requestUrl);

                    if (responseBDEO.IsSuccessStatusCode)
                    {
                        BDEOUsersResponse usersResponse = await responseBDEO.Content.ReadAsAsync<BDEOUsersResponse>();
                        httpResponse.StatusCode = HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<BDEOUsersResponse>(usersResponse, formatter);
                    } 
                    else
                    {
                        httpResponse.StatusCode = HttpStatusCode.BadRequest;
                        string responseMessage = await responseBDEO.Content.ReadAsStringAsync();
                        httpResponse.Content = new ObjectContent<string>(responseMessage, formatter);
                    }
                }
                catch (Exception ex)
                {
                    httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                    httpResponse.Content = new ObjectContent<object>(new { Message = ex.Message }, formatter);
                }
            }
            else
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new ObjectContent<object>(new { Message = "Header \"BDEOToken\" not found" }, formatter);
            }

            return httpResponse;
        }

    }
}