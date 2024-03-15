using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;


namespace MSSPAPI.Controllers
{
    public class AddDocumentController : ApiController
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: AddDocument
        [Route("api/document/{certificate}")]
        public IHttpActionResult getDocument(string certificate)
        {
            log4net.ThreadContext.Properties["IP"] = HttpContext.Current.Request.UserHostAddress;
            log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
            string jsonresponse = string.Empty;
            string jsonrequest = string.Empty;
            string jsonConfig = string.Empty;

            try
            {
                var policyServiceClient = new MSSPElita4.CertificateDocumentServiceClient(Constants.ENPTNAME, Constants.URLDOCUMENT);
                policyServiceClient.ClientCredentials.UserName.UserName = Constants.USERNAMEA1A2;
                policyServiceClient.ClientCredentials.UserName.Password = Constants.PSWDA1A2;
                Cliente cl = DBOperations.GetClientesById(Constants.IdClienteLiverpool); //Constantes cuando haya mas clientes

                MSSPElita4.AttachCertificateDocumentResponse attachCertificateDocumenthResponse = null;
                jsonConfig = cl.Configuraciones;
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
                try
                {
                    var search = new MSSPElita4.CertificateNumberLookup
                    {
                        DealerCode = jsonObj["DealerCode"]
                        , CertificateNumber = certificate
                    };

                    var document = new MSSPElita4.AttachCertificateDocumentRequest
                    {
                        CertificateSearch = search
                        , Comments = "UPLOAD JPEG TYPE DOCUMENT1"
                        , DocumentType = "true"
                        , FileName = "600013.jpg"
                        , ImageData = null
                        , ScanDate = Convert.ToDateTime("2020-02-20T15:56:34.6463683-05:00")
                        , UserName = "SRINIVASA1"
                    };
                    jsonrequest = JsonConvert.SerializeObject(document);
                    attachCertificateDocumenthResponse = policyServiceClient.AttachDocument(document);
                    jsonresponse = JsonConvert.SerializeObject(attachCertificateDocumenthResponse);
                    Bitacora bt = new Bitacora()
                    {
                        IP = HttpContext.Current.Request.UserHostAddress,
                        Navegador = HttpContext.Current.Request.Browser.Browser,
                        Fecha = DateTime.Now,
                        Usuario = Environment.UserName,
                        Descripcion = "Request and Response a método AddDocument exitosamente."
                    };
                    DBOperations.InsertBitacora(bt);
                }
                catch (FaultException<MSSPElita.CertificateNotFoundFault> fault)
                {
                    log.Error(string.Format("CertificateNotFoundFault - {0}", $"{DateTime.Now},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    BadRequest();
                }
                catch (FaultException<MSSPElita.EnrollFault> fault)
                {
                    log.Error(string.Format("EnrollFault - {0}", $"{DateTime.Now},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    return BadRequest();
                }
                catch (FaultException<MSSPElita.DealerNotFoundFault> fault)
                {
                    log.Error(string.Format("DealerNotFoundFault - {0}", $"{DateTime.Now},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    return BadRequest();
                }
                catch (FaultException<MSSPElita.RegItemFault> fault)
                {
                    log.Error(string.Format("RegItemFault - {0}", $"{DateTime.Now},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", fault.Detail.FaultMessage), fault);
                    return BadRequest();
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Exception - {0}", $"{DateTime.Now},{jsonresponse},{jsonrequest},{HttpContext.Current.Request.UserHostAddress},{HttpContext.Current.Request.Browser.Browser}", ex.Message), ex);
                    return BadRequest();
                }

                return Ok(jsonresponse);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

    }
}