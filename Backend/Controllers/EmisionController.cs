using MSSPAPI.EmisionServiceDev;
using MSSPAPI.Globals;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using System.ServiceModel;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using Microsoft.Ajax.Utilities;
using System.IO;
using System.Text;
using MSSPAPI.PolicyServiceWS;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Controlador de Emision Kitt
    /// </summary>
    public class EmisionController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Endpoint para emitir facturas
        /// </summary>
        /// <param name="cf">Objeto tipo ClienteFacturación</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("EmisionFacturaCFDI")]
        [ResponseType(typeof(ClienteFacturacion))]
        public HttpResponseMessage EmisionFacturaCFDI(ClienteFacturacion cf)
        {
            
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter jsonMediaType = new JsonMediaTypeFormatter();
            jsonMediaType.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            //Cliente cl = DBOperations.GetClientesById(cf.IdCliente); //Constantes cuando haya mas clientes
            //Marcas m = DBOperations.GetOneBranch(cf.Marca); 
            //string jsonresponse = string.Empty;
            //string jsonresponseVehicle = string.Empty;
            //string jsonConfig = string.Empty;
            //string jsonrequest = string.Empty;
            //jsonConfig = cl.Configuraciones;
            //dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
            //List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(cf.IdCliente);
            ////Creamos la instancia para el getpolicy
            //ElitaLoginService.LoginResponse responseLogin = null;
            //var loginElita = new ElitaLoginService.ElitaLoginServiceClient();
            //loginElita.Endpoint.Binding.SendTimeout = new TimeSpan(0, 30, 5);
            //if (m.TokenEnrollment == null)
            //{
            //    ElitaLoginService.LoginASRequest req = new ElitaLoginService.LoginASRequest()
            //    {
            //        LoginBody = new ElitaLoginService.LoginBody()
            //        {
            //            networkID = m.Configuration.UsrServiceConsumer,
            //            password = m.Configuration.PwdServiceConsumer,
            //            group = m.Configuration.GroupServiceConsumer
            //        }
            //    };
            //    responseLogin =  await loginElita.LoginAsync(req.LoginBody);
            //    jsonresponse = responseLogin.LoginBodyReponse.LoginBodyResult;
            //    //hacer el insert del token con su respectiva fecha
            //    DBOperations.UpdateTokenEnrollment(jsonresponse, DateTime.Now, DateTime.Now.AddDays(1));
            //    m = DBOperations.GetOneBranch(cf.Marca);
            //    jsonresponse = m.TokenEnrollment;
            //}
            //else
            //{
            //    if(!(m.FechaInicio <= DateTime.Now && DateTime.Now <= m.FechaFin))
            //    {
            //        ElitaLoginService.LoginASRequest req = new ElitaLoginService.LoginASRequest()
            //        {
            //            LoginBody = new ElitaLoginService.LoginBody()
            //            {
            //                networkID = m.Configuration.UsrServiceConsumer,
            //                password = m.Configuration.PwdServiceConsumer,
            //                group = m.Configuration.GroupServiceConsumer
            //            }
            //        };
            //        responseLogin = await loginElita.LoginAsync(req.LoginBody);
            //        jsonresponse = responseLogin.LoginBodyReponse.LoginBodyResult;
            //        //hacer el insert del token con su respectiva fecha
            //        DBOperations.UpdateTokenEnrollment(jsonresponse, DateTime.Now, DateTime.Now.AddDays(1));
            //        m = DBOperations.GetOneBranch(cf.Marca);
            //        jsonresponse = m.TokenEnrollment;
            //    }
            //    else
            //        jsonresponse = m.TokenEnrollment;
            //}

            ////Aqui va la cotización del vehiculo
            //ElitaVehicleService.EnrollResponse responseVehicle = null;
            //var elitav = new ElitaVehicleService.ElitaVehicleServiceClient();
            //elitav.Endpoint.Binding.SendTimeout = new TimeSpan(0, 30, 5);
            //ElitaVehicleService.ProcessRequest reque = new ElitaVehicleService.ProcessRequest()
            //{
            //    token = jsonresponse,
            //    ProcessRequestBody = new ElitaVehicleService.ProcessRequestProcessRequestBody()
            //    {
            //        VSCEnrollmentDs = new ElitaVehicleService.VSCEnrollmentDs()
            //        {                       
            //            VSCEnrollment = new ElitaVehicleService.VSCEnrollmentDsVSCEnrollment()
            //            {
            //                Certificate_Number = cf.CertificateNumber,
            //                Customer = new ElitaVehicleService.VSCEnrollmentDsVSCEnrollmentCustomer()
            //                {
            //                    Customer_Name = cf.Names + " " + cf.LastName,
            //                },
            //                Address = cf.Calle + " " + cf.Numero,
            //                City = cf.Localidad,
            //                Region = cf.Pais,
            //                Postal_Code = cf.CP,
            //                Country_Code = cf.Pais,
            //                Home_Phone = cf.Telefono,
            //                Vehicle_Year = Convert.ToInt32(cf.Anio),
            //                //Vehicle_Mileage = cf.Kilometers,
            //                VIN = cf.VIN,
            //                Vehicle_Purchase_Price = cf.PrecioTotal,
            //                Vehicle_Purchase_Date = cf.DateInicioPoliza,
            //                Vehicle_In_Service_Date = cf.DateCreation,
            //                Vehicle_Delivery_Date = cf.DateFinPoliza,
            //                //Plan_Code = cf.Cantidad.ToString(),
            //                //Plan_Amount = cf.Cantidad,
            //                //Quote_Item_Number = cf.Cantidad,
            //                //Term_Months = cf.Cantidad,
            //                //Term_Miles = cf.Cantidad,
            //                //Deductible = cf.Cantidad,
            //                Dealer_Code = cl.DealerCode.FirstOrDefault().DealerCode,
            //                //Agent_Number = cf.Cantidad.ToString(),
            //                //Warranty_Sale_Date = cf.DateCreation,
            //                //Quote_Number = cf.Cantidad.ToString(),
            //                //Vehicle_License_Tag = cf.Modelo,
            //                //Document_Type = cf.Calle,
            //                //Identity_document_No = cf.Cantidad.ToString(),  
            //                //RG_No = cf.Cantidad.ToString(),
            //                //Issuing_agency = cf.Localidad.ToString(),
            //                //ID_Type = cf.Cantidad.ToString(),
            //                //Collection_Method_Code = cf.Cantidad.ToString(),
            //                //Payment_Instrument_Code = cf.Cantidad.ToString(),
            //                //Installments_Number = cf.Cantidad,
            //                //External_Car_Code = cf.Cantidad.ToString()
            //            }
            //        }
            //    }
            //};
            //responseVehicle = await elitav.EnrollAsync(reque);
            //jsonresponse = responseLogin.LoginBodyReponse.LoginBodyResult;

            try
            {
                cf.PrecioSIVA = Convert.ToDecimal(Convert.ToDouble(cf.PrecioTotal) / 1.16);
                string preciosivas = cf.PrecioSIVA.ToString("f2");
                cf.IVA = Convert.ToDecimal(Convert.ToDouble(cf.PrecioSIVA) * 0.16);
                string ivas = cf.IVA.ToString("f2");
                string totales = cf.PrecioTotal.ToString("f2");
                decimal valorUnitario = (cf.PrecioSIVA / cf.Cantidad);

                //Requiere factura
                if (cf.RequiereFactura == true)
                {
                    EmisoresFactura emisor = DBOperations.GetEmisoresFactById(cf.IdEmisores);
                    string xmlCFDIV4 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    Guid uuid = new Guid();
                    TransactionProperty[] transactionProperties = new TransactionProperty[] { };
                    ErrorMessageCode error = new ErrorMessageCode();
                    string resultXml;
                    XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";
                    XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                    XElement Comprobante = new XElement(cfdi + "Comprobante");
                    Comprobante.Add(new XAttribute("Fecha", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")));
                    Comprobante.Add(new XAttribute("FormaPago", "01"));
                    Comprobante.Add(new XAttribute("Serie", "CFDI4.0"));
                    Comprobante.Add(new XAttribute("Folio", "16"));
                    Comprobante.Add(new XAttribute("MetodoPago", "PUE"));
                    Comprobante.Add(new XAttribute("LugarExpedicion", "20200"));
                    Comprobante.Add(new XAttribute("Moneda", "MXN"));
                    Comprobante.Add(new XAttribute("SubTotal", preciosivas));
                    Comprobante.Add(new XAttribute("TipoDeComprobante", "I"));
                    Comprobante.Add(new XAttribute("Total", totales));
                    Comprobante.Add(new XAttribute("Exportacion", "01"));
                    Comprobante.Add(new XAttribute("Version", "4.0"));
                    Comprobante.Add(new XAttribute("Sello", ""));
                    Comprobante.Add(new XAttribute("Certificado", ""));
                    Comprobante.Add(new XAttribute("NoCertificado", ""));
                    Comprobante.Add(new XAttribute(XNamespace.Xmlns + "cfdi", cfdi));
                    Comprobante.Add(new XAttribute(XNamespace.Xmlns + "xsi", xsi));
                    Comprobante.Add(new XAttribute(xsi + "schemaLocation", "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"));
                    XElement Emisor = new XElement(cfdi + "Emisor",
                            new XAttribute("Rfc", emisor.Rfc),
                            new XAttribute("Nombre", emisor.Nombre),
                            new XAttribute("RegimenFiscal", emisor.RegimenFiscal)
                    );
                    string domicilioF = string.Format("{0}, {1}, {2} {3}, {4}, CP {5}", cf.Calle, cf.Numero, cf.Colonia, cf.CP, cf.Localidad, cf.Estado);
                    XElement Receptor = new XElement(cfdi + "Receptor",
                            new XAttribute("Rfc", cf.RFC),
                            new XAttribute("Nombre", cf.Names + " " + cf.LastName),
                            new XAttribute("UsoCFDI", cf.UsoCFDI),
                            new XAttribute("DomicilioFiscalReceptor", cf.CP),
                            new XAttribute("RegimenFiscalReceptor", cf.RegimenFiscal)
                    );

                    XElement Conceptos = new XElement(cfdi + "Conceptos",
                            new XElement(cfdi + "Concepto",
                                    new XAttribute("Cantidad", cf.Cantidad),
                                    new XAttribute("ClaveProdServ", "84131500"),
                                    new XAttribute("ClaveUnidad", "IP"),
                                    new XAttribute("Descripcion", "Seguro SRA"),
                                    new XAttribute("Importe", preciosivas),
                                    new XAttribute("ValorUnitario", preciosivas),
                                    new XAttribute("ObjetoImp", "02"),
                                    new XElement(cfdi + "Impuestos",
                                            new XElement(cfdi + "Traslados",
                                                    new XElement(cfdi + "Traslado",
                                                            new XAttribute("Base", preciosivas),
                                                            new XAttribute("Importe", ivas),
                                                            new XAttribute("Impuesto", "002"),
                                                            new XAttribute("TasaOCuota", "0.160000"),
                                                            new XAttribute("TipoFactor", "Tasa")
                                                    )
                                            )
                                    )
                            )
                    );

                    XElement Impuestos = new XElement(cfdi + "Impuestos",
                            new XAttribute("TotalImpuestosTrasladados", ivas),
                            new XElement(cfdi + "Traslados",
                                    new XElement(cfdi + "Traslado",
                                        new XAttribute("Importe", ivas),
                                        new XAttribute("Impuesto", "002"),
                                        new XAttribute("TasaOCuota", "0.160000"),
                                        new XAttribute("TipoFactor", "Tasa"),
                                        new XAttribute("Base", preciosivas)
                                    )
                            )
                    );

                    Comprobante.Add(Emisor);
                    Comprobante.Add(Receptor);
                    Comprobante.Add(Conceptos);
                    Comprobante.Add(Impuestos);

                    xmlCFDIV4 += Comprobante.ToString(SaveOptions.None);

                    var emisionClient = new EmisionClient();
                    string errorMessage = string.Empty;

                    uuid = emisionClient.EmitirComprobante(Globals.Constants.APKD, xmlCFDIV4, string.Empty, ref transactionProperties, string.Empty, out error, out resultXml);

                    if (error != null)
                    {
                        // Aqui tenemos los errores 
                        errorMessage = getErrorsUntilDone(error);
                        httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        httpResponse.Content = new ObjectContent<object>(new { Message = "No se pudo emitir debido a los siguientes errores: " + errorMessage }, jsonMediaType);
                        return httpResponse;
                    }
                    else
                    {

                        cf.Names = EncDec.Encript(cf.Names);
                        cf.LastName = EncDec.Encript(cf.LastName);
                        cf.RegimenFiscal = EncDec.Encript(cf.RegimenFiscal);
                        cf.Calle = EncDec.Encript(cf.Calle);
                        cf.Numero = EncDec.Encript(cf.Numero);
                        cf.Colonia = EncDec.Encript(cf.Colonia);
                        cf.Telefono = EncDec.Encript(cf.Telefono);
                        cf.CP = EncDec.Encript(cf.CP);
                        cf.Email = EncDec.Encript(cf.Email);
                        cf.Pais = EncDec.Encript(cf.Pais);
                        cf.Estado = EncDec.Encript(cf.Estado);
                        cf.Localidad = EncDec.Encript(cf.Localidad);
                        cf.XMLResponse = resultXml;
                        cf.UUID = InvoiceXmlHelper.getUUIDFromInvoice(resultXml);
                        int idCliente = DBOperations.InsertClienteFact(cf);
                        cf.Id = idCliente;

                        string base64Xml = InvoiceXmlHelper.getBase64FromXML(resultXml);
                        string urlAzureXML = "";
                        string urlAzurePDF = "";
                        string urlAzureCertificate = "";

                        //DocumentCertification documentCertification = new DocumentCertification()
                        //{
                        //    IdCliente = cf.IdCliente,
                        //    Certificate = cf.CertificateNumber,
                        //    Country = jsonObj["Country"],
                        //    DealerCode = dc.FirstOrDefault().DealerCode,
                        //    IdCertificate = "",
                        //    Base64File = base64Xml,
                        //    Extension = "xml"
                        //};

                        //this.Request.GetRouteData();

                        //string AbsoluteUri = this.Request.RequestUri.AbsoluteUri;

                        //string baseUrlApi = AbsoluteUri.Substring(0, AbsoluteUri.LastIndexOf('/'));


                        //IEnumerable<KeyValuePair<string, IEnumerable<string>>> currentAuthHeaders =  this.Request.Headers.Where(h => h.Key == "Authorization" || h.Key == "ApiKey");

                        //string token = currentAuthHeaders.ElementAt(0).Value.FirstOrDefault();
                        //string apiKey = currentAuthHeaders.ElementAt(1).Value.FirstOrDefault();


                        //HttpClient httpClient = new HttpClient();
                        //httpClient.BaseAddress = new Uri(baseUrlApi);
                        //httpClient.DefaultRequestHeaders.Add("Authorization", token);
                        //httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

                        //HttpResponseMessage getAzureXmlUrl = await httpClient.PostAsJsonAsync<DocumentCertification>("/api/UploadCertFiles", documentCertification);


                        //if (getAzureXmlUrl.IsSuccessStatusCode) 
                        //{ 
                        //    urlAzureXML = await getAzureXmlUrl.Content.ReadAsStringAsync();
                        //}

                        //HttpResponseMessage getBase64InvoicePdf = await imprimirComprobanteCFDI(cf.UUID.ToString(), "pdf");

                        //if (getBase64InvoicePdf.IsSuccessStatusCode)
                        //{
                        //    documentCertification.Base64File = await getBase64InvoicePdf.Content.ReadAsStringAsync();
                        //    documentCertification.Extension = "pdf";

                        //    HttpResponseMessage getAzurePdfUrl = await httpClient.PostAsJsonAsync<DocumentCertification>("/api/UploadCertFiles", documentCertification);

                        //    if (getAzurePdfUrl.IsSuccessStatusCode)
                        //    {
                        //        urlAzurePDF = await getAzurePdfUrl.Content.ReadAsStringAsync();
                        //    }
                        //}

                        //HttpResponseMessage getCertficateRequest = await httpClient.GetAsync("/api/Certificate/Download/" + cf.Id);

                        //if (getCertficateRequest.IsSuccessStatusCode)
                        //{
                        //    Stream certificateContent = await getCertficateRequest.Content.ReadAsStreamAsync();
                        //    MemoryStream memoryStream = new MemoryStream();
                        //    await certificateContent.CopyToAsync(memoryStream);
                        //    byte[] bytesCert = memoryStream.GetBuffer();
                        //    string base64Cert = Convert.ToBase64String(bytesCert);

                        //    documentCertification.Base64File = base64Cert;
                        //    documentCertification.Certificate = $"Cert_{documentCertification.Certificate}";

                        //    HttpResponseMessage getAzureCertUrl = await httpClient.PostAsJsonAsync<DocumentCertification>("/api/UploadCertFiles", documentCertification);

                        //    if (getAzureCertUrl.IsSuccessStatusCode)
                        //    {
                        //        urlAzureCertificate = await getAzureCertUrl.Content.ReadAsStringAsync();
                        //    }

                        //}

                        cf.URLCertificate = urlAzureCertificate;
                        cf.InvoicePDF = urlAzurePDF;
                        cf.InvoiceXML = urlAzureXML;
                        DBOperations.UpdateClienteFact(cf);
                        httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
                        httpResponse.Content = new ObjectContent<ClienteFacturacion>(cf, jsonMediaType);
                        return httpResponse;
                    }

                }
                else
                {
                    cf.Names = EncDec.Encript(cf.Names);
                    cf.LastName = EncDec.Encript(cf.LastName);
                    cf.RegimenFiscal = EncDec.Encript(cf.RegimenFiscal);
                    cf.Calle = EncDec.Encript(cf.Calle);
                    cf.Numero = EncDec.Encript(cf.Numero);
                    cf.Colonia = EncDec.Encript(cf.Colonia);
                    cf.Telefono = EncDec.Encript(cf.Telefono);
                    cf.CP = EncDec.Encript(cf.CP);
                    cf.Email = EncDec.Encript(cf.Email);
                    cf.Pais = EncDec.Encript(cf.Pais);
                    cf.Estado = EncDec.Encript(cf.Estado);
                    cf.Localidad = EncDec.Encript(cf.Localidad);
                    cf.XMLResponse = "";
                    cf.UUID = Guid.Empty;
                    int idCliente = DBOperations.InsertClienteFact(cf);
                    cf.Id = idCliente;
                    string urlCertpdf = string.Empty;
                    HttpRequest request = HttpContext.Current.Request;
                    //aqui vamos actualizar el registro con los datos de las url
                    if (request.IsSecureConnection)
                        urlCertpdf = "https://";
                    else
                        urlCertpdf = "http://";

                    urlCertpdf += request["HTTP_HOST"] + "/" + "api/Certificate/Download/" + cf.Id;
                    cf.URLCertificate = urlCertpdf;
                    cf.InvoicePDF = "";
                    cf.InvoiceXML = "";
                    DBOperations.UpdateClienteFact(cf);
                    ClienteFacturacion cfa = DBOperations.GetClientesFactById(cf.Id);
                    httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    httpResponse.Content = new ObjectContent<ClienteFacturacion>(cfa, jsonMediaType);
                    return httpResponse;
                }
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                httpResponse.Content = new ObjectContent<object>(new { Message = string.Format("Exception: {0}", ex.Message) }, jsonMediaType);
                return httpResponse;
            }
        }

        private string getErrorsUntilDone(ErrorMessageCode error)
        {
            string errorMessage = string.Empty;

            if (error != null)
            {
                errorMessage += error.Message + "\r\n";

                if (error.InnerErrors != null)
                {
                    foreach (var innerError in error.InnerErrors)
                    {
                        errorMessage += getErrorsUntilDone(innerError);
                    }
                }
            }

            return errorMessage;
        }

        /// <summary>
        /// Enpoint para obtener una factura en formato PDF/XML, mediante el servicio de SOVOS
        /// </summary>
        /// <param name="uuid">identificador de la factura</param>
        /// <param name="formato">formato de la factura pdf o xml</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("imprimirComprobanteCFDI/{uuid}/{formato}")]
        public async Task<HttpResponseMessage> imprimirComprobanteCFDI(string uuid, string formato)
        {

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
            JsonMediaTypeFormatter jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            jsonMediaTypeFormatter.SupportedMediaTypes.Add(mediaType);

            try
            {
                string ApiRestReachcoreUrl = ConfigurationManager.AppSettings["ApiRestReachcoreUrl"];
                string endPoint = string.Format("{0}/{1}?uuid={2}&format={3}", "Timbre", "Get", uuid, formato);

                HttpClient httpClient = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(new Uri(ApiRestReachcoreUrl), endPoint)
                };

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("RCApiKey", Globals.Constants.APKD);

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Stream comprobante = await response.Content.ReadAsStreamAsync();
                    //httpResponse.Content = new StreamContent(comprobante);
                    //httpResponse.Content.Headers.ContentType = (formato == "pdf") ? new MediaTypeHeaderValue("application/pdf") : new MediaTypeHeaderValue("application/xml");
                    //ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment");
                    //if (ContentDispositionHeaderValue.TryParse("attachment; filename=" + uuid + "." + formato, out contentDisposition))
                    //{
                    //    httpResponse.Content.Headers.ContentDisposition = contentDisposition;
                    //}
                    //httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    MemoryStream memoryStream = new MemoryStream();
                    await comprobante.CopyToAsync(memoryStream);

                    byte[] data = memoryStream.ToArray();

                    string base64File = Convert.ToBase64String(data);

                    httpResponse.Content = new StringContent(base64File);
                    httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    httpResponse.Content = new ObjectContent<object>(new { Message = responseBody }, jsonMediaTypeFormatter, mediaType);
                }

            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                httpResponse.Content = new ObjectContent<object>(new { Message = "Ocurrio un error inesperado " + ex.Message }, jsonMediaTypeFormatter, mediaType);
            }

            return httpResponse;
        }


        //[HttpPost]
        //[Route("getGuidFromXmlInvoice")]
        //public async Task<HttpResponseMessage> getGuidFromXmlInvoice(object base64Xml)
        //{
        //    HttpResponseMessage response = new HttpResponseMessage();
        //    JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            
        //    try
        //    {
        //        byte[] xmlBytes = Convert.FromBase64String((string)base64Xml);
        //        MemoryStream ms = new MemoryStream(xmlBytes);

        //        string xmlStr = Encoding.UTF8.GetString(xmlBytes);    

        //        Guid uuid = InvoiceXmlHelper.getUUIDFromInvoice(xmlStr);

        //        response.Content = new StreamContent(ms);

        //        ContentDispositionHeaderValue contentDispositionHeader = new ContentDispositionHeaderValue("attachment");

        //        if (ContentDispositionHeaderValue.TryParse("attachment; filename=" + uuid.ToString() + ".xml", out contentDispositionHeader))
        //        {
        //            response.Content.Headers.ContentDisposition = contentDispositionHeader;
        //            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        //        }

        //        response.StatusCode = System.Net.HttpStatusCode.OK;
        //    }
        //    catch (Exception ex)
        //    {
                    
        //        formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        //        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        //        response.Content = new ObjectContent<object>(new { Message = ex.Message, }, formatter);
        //    }

        //    return response;
        //}

        //[HttpGet]
        //[Route("GetXmlFromObject")]
        //public IHttpActionResult GetXmlFromObject()
        //{

        //    string resultXml = "";
        //    TestXmlObject testXmlObject = new TestXmlObject();
        //    testXmlObject.TReceptorXml = new TReceptorXml() { Name = "Felipe Calderón del Sagrado Corazón De Jesús Hinojosa", Age = 60, Gender = "Male" };
        //    testXmlObject.Drinks = new List<Drinks>() 
        //    { 
        //        new Drinks() { Name = "Bacacho", Type = "Rum", Content = 1000 }, 
        //        new Drinks() { Name = "Tonayan", Type = "Charanda", Content = 750 } 
        //    };

        //    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TestXmlObject));
        //    System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();

        //    ns.Add("cfdi", "http://www.sat.gob.mx/cfd/4");
        //    ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

        //    try
        //    {
        //        StringWriterUTF8 sw = new StringWriterUTF8();
        //        serializer.Serialize(sw, testXmlObject, ns);
        //        resultXml = sw.ToString();
        //        return Ok(resultXml);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}


        //[HttpPost]
        //[Route("Base64ToString")]
        //public IHttpActionResult Base64ToString(object base64xmlstring)
        //{
        //    if (base64xmlstring.GetType() == typeof(string))
        //    {
        //        byte[] buffer = Convert.FromBase64String((string)base64xmlstring);

        //        string xml = Encoding.UTF8.GetString(buffer);

        //        return Ok(xml);

        //    } 
        //    else
        //    {
        //        return BadRequest("Por favor envie un string correcto");
        //    }
        //}

    }
}