using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MSSPAPI.Controllers
{
    public class TwilioController : ApiController
    {
        [HttpPost]
        [Route("api/SendMediaWhats")]
        public IHttpActionResult SendMediaWhats(ClienteFacturacion cf, string msg)
        {
            try
            {
                bool resp = sendMediaWhatsApp(cf.URLCertificate, msg, cf.Numero);
                if (resp)
                    return Ok("Mensaje Enviado");
                else return BadRequest("No Se Pudo Enviar El Mensaje");
            }
            catch (Exception)
            {
                return BadRequest("No Se Pudo Enviar El Mensaje");
            }
        }

        [HttpPost]
        [Route("api/SendTriggerWhats")]
        public IHttpActionResult SendTriggerWhats(ClienteFacturacion cf, string msg)
        {
            try
            {
                bool resp = sendWhatsAppTrigger("Hola Buen día, esta a punto de iniciar un reclamo", "Reclamo", "ElegiblePrueba", "+524493879709", "+14155238886");
                if (resp)
                    return Ok("Mensaje Enviado");
                else return BadRequest("No Se Pudo Enviar El Mensaje");
            }
            catch (Exception)
            {
                return BadRequest("No Se Pudo Enviar El Mensaje");
            }
        }


        [HttpGet]
        [Route("api/sendMessageWhatsapp")]
        public HttpResponseMessage sendMessageWhatsapp(string Message)
        {

            HttpResponseMessage httpResponse = new HttpResponseMessage();

            List<ServiceProviderConfiguration> serviceProviderList = DBOperations.GetServiceProviderConfiguration(null,"TWILIO");
            ServiceProviderConfiguration serviceProvider = serviceProviderList[0];
            JObject twilioConfig = JObject.Parse(serviceProvider.Configuration);

            string accountSID = twilioConfig["AccountSId"].Value<string>();
            string AuthToken = twilioConfig["AuthToken"].Value<string>();

            TwilioClient.Init(accountSID, AuthToken);


            Twilio.Rest.Api.V2010.Account.MessageResource message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(body: "Mensaje prueba", from: "whatsapp:+14155238886", to: "whatsapp:+524493879709");

            return httpResponse;
        }

        private bool sendMediaWhatsApp(string url, string msj, string number)
        {
            try
            {
                List<ServiceProviderConfiguration> spl = DBOperations.GetServiceProviderConfiguration(null, "TWILIO");
                ServiceProviderConfiguration sp = spl[0];
                string jsonConfig = sp.Configuration;
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);

                string accountSid = jsonObj["AccountSId"];
                string authToken = jsonObj["AuthToken"];

                TwilioClient.Init(accountSid, authToken);

                var mediaUrl = new[] {
                    new Uri(url)
                }.ToList();


                var message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
                    body: msj,
                    from: new Twilio.Types.PhoneNumber(jsonObj["WhatsApp"]),
                    to: new Twilio.Types.PhoneNumber(number)

                );

                var message1 = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
                    mediaUrl: mediaUrl,
                    from: new Twilio.Types.PhoneNumber(jsonObj["WhatsApp"]),
                    to: new Twilio.Types.PhoneNumber(number)

                );

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        private bool sendWhatsAppTrigger(string msj, string actionType, string elegible, string to, string from)
        {
            try
            {
                List<ServiceProviderConfiguration> spl = DBOperations.GetServiceProviderConfiguration(null,"TWILIO");
                ServiceProviderConfiguration sp = spl[0];
                JObject jsonConfig =  JObject.Parse(sp.Configuration);
                

                string endPoint = sp.BaseUrl;

                string ParametersJson = $"{{\"vehicleElegible\":\"{elegible}\",\"msj\":\"{msj}\",\"action\":\"{actionType}\"}}";

                var client = new HttpClient();

                var data = new[]
                {
                    new KeyValuePair<string, string>("To", to),
                    new KeyValuePair<string, string>("From", from),
                    new KeyValuePair<string, string>("Parameters", ParametersJson)
                };
                //aqui van acountsid:authtoken
                var authenticationString = jsonConfig["AccountSId"].Value<string>() + ":" + jsonConfig["AuthToken"].Value<string>();

                byte[] bufferBase64 = Encoding.UTF8.GetBytes(authenticationString);

                var base64EncodedAuthenticationString = Convert.ToBase64String(bufferBase64);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                HttpResponseMessage res = client.PostAsync(endPoint, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();

                Stream responseContent = res.Content.ReadAsStreamAsync().GetAwaiter().GetResult();

                MemoryStream memoryStream = new MemoryStream();

                responseContent.CopyTo(memoryStream);


                byte[] bufferFromMS = memoryStream.GetBuffer();


                string content = Encoding.UTF8.GetString(bufferFromMS);


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}