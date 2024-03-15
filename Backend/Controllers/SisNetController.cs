using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;
using System.Web.Http.Description;
using Org.BouncyCastle.Asn1.Crmf;
using System.Web.Http.Results;
using System.Windows.Forms;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Controlador correspondiente a sisnet (apis que solo consumimos mediante la url y los parametros que requieran)
    /// </summary>
    [RoutePrefix("SisNet")]
    public class SisNetController : ApiController    
    {

        /// <summary>
        /// Endpoint para descargar de documentos siniestro generico externo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DescargaDocumentoSiniestroExterno")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> DescargaDocumentoSiniestro(DataSisnetDescargaDocumento request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "ObtenerDescargarDocumentosSiniestrosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisNetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse = validateSisnetResponse(sisNetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }


            return httpResponse;
        }

        /// <summary>
        /// Endpoint para cargar de documentos siniestro generico externo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CargaDocumentoSiniestroExterno")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CargaDocumentoSiniestro(DataSisnetCargaDocumento request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaDocumentosSiniestrosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisNetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse = validateSisnetResponse(sisNetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }


            return httpResponse;
        }

        /// <summary>
        /// Endpoint para la carga de siniestros para Nissan Gap y Mazda Gap
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("CargaSiniestroMazdaGapNissanGap")]
        public async Task<HttpResponseMessage> CargaSiniestroMazdaGapNissanGap(DataSisNetCargaSiniestroMazdaNissan requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            SisNetTracking snt = new SisNetTracking();
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "SISNET");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Usuario", serviceProviderConfiguration.SvcUsr);
                httpClient.DefaultRequestHeaders.Add("Password", serviceProviderConfiguration.SvcPwd);

                SisNetRequest netRequest = new SisNetRequest()
                {
                    Start = 0,
                    Limit = 25,
                    Page = 0,
                    Comando = "CargaSiniestroGenericoExterno",
                    Data = JsonConvert.SerializeObject(requestModel),
                    UserGroupData = 0
                };

                HttpResponseMessage sisNetResponse = await httpClient.PostAsJsonAsync<SisNetRequest>(serviceProviderConfiguration.BaseUrl + "sisos/Execute", netRequest);

                if (sisNetResponse.IsSuccessStatusCode)
                {
                    string respSisNet = await sisNetResponse.Content.ReadAsStringAsync();

                    SisNetResponse obtenerCertificadoResponse = GetSisNetResponse(respSisNet);

                    if (obtenerCertificadoResponse != null)
                    {
                        if (obtenerCertificadoResponse.data.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(obtenerCertificadoResponse.data[0].tipo) && obtenerCertificadoResponse.data[0].tipo == "ERR")
                            {
                                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                                httpResponse.Content = new StringContent(obtenerCertificadoResponse.data[0].texto);
                                snt.Command = "CargaSiniestroGenericoExterno";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                            else
                            {
                                httpResponse.StatusCode = HttpStatusCode.OK;
                                httpResponse.Content = new ObjectContent<SisNetResponse>(obtenerCertificadoResponse, formatter);
                                snt.Command = "CargaSiniestroGenericoExterno";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.OK);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error inesperado");
                    }

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Ocurrio un error inesperado en sisnet");
                    snt.Command = "CargaSiniestroGenericoExterno";
                    snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                    snt.Rqt = netRequest.Data;
                    snt.Rsn = "Ocurrio un error inesperado en sisnet";
                    DBOperations.CreateTrackingSisNet(snt);
                }

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "ObtenerCertificadosGenericoExterno";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a obtener certificados
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("ObtenerCertificados")]
        public async Task<HttpResponseMessage> ObtenerCertificados(DataCertificadoGenericoExterno requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            SisNetTracking snt = new SisNetTracking();
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "SISNET");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Usuario", serviceProviderConfiguration.SvcUsr);
                httpClient.DefaultRequestHeaders.Add("Password", serviceProviderConfiguration.SvcPwd);

                SisNetRequest netRequest = new SisNetRequest()
                {
                    Start = 0,
                    Limit = 25,
                    Page = 0,
                    Comando = "ObtenerCertificadosGenericoExterno",
                    Data = JsonConvert.SerializeObject(requestModel),
                    UserGroupData = 0
                };

                HttpResponseMessage sisNetResponse = await httpClient.PostAsJsonAsync<SisNetRequest>(serviceProviderConfiguration.BaseUrl + "sisos/Execute", netRequest);
                
                if (sisNetResponse.IsSuccessStatusCode)
                {
                    string respSisNet = await sisNetResponse.Content.ReadAsStringAsync();

                    SisNetResponse obtenerCertificadoResponse = GetSisNetResponse(respSisNet);

                    if (obtenerCertificadoResponse != null)
                    {
                        if (obtenerCertificadoResponse.data.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(obtenerCertificadoResponse.data[0].tipo) && obtenerCertificadoResponse.data[0].tipo == "ERR")
                            {
                                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                                httpResponse.Content = new StringContent(obtenerCertificadoResponse.data[0].texto);
                                snt.Command = "ObtenerCertificadosGenericoExterno";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                            else
                            {
                                httpResponse.StatusCode = HttpStatusCode.OK;
                                httpResponse.Content = new ObjectContent<SisNetResponse>(obtenerCertificadoResponse, formatter);
                                snt.Command = "ObtenerCertificadosGenericoExterno";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.OK);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error inesperado");
                    }

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Ocurrio un error inesperado en sisnet");
                    snt.Command = "ObtenerCertificadosGenericoExterno";
                    snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                    snt.Rqt = netRequest.Data;
                    snt.Rsn = "Ocurrio un error inesperado en sisnet";
                    DBOperations.CreateTrackingSisNet(snt);
                }

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "ObtenerCertificadosGenericoExterno";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a carga endosos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("CargaEndososSumaAseguradaExternoNCPIMazda")]
        public async Task<HttpResponseMessage> CargaEndososSumaAseguradaExterno(DataCargaEndososNCPI_MazdaGAP requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            SisNetTracking snt = new SisNetTracking();
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "SISNET");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Usuario", serviceProviderConfiguration.SvcUsr);
                httpClient.DefaultRequestHeaders.Add("Password", serviceProviderConfiguration.SvcPwd);

                SisNetRequest netRequest = new SisNetRequest()
                {
                    Start = 0,
                    Limit = 25,
                    Page = 0,
                    Comando = "CargaEndosoSumaAseguradaGenericoExterno",
                    Data = JsonConvert.SerializeObject(requestModel),
                    UserGroupData = 0
                };

                HttpResponseMessage sisNetResponse = await httpClient.PostAsJsonAsync<SisNetRequest>(serviceProviderConfiguration.BaseUrl + "sisos/Execute", netRequest);
               
                if (sisNetResponse.IsSuccessStatusCode)
                {
                    string respSisNet = await sisNetResponse.Content.ReadAsStringAsync();

                    SisNetResponse obtenerCertificadoResponse = GetSisNetResponse(respSisNet);

                    if (obtenerCertificadoResponse != null)
                    {
                        if (obtenerCertificadoResponse.data.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(obtenerCertificadoResponse.data[0].tipo) && obtenerCertificadoResponse.data[0].tipo == "ERR")
                            {
                                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                                httpResponse.Content = new StringContent(obtenerCertificadoResponse.data[0].texto);
                                snt.Command = "CargaEndosoSumaAseguradaGenericoExternoNCPIMazda";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                            else
                            {
                                httpResponse.StatusCode = HttpStatusCode.OK;
                                httpResponse.Content = new ObjectContent<SisNetResponse>(obtenerCertificadoResponse, formatter);
                                snt.Command = "CargaEndosoSumaAseguradaGenericoExternoNCPIMazda";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.OK);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);

                                
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error inesperado");
                    }

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Ocurrio un error inesperado en sisnet");
                    snt.Command = "CargaEndosoSumaAseguradaGenericoExternoNCPIMazda";
                    snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                    snt.Rqt = netRequest.Data;
                    snt.Rsn = "Ocurrio un error inesperado en sisnet";
                    DBOperations.CreateTrackingSisNet(snt);
                }

            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndosoSumaAseguradaGenericoExternoNCPIMazda";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a carga endosos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("CargaEndososSumaAseguradaExternoSICrea")]
        public async Task<HttpResponseMessage> CargaEndososSumaAseguradaExternoSICrea(DataCargaEndososSiCrea requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            SisNetTracking snt = new SisNetTracking();
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "SISNET");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Usuario", serviceProviderConfiguration.SvcUsr);
                httpClient.DefaultRequestHeaders.Add("Password", serviceProviderConfiguration.SvcPwd);

                SisNetRequest netRequest = new SisNetRequest()
                {
                    Start = 0,
                    Limit = 25,
                    Page = 0,
                    Comando = "CargaEndosoSumaAseguradaGenericoExterno",
                    Data = JsonConvert.SerializeObject(requestModel),
                    UserGroupData = 0
                };

                HttpResponseMessage sisNetResponse = await httpClient.PostAsJsonAsync<SisNetRequest>(serviceProviderConfiguration.BaseUrl + "sisos/Execute", netRequest);
                
                if (sisNetResponse.IsSuccessStatusCode)
                {
                    string respSisNet = await sisNetResponse.Content.ReadAsStringAsync();

                    SisNetResponse obtenerCertificadoResponse = GetSisNetResponse(respSisNet);

                    if (obtenerCertificadoResponse != null)
                    {
                        if (obtenerCertificadoResponse.data.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(obtenerCertificadoResponse.data[0].tipo) && obtenerCertificadoResponse.data[0].tipo == "ERR")
                            {
                                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                                httpResponse.Content = new StringContent(obtenerCertificadoResponse.data[0].texto);
                                snt.Command = "CargaEndosoSumaAseguradaGenericoExternoSiCrea";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                            else
                            {
                                httpResponse.StatusCode = HttpStatusCode.OK;
                                httpResponse.Content = new ObjectContent<SisNetResponse>(obtenerCertificadoResponse, formatter);
                                snt.Command = "CargaEndosoSumaAseguradaGenericoExternoSiCrea";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.OK);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);

                                
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error inesperado");
                    }

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Ocurrio un error inesperado en sisnet");
                    snt.Command = "CargaEndosoSumaAseguradaGenericoExternoSiCrea";
                    snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                    snt.Rqt = netRequest.Data;
                    snt.Rsn = "Ocurrio un error inesperado en sisnet";
                    DBOperations.CreateTrackingSisNet(snt);
                }

            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndosoSumaAseguradaGenericoExternoSiCrea";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para a carga endosos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("CargaEndososSumaAseguradaExternoNissanGAP")]
        public async Task<HttpResponseMessage> CargaEndososSumaAseguradaExternoNissanGAP(DataCargaEndososNissanGap requestModel)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            SisNetTracking snt = new SisNetTracking();
            try
            {

                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "SISNET");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Usuario", serviceProviderConfiguration.SvcUsr);
                httpClient.DefaultRequestHeaders.Add("Password", serviceProviderConfiguration.SvcPwd);

                SisNetRequest netRequest = new SisNetRequest()
                {
                    Start = 0,
                    Limit = 25,
                    Page = 0,
                    Comando = "CargaEndosoSumaAseguradaGenericoExterno",
                    Data = JsonConvert.SerializeObject(requestModel),
                    UserGroupData = 0
                };

                HttpResponseMessage sisNetResponse = await httpClient.PostAsJsonAsync<SisNetRequest>(serviceProviderConfiguration.BaseUrl + "sisos/Execute", netRequest);
                
                if (sisNetResponse.IsSuccessStatusCode)
                {
                    string respSisNet = await sisNetResponse.Content.ReadAsStringAsync();

                    SisNetResponse obtenerCertificadoResponse = GetSisNetResponse(respSisNet);

                    if (obtenerCertificadoResponse != null)
                    {
                        if (obtenerCertificadoResponse.data.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(obtenerCertificadoResponse.data[0].tipo) && obtenerCertificadoResponse.data[0].tipo == "ERR")
                            {
                                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                                httpResponse.Content = new StringContent(obtenerCertificadoResponse.data[0].texto);
                                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                            else
                            {
                                httpResponse.StatusCode = HttpStatusCode.OK;
                                httpResponse.Content = new ObjectContent<SisNetResponse>(obtenerCertificadoResponse, formatter);
                                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                                snt.StCo = Convert.ToInt32(HttpStatusCode.OK);
                                snt.Rqt = netRequest.Data;
                                snt.Rsn = obtenerCertificadoResponse.data[0].texto;
                                DBOperations.CreateTrackingSisNet(snt);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error inesperado");
                    }

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Ocurrio un error inesperado en sisnet");
                    snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                    snt.StCo = Convert.ToInt32(HttpStatusCode.BadRequest);
                    snt.Rqt = netRequest.Data;
                    snt.Rsn = "Ocurrio un error inesperado en sisnet";
                    DBOperations.CreateTrackingSisNet(snt);
                }

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
                snt.Command = "CargaEndososSumaAseguradaExternoNissanGAP";
                snt.StCo = Convert.ToInt32(HttpStatusCode.InternalServerError);
                snt.Rqt = JsonConvert.SerializeObject(requestModel);
                snt.Rsn = ex.Message;
                DBOperations.CreateTrackingSisNet(snt);
            }

            return httpResponse;
        }


        /// <summary>
        /// Endpoint para cancelar un certificado de tipo NCPI, Mazda GAP, SI-CREA
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelacionCertificadoNcpiMazdaGapSiCrea")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CancelacionCertificadoNcpiMazdaGapSiCrea(SisNetCargaCancelacionCertificadoNCPI_MazdaGap_SiCrea request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest netRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaCancelacionCertificadoGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                netRequest.Data = JsonConvert.SerializeObject(request);

                string respSisNet = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(netRequest);

                httpResponse = validateSisnetResponse(respSisNet);

                await SaveTrackingSisnet(httpResponse, netRequest);

            }
            catch (TimeoutException ex)
            {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    await SaveTrackingSisnet(httpResponse, netRequest);
                    httpResponse.Content = new StringContent(ex.Message);

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                await SaveTrackingSisnet(httpResponse, netRequest);
                httpResponse.Content = new StringContent(ex.Message);

            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para cancelar un certificado de tipo Nissan GAP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelacionCertificadoNissanGap")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CancelacionCertificadoNissanGap(SisNetCargaCancelacionCertificadoNissanGap request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest netRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaCancelacionCertificadoGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                netRequest.Data = JsonConvert.SerializeObject(request);

                string respSisNet = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(netRequest);

                httpResponse = validateSisnetResponse(respSisNet);

                await SaveTrackingSisnet(httpResponse, netRequest);

            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para cargar certificados de tipo NCPI 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CargaCertificadoNCPI")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CargaCertificadoNCPI(SisnetCargaCertificadosNCPI request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaCertificadosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisNetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse = validateSisnetResponse(sisNetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para cargar certificados de tipo SI-CREA 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CargaCertificadoSiCrea")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CargaCertificadoSiCrea(SisnetCargaCertificadosSiCrea request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaCertificadosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisNetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse = validateSisnetResponse(sisNetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
           

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para cargar certificados de tipo Mazda GAP 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CargaCertificadoMazdaGap")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CargaCertificadoMazdaGap(SisnetCargaCertificadosMazdaGap request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaCertificadosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisNetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse = validateSisnetResponse(sisNetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
           

            return httpResponse;
        }

        /// <summary>
        /// Endpoint para cargar certificados de tipo Nissan GAP 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CargaCertificadoNissanGap")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CargaCertificadoNissanGap(SisnetCargaCertificadosNissanGap request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaCertificadosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisNetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse = validateSisnetResponse(sisNetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            

            return httpResponse;
        }


        /// <summary>
        /// Enpoint para la carga de un siniestro de tipo NCPI, Si-Crea
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CargaSiniestroNcpiSiCrea")]
        [Authorize]
        [ResponseType(typeof(SisNetResponse))]
        public async Task<HttpResponseMessage> CargaSiniestroNcpiSiCrea(SisNetCargaSiniestroNCPI_SiCrea request)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            SisNetRequest sisNetRequest = new SisNetRequest()
            {
                Start = 0,
                Limit = 25,
                Page = 0,
                Comando = "CargaSiniestroGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                sisNetRequest.Data = JsonConvert.SerializeObject(request);

                string sisnetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisNetRequest);

                httpResponse =  validateSisnetResponse(sisnetResponse);

                await SaveTrackingSisnet(httpResponse, sisNetRequest);

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
           

            return httpResponse;
        }

        /// <summary>
        /// Enpoint para obtener un listado de siniestros
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ObtenerSiniestros")]
        [Authorize]
        public async Task<HttpResponseMessage> ObtenerSiniestros(SisnetSinisterSearchFilters filters, int? limit = null)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            SisNetRequest sisnetrequest = new SisNetRequest()
            {
                Start = 0,
                Limit = (limit.HasValue) ? limit.Value : 25,
                Page = 0,
                Comando = "ObtenerSiniestrosGenericoExterno",
                Data = "",
                UserGroupData = 0
            };

            try
            {
                JObject filtersSearch = new JObject();

                if (filters.PolizaId.HasValue && filters.PolizaId.Value > 0)
                {
                    JToken polizaIdVal = JToken.FromObject(filters.PolizaId.Value);
                    filtersSearch.Add("PolizaId", polizaIdVal);
                }

                if (filters.SiniestroId.HasValue && filters.SiniestroId.Value > 0)
                {
                    JToken siniestroIdVal = JToken.FromObject(filters.SiniestroId.Value);
                    filtersSearch.Add("SiniestroId", siniestroIdVal);
                }

                if (!string.IsNullOrEmpty(filters.CodigoSiniestro))
                {
                    JToken CodigoSiniestroVal = JToken.FromObject(filters.CodigoSiniestro);
                    filtersSearch.Add("CodigoSiniestro", CodigoSiniestroVal);
                }

                if (!string.IsNullOrEmpty(filters.CodigoSecundarioCertificado))
                {
                    JToken CodigoSecundarioCertificadoVal = JToken.FromObject(filters.CodigoSecundarioCertificado);
                    filtersSearch.Add("CodigoSecundarioCertificado", CodigoSecundarioCertificadoVal);
                }

                if (filtersSearch.HasValues)
                {
                    sisnetrequest.Data = JsonConvert.SerializeObject(filtersSearch);
                    string sisnetResponse = await SisnetRequestHelper<SisNetRequest>.sendRequestToSisnet(sisnetrequest);

                    httpResponse = validateSisnetResponse(sisnetResponse);

                    await SaveTrackingSisnet(httpResponse, sisnetrequest);
                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Por favor envie al menos un filtro con valor");
                }

            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
            }

            return httpResponse;
        } 


        private SisNetResponse GetSisNetResponse(string response)
        {
            SisNetResponse result = new SisNetResponse();
            try
            {
                JObject jsonResponse = JObject.Parse(response);
                JArray dataArray = JArray.Parse(jsonResponse.SelectToken("data").ToString());
                JToken data = dataArray[0];


                SisNetResponseData respData = new SisNetResponseData();
                respData.id = data.SelectToken("$id").Value<string>();
                respData.href = data.SelectToken("href").Value<string>();
                respData.texto = data.SelectToken("texto").Value<string>();
                respData.tipo = data.SelectToken("tipo").Value<string>();

                result.id = jsonResponse.SelectToken("$id").Value<string>();
                result.data = new SisNetResponseData[] { respData };
                result.total = jsonResponse.SelectToken("total").Value<int>();
                result.sucess = jsonResponse.SelectToken("sucess").Value<bool>();
            }
            catch (FormatException)
            {
                result = null;
            }
            catch (TimeoutException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        private HttpResponseMessage validateSisnetResponse(string response)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    SisNetResponse sisnetResponse = GetSisNetResponse(response);

                    if (sisnetResponse != null)
                    {
                        if (sisnetResponse.data.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(sisnetResponse.data[0].tipo) && sisnetResponse.data[0].tipo == "ERR")
                            {
                                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                                httpResponse.Content = new StringContent(sisnetResponse.data[0].texto);
                            }
                            else
                            {
                                httpResponse.StatusCode = HttpStatusCode.OK;
                                httpResponse.Content = new ObjectContent<SisNetResponse>(sisnetResponse, formatter);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error inesperado");
                    }

                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new StringContent("Ocurrio un error inesperado en sisnet");
                }
            }
            catch (TimeoutException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (FormatException ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(ex.Message);
            }
            

            return httpResponse;
        }


        private async Task SaveTrackingSisnet(HttpResponseMessage httpResponse, SisNetRequest sisNetRequest)
        {
            try
            {
                SisNetTracking netTracking = new SisNetTracking();
                netTracking.Id = 0;
                netTracking.Command = sisNetRequest.Comando;
                netTracking.StCo = (int)httpResponse.StatusCode;
                netTracking.Rqt = JsonConvert.SerializeObject(sisNetRequest);
                netTracking.Rsn = (httpResponse.IsSuccessStatusCode) ? JsonConvert.SerializeObject(await httpResponse.Content.ReadAsAsync<SisNetResponse>()) : await httpResponse.Content.ReadAsStringAsync();
                netTracking.CrudHistoryList = "";
                netTracking.Active = true;
                DBOperations.CreateTrackingSisNet(netTracking);
            }
            catch (FormatException e)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
            }
            catch (TimeoutException e)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
            }
            catch (Exception e)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new StringContent(e.Message);
            }
           
        }
    }
}