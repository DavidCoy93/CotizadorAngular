using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using MSSPAPI.PolicyServiceWS;
using Newtonsoft.Json;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class ProductServiceController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el product service para de esta manera obtener mas detalle del producto
        /// </summary>
        /// <returns></returns>
        // GET: ProductService
        [System.Web.Http.Route("api/ProductService")]
        public IHttpActionResult GetProductService()
        {
            try
            {
                var hreq = this.Request.Headers;
                string msspt = hreq.GetValues("tokenmssp").First();
                if (DBOperations.GetToken(msspt))
                {
                    ProductServiceWS.GetProdDeviceTypesResponse responsePolicy = null;

                    try
                    {


                        log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
                        log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
                        var productServiceClient = new ProductServiceWS.ProductServiceClient(Constants.ENPTNAMEPRODUCT, Constants.URLPRODUCT);

                        string jsonresponse = string.Empty;
                        string jsonConfig = string.Empty;
                        string jsonrequest = string.Empty;
                        Cliente cl = DBOperations.GetClientesById(1); //Constantes cuando haya mas clientes
                        jsonConfig = cl.Configuraciones;
                        dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                        productServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
                        productServiceClient.ClientCredentials.UserName.Password = jsonObj["Pwds"];
        
                       
                        try
                        {
                            var requestProduct = new ProductServiceWS.GetProdDeviceTypesRequest
                            {
                            };
                            jsonrequest = JsonConvert.SerializeObject(requestProduct);
                            responsePolicy = productServiceClient.GetProductsAndDeviceTypes(requestProduct);
                            Bitacora btreq = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Request a método RegistereItem exitosamente. " + jsonrequest, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btreq);
                            jsonresponse = JsonConvert.SerializeObject(responsePolicy);
                            Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Response a método RegistereItem exitosamente. " + jsonresponse, Plataforma = "MSSP_ELITA" };
                            DBOperations.InsertBitacora(btresp);
                            log.Info(string.Format("Info - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}"));
                        }
                        catch (FaultException<PolicyServiceWS.CertificateNotFoundFault> fault)
                        {
                            log.Error(string.Format("CertificateNotFoundFault - {0}", fault.Detail.FaultMessage), fault);
                            return Ok(GenerateJsonResponseSimulation());
                        }
                        catch (FaultException<PolicyServiceWS.EnrollFault> fault)
                        {
                            log.Error(string.Format("EnrollFault - {0}", fault.Detail.FaultMessage), fault);
                            return Ok(GenerateJsonResponseSimulation());
                        }
                        catch (FaultException<PolicyServiceWS.DealerNotFoundFault> fault)
                        {
                            log.Error(string.Format("DealerNotFoundFault - {0}", fault.Detail.FaultMessage), fault);
                            return Ok(GenerateJsonResponseSimulation());
                        }
                        catch (FaultException<PolicyServiceWS.RegItemFault> fault)
                        {
                            log.Error(string.Format("RegItemFault - {0}", fault.Detail.FaultMessage), fault);
                            return Ok(GenerateJsonResponseSimulation());
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("Exception - {0}", ex.Message), ex);
                            return Ok(GenerateJsonResponseSimulation());
                        }

                        return Ok(jsonresponse);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                        return Ok(GenerateJsonResponseSimulation());
                    }
                }
                return Ok(GenerateJsonResponseSimulation());
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Exception - {0}", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))},{null},{null},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                return Ok(GenerateJsonResponseSimulation());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateJsonResponseSimulation()
        {
            string data = "{\"ErrorCode\":\"0\",\"ErrorMessage\":\"null\",\"ProductDeviceTypes\":[{\"DealerCode\":\"EA13\",\"DealerName\":\"Liverpool - PIF\",\"ProductCode\":\"LVPIF\",\"ProductDescription\":\"Liverpool PIF\",\"AllowRegisteredItems\":\"Yes\",\"MaxAgeofRegisteredItem\":\"36\",\"MaxRegistrationsAllowed\":\"\",\"RewardInfo\":[{\"DaysToRedeem\":\"\",\"EffectiveDate\":\"\",\"ExpirationDate\":\"\",\"MinPurchasePrice\":\"\",\"RewardAmount\":\"\",\"RewardName\":\"\",\"RewardType\":\"\"}],\"DeviceTypeInfo\":[{\"DeviceTypeCode\":\"AICOM\",\"DeviceTypeDescription\":\"Compresor de aire\"},{\"DeviceTypeCode\":\"AICON\",\"DeviceTypeDescription\":\"Aire acondicionado\"},{\"DeviceTypeCode\":\"AUPRO\",\"DeviceTypeDescription\":\"Equipo de sonido\"},{\"DeviceTypeCode\":\"BYTOY\",\"DeviceTypeDescription\":\"Juguete eléctrico\"},{\"DeviceTypeCode\":\"CAAUD\",\"DeviceTypeDescription\":\"Autoestéreo\"},{\"DeviceTypeCode\":\"CEL\",\"DeviceTypeDescription\":\"CELLULAR\"},{\"DeviceTypeCode\":\"CEPHO\",\"DeviceTypeDescription\":\"Teléfono celular\"},{\"DeviceTypeCode\":\"COMP\",\"DeviceTypeDescription\":\"Computadora\"},{\"DeviceTypeCode\":\"DESKT\",\"DeviceTypeDescription\":\"Computadora de escritorio\"},{\"DeviceTypeCode\":\"DIWAS\",\"DeviceTypeDescription\":\"Lavavajillas\"},{\"DeviceTypeCode\":\"DRYER\",\"DeviceTypeDescription\":\"Secadora\"},{\"DeviceTypeCode\":\"ELTOO\",\"DeviceTypeDescription\":\"Herramienta eléctrica\"},{\"DeviceTypeCode\":\"FITNE\",\"DeviceTypeDescription\":\"Aparato de ejercicio\"},{\"DeviceTypeCode\":\"GAOVE\",\"DeviceTypeDescription\":\"Horno de gas\"},{\"DeviceTypeCode\":\"GAWAT\",\"DeviceTypeDescription\":\"Calentador de agua\"},{\"DeviceTypeCode\":\"HOTHE\",\"DeviceTypeDescription\":\"Teatro en casa\"},{\"DeviceTypeCode\":\"KIT\",\"DeviceTypeDescription\":\"Kitchen Appliances\"},{\"DeviceTypeCode\":\"LAV\",\"DeviceTypeDescription\":\"Lavador\"},{\"DeviceTypeCode\":\"MICRO\",\"DeviceTypeDescription\":\"Horno de mocroondas\"},{\"DeviceTypeCode\":\"MUPRI\",\"DeviceTypeDescription\":\"Multifuncional\"},{\"DeviceTypeCode\":\"NOTEB\",\"DeviceTypeDescription\":\"Computadora portátil\"},{\"DeviceTypeCode\":\"PECAP\",\"DeviceTypeDescription\":\"Producto de cuidado personal\"},{\"DeviceTypeCode\":\"PHCAM\",\"DeviceTypeDescription\":\"Cámara fotográfica\"},{\"DeviceTypeCode\":\"POAUD\",\"DeviceTypeDescription\":\"Audifonos\"},{\"DeviceTypeCode\":\"POOFP\",\"DeviceTypeDescription\":\"Control inalambrico\"},{\"DeviceTypeCode\":\"POVID\",\"DeviceTypeDescription\":\"Cámara de video portátil\"},{\"DeviceTypeCode\":\"PRINT\",\"DeviceTypeDescription\":\"Impresora\"},{\"DeviceTypeCode\":\"RANGE\",\"DeviceTypeDescription\":\"Estufas\"},{\"DeviceTypeCode\":\"REFRI\",\"DeviceTypeDescription\":\"Refrigerador\"},{\"DeviceTypeCode\":\"SAPP\",\"DeviceTypeDescription\":\"Small Appliances\"},{\"DeviceTypeCode\":\"SMAPP\",\"DeviceTypeDescription\":\"Electrodoméstico\"},{\"DeviceTypeCode\":\"SMWAT\",\"DeviceTypeDescription\":\"Reloj inteligente\"},{\"DeviceTypeCode\":\"TABLE\",\"DeviceTypeDescription\":\"Tableta electrónica\"},{\"DeviceTypeCode\":\"TELEV\",\"DeviceTypeDescription\":\"Pantalla TV\"},{\"DeviceTypeCode\":\"TEPRO\",\"DeviceTypeDescription\":\"Teléfono inalámbrico\"},{\"DeviceTypeCode\":\"TV\",\"DeviceTypeDescription\":\"Television\"},{\"DeviceTypeCode\":\"VIDEO\",\"DeviceTypeDescription\":\"Consola de videojuego\"},{\"DeviceTypeCode\":\"VIPRO\",\"DeviceTypeDescription\":\"Video Proyector\"},{\"DeviceTypeCode\":\"WADRC\",\"DeviceTypeDescription\":\"Centro de lavado\"},{\"DeviceTypeCode\":\"WASHE\",\"DeviceTypeDescription\":\"Lavadora\"},{\"DeviceTypeCode\":\"WICOO\",\"DeviceTypeDescription\":\"Enfriador de vino\"}],\"UpdateReplaceRegItems\":\"\",\"RegisteredItemsLimit\":\"\"}]}";
            return data;
        }
    }
}