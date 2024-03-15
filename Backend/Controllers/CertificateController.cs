using MSSPAPI.Helpers;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http;
using MSSPAPI.Models;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MSSPAPI.ClaimServiceWS;
using Newtonsoft.Json;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Tls;

namespace MSSPAPI.Controllers
{
    public class CertificateController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("api/CertificateReportFTP")]
        public HttpResponseMessage CertificateReportFTP()
        {
            DateTime fechaIni = Convert.ToDateTime("2023-11-08");//DateTime.Today.AddDays(-1);
            DateTime fechaFin = Convert.ToDateTime("2023-11-10");//DateTime.Today.AddSeconds(-1);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            try
            {
                List<ClienteFacturacion> cf = DBOperations.GetAllClientesFact(fechaIni, fechaFin);
                foreach(var item in cf)
                {
                    
                    byte[] buffer = new byte[0];
                    MemoryStream memoryStream = new MemoryStream();
                    ClienteFacturacion issuance = DBOperations.GetClientesFactById(item.Id);
                    try
                    {
                        if (issuance != null)
                        {
                            
                            memoryStream = BuildCertificate(issuance);
                            buffer = memoryStream.ToArray();

                            response.Content = new StreamContent(new MemoryStream(buffer));
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentLength = buffer.Length;
                            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment");
                            if (ContentDispositionHeaderValue.TryParse("inline; filename=" + issuance.CertificateNumber + ".pdf", out contentDisposition))
                            {
                                response.Content.Headers.ContentDisposition = contentDisposition;
                            }
                            response.StatusCode = HttpStatusCode.OK;
                        }
                        else
                        {
                            log.Warn(string.Format("No se recuperó objeto para la Póliza número - {0}", item.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("DownloadCertificate - {0}", ex.Message), ex);
                        throw;
                    }
                    finally
                    {
                        memoryStream.Dispose();
                        memoryStream = null;
                    }
                    
                }
                return response;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("DownloadCertificate - {0}", ex.Message), ex);
                throw;
            }
            
        }

        [HttpGet]
        [Route("api/Certificate/Download/{idCertificate}")]
        public HttpResponseMessage DownloadCertificate(int idCertificate)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            byte[] buffer = new byte[0];
            MemoryStream memoryStream = new MemoryStream();
            ClienteFacturacion issuance = DBOperations.GetClientesFactById(idCertificate); 
            try
            {
                if (issuance != null)
                {
                    memoryStream = BuildCertificate(issuance);
                    buffer = memoryStream.ToArray();

                    response.Content = new StreamContent(new MemoryStream(buffer));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentLength = buffer.Length;
                    ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment");
                    if (ContentDispositionHeaderValue.TryParse("inline; filename=" + issuance.CertificateNumber + ".pdf", out contentDisposition))
                    {
                        response.Content.Headers.ContentDisposition = contentDisposition;
                    }
                    response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    log.Warn(string.Format("No se recuperó objeto para la Póliza número - {0}", idCertificate));
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("DownloadCertificate - {0}", ex.Message), ex);
                throw;
            }
            finally
            {
                memoryStream.Dispose();
                memoryStream = null;
            }
            return response;
        }

        public static bool ValidateB64(string cadena)
        {
            try
            {
                if (!string.IsNullOrEmpty(cadena))
                {
                    byte[] data = Convert.FromBase64String(cadena);
                    return true;
                }
                else
                {
                    cadena = "";
                    return true;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private MemoryStream BuildCertificate(ClienteFacturacion issuance)
        {
            MemoryStream tempStream = new MemoryStream();
            try
            {
                DateTime deducible = new DateTime(2022, 9, 01, 1, 00, 00, 00);
                string filePath;

                Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(issuance.IdCliente)); //Constantes cuando haya mas clientes
                string jsonConfig = cl.Configuraciones;
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                string template = jsonObj["Template"];

                filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data", template);

                PdfReader pdfReader = new PdfReader(filePath);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, tempStream);
                AcroFields pdfFormFields = pdfStamper.AcroFields;

                string domicilio = string.Format("{0}, {1}, {2} {3}, {4}, CP {5}", EncDec.Decript(issuance.Calle), EncDec.Decript(issuance.Numero), EncDec.Decript(issuance.Colonia), EncDec.Decript(issuance.CP), 
                    EncDec.Decript(issuance.Estado) , EncDec.Decript(issuance.Localidad));
                string nombre = string.Format("{0} {1}", EncDec.Decript(issuance.Names), EncDec.Decript(issuance.LastName)).ToUpper();
                //Datos Mazda

                pdfFormFields.SetField("CertificateNumber", issuance.CertificateNumber.ToString());
                pdfFormFields.SetField("NameC", nombre);
                pdfFormFields.SetField("RFCC", issuance.RFC);
                pdfFormFields.SetField("AddressD", domicilio);

                pdfFormFields.SetField("NameA", nombre);
                pdfFormFields.SetField("RFCA", issuance.RFC);
                pdfFormFields.SetField("AddressA", domicilio);

                pdfFormFields.SetField("DayIniEmi", (issuance.DateCreation).Day.ToString());
                pdfFormFields.SetField("MonthIniEmi", (issuance.DateCreation).Month.ToString());
                pdfFormFields.SetField("YearIniEmi", (issuance.DateCreation).Year.ToString());

                pdfFormFields.SetField("DayIniValidity", (issuance.DateInicioPoliza).Day.ToString());
                pdfFormFields.SetField("MonthIniValidity", (issuance.DateInicioPoliza).Month.ToString());
                pdfFormFields.SetField("YearIniValidity", (issuance.DateInicioPoliza).Year.ToString());

                pdfFormFields.SetField("DayEndValidity", (issuance.DateFinPoliza).Day.ToString());
                pdfFormFields.SetField("MonthEndValidity", (issuance.DateFinPoliza).Month.ToString());
                pdfFormFields.SetField("YearEndValidity", (issuance.DateFinPoliza).Year.ToString());

                pdfFormFields.SetField("Money", issuance.Currency);
                pdfFormFields.SetField("Model", issuance.Modelo);
                pdfFormFields.SetField("Brand", issuance.Marca);
                pdfFormFields.SetField("YearAuto", issuance.Anio);
                pdfFormFields.SetField("VIN", issuance.VIN);
                pdfFormFields.SetField("Deductible", "$0.00");
                pdfFormFields.SetField("ExpeditionExpenses", "$0.00");
                pdfFormFields.SetField("NetPremium", Convert.ToDecimal(issuance.PrecioTotal).ToString("C2", CultureInfo.CurrentCulture));
                pdfFormFields.SetField("PremiumBeforeTax", Convert.ToDecimal(issuance.PrecioSIVA).ToString("C2", CultureInfo.CurrentCulture));
                pdfFormFields.SetField("Iva", Convert.ToDecimal(issuance.IVA).ToString("C2", CultureInfo.CurrentCulture));
                pdfFormFields.SetField("TotalPremium", Convert.ToDecimal(issuance.PrecioTotal).ToString("C2", CultureInfo.CurrentCulture));

                //string dia = (DateTime.Today.Day < 10) ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
                //string mes = (DateTime.Today.Month < 10) ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
                //string anio = DateTime.Today.Year.ToString();
                //string fechaActual = string.Format("{0}/{1}/{2}", dia, mes, anio);

                CultureInfo ci = new CultureInfo("es-MX");

                //pdfFormFields.SetField("Day", DateTime.ParseExact(fechaActual, @"dd/MM/yyyy", CultureInfo.InvariantCulture).Day.ToString());
                //pdfFormFields.SetField("Month", DateTime.ParseExact(fechaActual, @"dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MMMM", ci).ToUpper());
                //pdfFormFields.SetField("Year", DateTime.ParseExact(fechaActual, @"dd/MM/yyyy", CultureInfo.InvariantCulture).Year.ToString());

                //pdfFormFields.SetField("de_3", "");
                //pdfFormFields.SetField("con el número", "");
                //pdfFormFields.SetField("undefined_9", "");

                pdfStamper.FormFlattening = true;
                pdfStamper.Writer.CloseStream = false;
                pdfStamper.Close();
                pdfReader.Close();
                tempStream.Position = 0;

                iTextSharp.text.Document mainDocument = new iTextSharp.text.Document(PageSize.LETTER);
                var pdfCopier = new PdfSmartCopy(mainDocument, tempStream) { CloseStream = false };
                pdfCopier.Close();
                return tempStream;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("BuildPóliza - {0}", ex.Message), ex);
                throw ex;
            }
            finally
            {
            }
        }
    }
}