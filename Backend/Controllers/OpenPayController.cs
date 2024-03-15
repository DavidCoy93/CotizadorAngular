using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers {
    /// <summary>
    /// Controlador para las apis relacionadas con los pagos
    /// </summary>
    public class OpenPayController : ApiController {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Crear un cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("OpenPay/cargo")]
        public IHttpActionResult CreateCargo(OpenPay_Cargo cargo)
        {
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "OPENPAY");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                var client = new RestClient(serviceProviderConfiguration.BaseUrl);
                var request = new RestRequest("/api/charge", Method.Post);
                request.AddObject(cargo);
                var response = client.Execute(request);
                OpenPay_Response openPayResponse = new OpenPay_Response();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    openPayResponse = JsonConvert.DeserializeObject<OpenPay_Response>(response.Content);
                    return Ok(openPayResponse);
                }
                else return BadRequest(response.ErrorMessage);
                    
            }
            catch(Exception ex)
            {
                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackingKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("OpenPay/cargo/{trackingKey}")]
        public IHttpActionResult ReadReference(string trackingKey)
        {
            try 
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "OPENPAY");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                var client = new RestClient(serviceProviderConfiguration.BaseUrl);
                var request = new RestRequest(serviceProviderConfiguration.BaseUrl + "/api/charge/" + trackingKey, Method.Get);
                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    OpenPay_Response openPayResponse = JsonConvert.DeserializeObject<OpenPay_Response>(response.Content);

                    return Ok(openPayResponse);
                }
                else
                    return BadRequest(response.ErrorMessage);
            }
            catch(Exception ex)
            {
                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Consultar el cargo
        /// </summary>
        /// <param name="trackingKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("OpenPay/peticion_cargo/{trackingKey}")]
        public IHttpActionResult ReadCargo(string trackingKey)
        {
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "OPENPAY");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                var client = new RestClient(serviceProviderConfiguration.BaseUrl);
                var request = new RestRequest("/AIZPay/index/" + trackingKey, Method.Get);
                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)                
                    return Ok(response.Content);
                else
                    return BadRequest(response.ErrorMessage);
            }
            catch(Exception ex)
            {
                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
            
        }

        /// <summary>
        /// Obtener el pdf del cargo
        /// </summary>
        /// <param name="trackingKey"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize]
        [Route("OpenPay/download_referencia/{trackingKey}")]
        public IHttpActionResult GetPDF(string trackingKey)
        {
            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null, "OPENPAY");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];
                var client = new RestClient(serviceProviderConfiguration.BaseUrl);
                var request = new RestRequest(serviceProviderConfiguration.BaseUrl + "/api/pdf/" + trackingKey, Method.Get);
                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    OpenPay_Response openPayResponse = JsonConvert.DeserializeObject<OpenPay_Response>(response.Content);
                    return Ok(serviceProviderConfiguration.BaseUrl + "/api/pdf/" + trackingKey);
                }
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{""},{""},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                return BadRequest(ex.Message);
            }
            
        }
    }
}