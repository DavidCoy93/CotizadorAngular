using MSSPAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    [RoutePrefix("download")]
    public class DownloadController : ApiController
    {
    //    public object ReponseMsgCode { get; private set; }
    //    private const string DefaultDateFormat = "dd/MM/yyyy";
    //    private readonly string _language = ConfigurationManager.AppSettings["Language"];
    //    private readonly string _countryCode = ConfigurationManager.AppSettings["CountryCode"];


    //    /*This method is used from Exact Target fulfillment email for the pdf certificate download link*/

    //    [HttpGet]
    //    [Route("certificate/{t}")]
    //    public HttpResponseMessage ViewCertificate(string t)
    //    {
    //        HttpResponseMessage result = new HttpResponseMessage();

    //        if (string.IsNullOrEmpty(t))
    //        {
    //            result.StatusCode = HttpStatusCode.BadRequest;
    //            return result;
    //        }

    //        try
    //        {
    //            // var policies = _policyService.GetAll().Where(x => x.DownloadToken == t).ToList();
    //            var policies = .GetPoliciesOfRespectiveDownloadToken(t);

    //            if (policies != null && policies.Count() > 0)
    //            {
    //                if (((DateTime)policies[0].CreateDate).AddYears(1) <= DateTime.Now) //1 year expiration date.
    //                {
    //                    throw new System.Exception("Expired");
    //                }
    //                policies = policies.OrderByDescending(x => x.FromDate).ToList();

    //                string fromdate = DateTime.ParseExact(policies[0].FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);
    //                string todate = (DateTime.ParseExact(fromdate, DefaultDateFormat, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1)).ToString(DefaultDateFormat);

    //                //Corporate
    //                if (policies[0].IdentificationNumber.Length == 11 && (policies[0].IdentificationNumber.StartsWith("30") ||
    //                                                                     policies[0].IdentificationNumber.StartsWith("33") ||
    //                                                                     policies[0].IdentificationNumber.StartsWith("34")))
    //                {
    //                    if (policies.Count() == 1) //Unique
    //                    {
    //                        var corporateUniquePDFModel = new CorporateUniquePDF()
    //                        {
    //                            CUSTOMER_NAME = policies[0].CustomerName,
    //                            ADDRESS = policies[0].Address,
    //                            POSTAL_CODE = policies[0].PostalCode,
    //                            CITY = policies[0].City,
    //                            PROVINCE = policies[0].Province,
    //                            CERT_NUMBER = policies[0].CertificateNumber,
    //                            FROM_DATE = fromdate,
    //                            TO_DATE = todate,
    //                            POLICY_NUMBER = policies[0].PolicyNumber,
    //                            TAX_ID = policies[0].IdentificationNumber,
    //                            WORK_PHONE = policies[0].WorkPhone,
    //                            EMAIL = policies[0].Email,
    //                            MAIN_ACTIVITY = "",
    //                            MANUFACTURER = policies[0].Manufacturer,
    //                            MODEL = policies[0].Model,
    //                            SERIAL_NUMBER = policies[0].SerialNumber,
    //                            PREMIUM = policies[0].Prmium
    //                        };

    //                        var base64StringPDF = _pdfService.GenerateCorporateUniquePDF(corporateUniquePDFModel);

    //                        result.StatusCode = HttpStatusCode.OK;
    //                        GeneratePdf(result, base64StringPDF);

    //                        policies[0].Downloaded = true;
    //                        policies[0].DownloadedDate = DateTime.Now;
    //                        _policyService.Update(policies[0]);
    //                    }
    //                    else //Multiple
    //                    {
    //                        var corporateMultiplePDFModel = new CorporateUniquePDF()
    //                        {
    //                            POLICY_NUMBER = policies[0].PolicyNumber,
    //                            CUSTOMER_NAME = policies[0].CustomerName,
    //                            TAX_ID = policies[0].IdentificationNumber,
    //                            ADDRESS = policies[0].Address,
    //                            WORK_PHONE = policies[0].WorkPhone,
    //                            EMAIL = policies[0].Email,
    //                            MAIN_ACTIVITY = ""
    //                        };

    //                        var certList = new List<CertDetail>();
    //                        foreach (var policy in policies)
    //                        {
    //                            //DateTime fd1 = DateTime.Parse(policy.FromDate); // returns 09/25/2011
    //                            //string fd2 = fd1.ToString(DefaultDateFormat); //should return 25/09/2011

    //                            string fd = DateTime.ParseExact(policy.FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);
    //                            // string fd = fromdate;

    //                            //DateTime td1 = DateTime.Parse(policy.ToDate); // returns 09/25/2011
    //                            //string td2 = td1.ToString(DefaultDateFormat); //should return 25/09/2011

    //                            string td = DateTime.ParseExact(policy.FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1).ToString(DefaultDateFormat);
    //                            //string td = DateTime.ParseExact(policy.ToDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);

    //                            certList.Add(new CertDetail()
    //                            {
    //                                CERT_NUMBER = policy.CertificateNumber,
    //                                MANUFACTURER = policy.Manufacturer,
    //                                MODEL = policy.Model,
    //                                SERIAL_NUMBER = policy.SerialNumber,
    //                                WORK_PHONE = policy.WorkPhone,
    //                                FROM_DATE = fd,
    //                                TO_DATE = td,
    //                                PREMIUM = policy.Prmium
    //                            });

    //                            policy.Downloaded = true;
    //                            policy.DownloadedDate = DateTime.Now;
    //                            _policyService.Update(policy);
    //                        }

    //                        corporateMultiplePDFModel.CertList = certList;

    //                        var base64StringPDF = _pdfService.GenerateCorporateMultiplePDF(corporateMultiplePDFModel);

    //                        result.StatusCode = HttpStatusCode.OK;
    //                        GeneratePdf(result, base64StringPDF);
    //                    }
    //                }
    //                else //individual
    //                {
    //                    if (policies.Count() == 1) //Unique
    //                    {
    //                        //DateTime fd1 = DateTime.Parse(policies[0].FromDate); // returns 09/25/2011
    //                        //string fd2 = fd1.ToString(DefaultDateFormat); //should return 25/09/2011

    //                        string fd = DateTime.ParseExact(policies[0].FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);
    //                        // string fd = fromdate;

    //                        //DateTime td1 = DateTime.Parse(policies[0].ToDate); // returns 09/25/2011
    //                        //string td2 = td1.ToString(DefaultDateFormat); //should return 25/09/2011

    //                        string td = DateTime.ParseExact(fd, DefaultDateFormat, CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1).ToString(DefaultDateFormat);
    //                        //string td = DateTime.ParseExact(policies[0].ToDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);

    //                        var individualUniquePDFModel = new IndividualUniquePDF()
    //                        {
    //                            CERT_NUMBER = policies[0].CertificateNumber,
    //                            FROM_DATE = fd,
    //                            TO_DATE = td,
    //                            POLICY_NUMBER = policies[0].PolicyNumber,
    //                            CUSTOMER_NAME = policies[0].CustomerName,
    //                            DATE_PLACE_OF_BIRTH = "",
    //                            NATIONALITY = "",
    //                            GENDER = "",
    //                            MARTIAL_STATUS = "",
    //                            ADDRESS = policies[0].Address,
    //                            TAX_ID = policies[0].IdentificationNumber,
    //                            CUIL = "",
    //                            WORK_PHONE = policies[0].WorkPhone,
    //                            EMAIL = policies[0].Email,
    //                            PROFESSION = "",
    //                            MANUFACTURER = policies[0].Manufacturer,
    //                            MODEL = policies[0].Model,
    //                            SERIAL_NUMBER = policies[0].SerialNumber,
    //                            PREMIUM = policies[0].Prmium
    //                        };

    //                        var base64StringPDF = _pdfService.GenerateIndividualUniquePDF(individualUniquePDFModel);

    //                        result.StatusCode = HttpStatusCode.OK;
    //                        GeneratePdf(result, base64StringPDF);

    //                        policies[0].Downloaded = true;
    //                        policies[0].DownloadedDate = DateTime.Now;
    //                        _policyService.Update(policies[0]);
    //                    }
    //                    else //multiple
    //                    {
    //                        var individualMultiplePDFModel = new IndividualMultiplePDF()
    //                        {
    //                            POLICY_NUMBER = policies[0].PolicyNumber,
    //                            CUSTOMER_NAME = policies[0].CustomerName,
    //                            DATE_PLACE_OF_BIRTH = "",
    //                            NATIONALITY = "",
    //                            GENDER = "",
    //                            MARTIAL_STATUS = "",
    //                            ADDRESS = policies[0].Address,
    //                            TAX_ID = policies[0].IdentificationNumber,
    //                            CUIL = "",
    //                            WORK_PHONE = policies[0].WorkPhone,
    //                            EMAIL = policies[0].Email,
    //                            PROFESSION = ""
    //                        };

    //                        var certList = new List<CertDetail>();
    //                        foreach (var policy in policies)
    //                        {
    //                            //DateTime fd1 = DateTime.Parse(policy.FromDate); // returns 09/25/2011
    //                            //string fd2 = fd1.ToString(DefaultDateFormat); //should return 25/09/2011

    //                            string fd = DateTime.ParseExact(policy.FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);
    //                            // string fd = fromdate;

    //                            //DateTime td1 = DateTime.Parse(policy.ToDate); // returns 09/25/2011
    //                            //string td2 = td1.ToString(DefaultDateFormat); //should return 25/09/2011

    //                            string td = DateTime.ParseExact(policy.FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1).ToString(DefaultDateFormat);
    //                            //string td = DateTime.ParseExact(policy.ToDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(DefaultDateFormat);

    //                            certList.Add(new CertDetail()
    //                            {
    //                                CERT_NUMBER = policy.CertificateNumber,
    //                                MANUFACTURER = policy.Manufacturer,
    //                                MODEL = policy.Model,
    //                                SERIAL_NUMBER = policy.SerialNumber,
    //                                WORK_PHONE = policy.WorkPhone,
    //                                FROM_DATE = fd,
    //                                TO_DATE = td,
    //                                PREMIUM = policy.Prmium
    //                            });

    //                            policy.Downloaded = true;
    //                            policy.DownloadedDate = DateTime.Now;
    //                            _policyService.Update(policy);
    //                        }

    //                        individualMultiplePDFModel.CertList = certList;

    //                        var base64StringPDF = _pdfService.GenerateIndividualMultiplePDF(individualMultiplePDFModel);

    //                        result.StatusCode = HttpStatusCode.OK;
    //                        GeneratePdf(result, base64StringPDF);
    //                    }
    //                }
    //            }
    //        }
    //        catch (System.Exception e)
    //        {
    //            result.StatusCode = HttpStatusCode.InternalServerError;
    //            result.Content = new StringContent("there are some technical difficulties. please try again later.");
    //            _logService.LogError("Download Certificate error", "", $"token : {t} message : {e.Message}");
    //        }

    //        return result;
    //    }

    //    /// <summary>
    //    /// Download link in sent email will call this action method to open PDF in browser
    //    /// </summary>
    //    /// <param name="t">Download Token</param>
    //    /// <returns>PDF gets open in browser</returns>
    //    [HttpGet]
    //    [Route("Fravegacertificate/{t}")]
    //    public HttpResponseMessage ViewCertificateForFravega(string t)
    //    {
    //        HttpResponseMessage result = new HttpResponseMessage();

    //        if (string.IsNullOrEmpty(t))
    //        {
    //            result.StatusCode = HttpStatusCode.BadRequest;
    //            return result;
    //        }

    //        try
    //        {
    //            //Same token is fetched from the database .
    //            // var policies = _fravegaPolicyService.GetAll().Where(x => x.DownloadToken == t).ToList();
    //            var policies = _fravegaPolicyService.GetFravegaPoliciesOfRespectiveDownloadToken(t);
    //            if (policies != null && policies.Count() > 0)
    //            {
    //                if (((DateTime)policies[0].CreateDate).AddYears(1) <= DateTime.Now) //1 year expiration date.
    //                {
    //                    throw new System.Exception("Expired");
    //                }

    //                if (policies.Count() == 1) //Unique
    //                {
    //                    var fravegaIndividualUniquePDFModel = new FravegaIndividualUniquePDF()
    //                    {
    //                        CustomerName = policies[0].CustomerName,
    //                        IdentificationNumber = policies[0].IdentificationNumber,
    //                        BirthDate = policies[0].BirthDate,
    //                        NacionalIDAD = policies[0].NacionalIDAD,
    //                        CertificateNumber = policies[0].CertificateNumber,

    //                        Ciudad = policies[0].Ciudad,
    //                        CostoNeto = policies[0].CostoNeto,
    //                        CuitCuil = policies[0].CuitCuil,
    //                        Domicilio = policies[0].Domicilio,
    //                        Email = policies[0].Email,
    //                        FechaEmision = policies[0].FechaEmision,
    //                        FinGarantiaExt = policies[0].FinGarantiaExt,
    //                        FravegaSACIEI = "Fravega S.A.C.I.E.I",
    //                        GenderID = policies[0].GenderID,
    //                        HomePhone = policies[0].HomePhone,

    //                        InicioGarantiaExt = policies[0].InicioGarantiaExt,
    //                        InicioGarantiaFab = policies[0].InicioGarantiaFab,
    //                        InvoiceNumber = policies[0].InvoiceNumber,
    //                        Iva = policies[0].Iva,
    //                        LugarEmision = policies[0].LugarEmision,
    //                        Manufacturer_Model = policies[0].Manufacturer + "/" + policies[0].Model,
    //                        MaritalStatusID = policies[0].MaritalStatusID,

    //                        PeriodoGarantiaFab = policies[0].PeriodoGarantiaFab,
    //                        PlaceOfBirthID = policies[0].PlaceOfBirthID,
    //                        Policy = policies[0].Policy,
    //                        PostalCode = policies[0].PostalCode,
    //                        Provincia = policies[0].Provincia,
    //                        Reparacion = policies[0].Reparacion,
    //                        SalesPrice = policies[0].SalesPrice,
    //                        Sucursal = policies[0].Sucursal,
    //                        SumGrossAmt = policies[0].SumGrossAmt,
    //                        TipoCobertura = policies[0].TipoCobertura,
    //                        WarrantySalesDate = policies[0].WarrantySalesDate
    //                    };

    //                    var base64StringPDF = _pdfService.GenerateFravegaIndividualUniquePDF(fravegaIndividualUniquePDFModel);

    //                    result.StatusCode = HttpStatusCode.OK;
    //                    GeneratePdf(result, base64StringPDF);

    //                    policies[0].Downloaded = true;
    //                    policies[0].DownloadedDate = DateTime.Now;
    //                    _fravegaPolicyService.Update(policies[0]);
    //                }
    //            }
    //        }
    //        catch (System.Exception e)
    //        {
    //            result.StatusCode = HttpStatusCode.InternalServerError;
    //            result.Content = new StringContent("there are some technical difficulties. please try again later.");
    //            _logService.LogError("Download Certificate error", "", $"token : {t} message : {e.Message}");
    //        }

    //        return result;
    //    }

    //    [HttpGet]
    //    [Route("certificateportal")]
    //    public HttpResponseMessage ViewCertificateFromPortal(string certificateNumber, string DealerCode)
    //    {
    //        return PerformDownload(certificateNumber, DealerCode);
    //    }

    //    [HttpPost]
    //    [HMACAuthentication]
    //    [Route("DownloadCertificate")]
    //    public HttpResponseMessage DownloadCertificate(CertificateDownloadRequest request)
    //    {
    //        return PerformDownload(request.CertificateNumber, request.DealerCode);
    //    }

    //    //[HttpPost]
    //    //[HMACAuthentication]
    //    //[Route("DownloadCertificateForClient")]
    //    //public HttpResponseMessage DownloadCertificateForClient(CertificateDownloadForClientRequest request)
    //    //{
    //    //    return PerformDownload(request.CertificateNumber, request.DealerCode, request.ClientName);
    //    //}
    //    [HttpGet]
    //    [Route("DownloadCertificateForClient")]
    //    public HttpResponseMessage DownloadCertificateForClient(string CertificateNumber, string DealerCode, string ClientName)
    //    {
    //        return PerformDownload(CertificateNumber, DealerCode, ClientName);
    //    }

    //    private HttpResponseMessage CreatePdfGenerationFailedResult(HttpResponseMessage result, string nucertificateNumber)
    //    {
    //        result.StatusCode = HttpStatusCode.ExpectationFailed;
    //        result.Content = new StringContent("Certificate PDF file not created");
    //        string nMessage = "Certificate PDF file not created.";
    //        _logService.LogError("Download Certificateportal error", "", $"token : {nucertificateNumber} message : {nMessage}");
    //        return result;
    //    }

    //    private static void SetPdfResult(HttpResponseMessage result, string windowsServiceHeader, byte[] pdfByte)
    //    {
    //        result.StatusCode = HttpStatusCode.OK;
    //        if (windowsServiceHeader != null && windowsServiceHeader.ToUpper() == "BYTECONTENT")
    //        {
    //            result.Content = new StringContent(JsonConvert.SerializeObject(new
    //            {
    //                FileName = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-certificado.pdf",
    //                FileContent = Convert.ToBase64String(pdfByte),
    //                Error = new ErrorResponse()
    //            }));
    //        }
    //        else
    //        {
    //            MemoryStream streamc = new MemoryStream(pdfByte);
    //            result.Content = new StreamContent(streamc);
    //            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
    //            result.Content.Headers.ContentDisposition =
    //                new System.Net.Http.Headers.ContentDispositionHeaderValue(dispositionType: "Inline")
    //                {
    //                    FileName = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-certificado.pdf"
    //                };
    //        }
    //    }

    //  //  [HttpGet]
    //   // [Route("encryptcertificate")] NOt used by front end
    //    public HttpResponseMessage GetEncrypted(string certificateNumber, string DealerCode)
    //    {
    //        EcryptResponse resp = new EcryptResponse();
    //        try
    //        {
    //            resp.CertificateEncrypted = EncryptionMethods.Encrypt(certificateNumber);
    //            resp.DealerCodeEncrypted = EncryptionMethods.Encrypt(DealerCode);
    //        }
    //        catch (System.Exception e)
    //        {
    //            resp.Error.ErrorCode = (int)ResponseMsgCode.EncryptionFailed;
    //            resp.Error.ErrorMessage = "Encryption failed";
    //            _logService.LogInfo("encrypt", resp.Error.ErrorMessage, e.Message);
    //            return ToHttpResponseMessage(HttpStatusCode.ExpectationFailed, resp);
    //        }
    //        return ToHttpResponseMessage(HttpStatusCode.OK, resp);
    //    }
    //    //public HttpResponseMessage GetEncrypted(string certificateNumber, string DealerCode, string ClientName="")
    //    //{
    //    //    EcryptResponse resp = new EcryptResponse();
    //    //    try
    //    //    {
    //    //        var encyptiondata = _encryptionService.GetAll().FirstOrDefault(x => x.ClientName == ClientName);


    //    //        resp.CertificateEncrypted = EncryptionMethods.Encrypt(certificateNumber,encyptiondata.Salt, encyptiondata.Password, encyptiondata.Iterations);
    //    //        resp.DealerCodeEncrypted = EncryptionMethods.Encrypt(DealerCode, encyptiondata.Salt, encyptiondata.Password, encyptiondata.Iterations);
    //    //    }
    //    //    catch (System.Exception e)
    //    //    {
    //    //        resp.Error.ErrorCode = (int)ResponseMsgCode.EncryptionFailed;
    //    //        resp.Error.ErrorMessage = "Encryption failed";
    //    //        _logService.LogInfo("encrypt", resp.Error.ErrorMessage, e.Message);
    //    //        return ToHttpResponseMessage(HttpStatusCode.ExpectationFailed, resp);
    //    //    }
    //    //    return ToHttpResponseMessage(HttpStatusCode.OK, resp);
    //    //}

    //    private int MonthsDifference(int purchMonth, int purchYear, int nowMonth, int nowYear)
    //    {
    //        int wholeMonthsDiff = ((nowYear * 12) + nowMonth) - ((purchYear * 12) + purchMonth);
    //        //if (date2.Day < date1.Day)
    //        //    wholeMonthsDiff--;
    //        //var diff = date2 - date1.AddMonths(wholeMonthsDiff);
    //        int monthsDifference = wholeMonthsDiff;// +(diff.Days / 30d);

    //        int monthsApart = 12 * (purchYear - nowYear) + purchMonth - nowMonth;
    //        var nn = Math.Abs(monthsApart);

    //        return monthsDifference;
    //    }

    //    public static int GetMonthsBetween(DateTime from, DateTime to)
    //    {
    //        if (from > to) return GetMonthsBetween(to, from);

    //        var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

    //        if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
    //        {
    //            return monthDiff - 1;
    //        }
    //        else
    //        {
    //            double month = (to - from).Days / 30;

    //            return monthDiff;
    //        }
    //    }

    //    private static void GeneratePdf(HttpResponseMessage result, string base64StringPDF)
    //    {
    //        var pdfByte = Convert.FromBase64String(base64StringPDF);

    //        MemoryStream stream = new MemoryStream(pdfByte);
    //        result.Content = new StreamContent(stream);
    //        result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
    //        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue(dispositionType: "Inline");
    //        result.Content.Headers.ContentDisposition.FileName = string.Format("{0}-{1}-{2}-certificado.pdf", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
    //    }
        
    //    private HttpResponseMessage PerformDownload(string certificateNumber, string DealerCode, string clientName ="") //this is for PIL
    //    {
    //        #region Validation for Email Automation request
    //        HttpResponseMessage result = new HttpResponseMessage();
    //        if (string.IsNullOrEmpty(certificateNumber) || string.IsNullOrEmpty(DealerCode))
    //        {
    //            result.StatusCode = HttpStatusCode.BadRequest;
    //            return result;
    //        }

    //        var request = Request;
    //        var headers = request.Headers.ToDictionary(k => k.Key, v => v.Value);
    //        string windowsServiceHeader = null;

    //        #endregion Validation for Email Automation request

    //        XMLService nuData = new XMLService();
    //        // HttpResponseMessage result = new HttpResponseMessage();
    //        var xPressClient = new OCService(); // new XPService();//
    //        string countryCode = _countryCode;

    //        try
    //        {
    //            if (headers.Keys.Contains("X-Response"))
    //            {
    //                windowsServiceHeader = EncryptionMethods.Decrypt(headers["X-Response"]?.FirstOrDefault());
    //            }

    //            var encyptiondata = _encryptionService.GetAll().FirstOrDefault(x => x.ClientName == clientName) ;

    //            string nucertificateNumber = "";
    //            string nuDealerCode = "";

    //            if (encyptiondata == null)
    //            {
    //                nucertificateNumber = EncryptionMethods.Decrypt(certificateNumber.Replace(' ', '+'));
    //                nuDealerCode = EncryptionMethods.Decrypt(DealerCode.Replace(' ', '+'));
    //            }
    //            else
    //            {
    //                nucertificateNumber = EncryptionMethods.Decrypt(certificateNumber.Replace(' ', '+'),encyptiondata.Salt,encyptiondata.Password,encyptiondata.Iterations);
    //                nuDealerCode = EncryptionMethods.Decrypt(DealerCode.Replace(' ', '+'), encyptiondata.Salt, encyptiondata.Password, encyptiondata.Iterations);

    //            }

    //            if (!nuDealerCode.Equals(ConfigurationManager.AppSettings["AgencyCode"]))
    //            {
    //                var dealerFilter = _dealerCodeFilterService.GetAll();//Get DEALER CODE FILTER

    //                var policies = _pilService.GetCertificate(nucertificateNumber, _language, countryCode, nuDealerCode, ConfigurationManager.AppSettings["ElitaSystem"]);
    //                if (policies != null)
    //                {
    //                    #region ELITA

    //                    #region For multiple models(products)

    //                    string polNumber = policies.PolicyCore.ContractPolicyNumber;
    //                    string latestExpDate = string.Empty;
    //                    string nuSerial = string.Empty;
    //                    string nuModel = string.Empty;
    //                    string nuManufact = string.Empty;
    //                    if (policies.Product.Count() > 1)
    //                    {
    //                        // if ()
    //                        switch (policies.PolicyCore.DealerNumber.ToUpper())
    //                        {
    //                            case "ITBC":  //SmartPhone Adic template
    //                                polNumber = "9090";
    //                                break;

    //                            case "ITIP": //IPHONE Adic template
    //                                polNumber = "8080";
    //                                break;
    //                        }
    //                        //Get lastest end date if multiple products
    //                        var latestend = dealerFilter.Where(
    //                            i => i.DealerCode.ToUpper() == policies.PolicyCore.DealerNumber.ToUpper() &&
    //                            i.Country.ToUpper() == countryCode &&
    //                            i.LogicCode == "enddate");

    //                        if (latestend.Any())
    //                        {
    //                            //latestExpDate = policies.Product.Max(d => d.ExpirationDate);
    //                            var nuItem = policies.Product.Select(x => x).OrderByDescending(d => d.EffectiveDate).FirstOrDefault();
    //                            latestExpDate = nuItem.ExpirationDate;
    //                            nuModel = nuItem.Model;
    //                            nuSerial = nuItem.SerialNumber;
    //                            nuManufact = nuItem.Manufacturer;
    //                        }
    //                    }

    //                    #endregion For multiple models(products)

    //                    #region For Monthly policies

    //                    string expirationDate = string.Empty;
    //                    //end date 30 days for monthly policies
    //                    var monthly = dealerFilter.Where(
    //                                                    i => i.DealerCode.ToUpper() == policies.PolicyCore.DealerNumber.ToUpper() &&
    //                                                    i.Country.ToUpper() == countryCode &&
    //                                                    i.LogicCode == "30days");
    //                    if (monthly.Any())
    //                    {
    //                        expirationDate = DateTime.Now.ToShortDateString();
    //                    }

    //                    #endregion For Monthly policies                       

    //                    #region Calculates coverage length - lastest end date - end date 30 days

    //                    for (int i = 0; i < policies.Product.Count(); i++)
    //                    {
    //                        if (expirationDate.Trim().Length > 0)
    //                        {
    //                            string nuExpDate = (Convert.ToDateTime(policies.Product[i].EffectiveDate).AddMonths(1).AddDays(-1)).ToString("yyyy-MM-dd");//end date 30 days for monthly policies
    //                            policies.Product[i].ExpirationDate = nuExpDate;
    //                        }
    //                        for (int x = 0; x < policies.Product[i].Coverage.Count(); x++)
    //                        {
    //                            if (latestExpDate.Trim().Length > 0)
    //                            {
    //                                policies.Product[i].Coverage[x].EffectiveEndingOn = latestExpDate;
    //                                policies.Product[i].SerialNumber = nuSerial;
    //                                policies.Product[i].Model = nuModel;
    //                                policies.Product[i].Manufacturer = nuManufact;
    //                            }//Get lastest end date if multiple products

    //                            if (policies.Product[i].Coverage[x].CoverageName.ToUpper() == ConfigurationManager.AppSettings["Extended"])
    //                            {
    //                                DateTime startdate1 = Convert.ToDateTime(policies.Product[i].Coverage[x].EffectiveStartingOn);
    //                                DateTime enddate1 = Convert.ToDateTime(policies.Product[i].Coverage[x].EffectiveEndingOn);

    //                                //int monthsDifference = MonthsDifference(startdate1.Month, startdate1.Year, enddate1.Month, enddate1.Year);// + 1;

    //                                double month = (enddate1 - startdate1).Days / 30;
    //                                //set the coverage length to collection
    //                                policies.Product[i].Coverage[x].MethodOfRepairCode = Convert.ToInt32(month).ToString();// monthsDifference.ToString();
    //                            }
    //                        }
    //                    }

    //                    #endregion Calculates coverage length - lastest end date - end date 30 days     
                        
    //                    byte[] pdfByte = new byte[1000];

    //                    //Get dealer code from table to compare with elita dealer code
    //                    var dealercode = _elitaDealerCodeService.GetAll()
    //                                     .Where(i => i.DealerCode.ToUpper() == policies.PolicyCore.DealerNumber.ToUpper());                        
                        
    //                    if (dealercode.Any())
    //                    {
    //                        var asposeService = new AsposePDFService();
    //                        pdfByte = asposeService.GenElitaPDF(policies);
    //                    }
    //                    else
    //                    {
    //                        //var templateid = _xPressionTemplateService.GetAll().FirstOrDefault(x => x.Policy == Convert.ToInt64(polNumber))?.Template;
    //                        // var template = _xPressionTemplateNewService.FindAll().FirstOrDefault(x => x.DealerCode == nuDealerCode)?.Template;
    //                        var availableTemplatesOfDealer = _xPressionTemplateNewService.FindAll().Where(x => x.DealerCode == nuDealerCode).ToList();
    //                        string template = null;
    //                        if (availableTemplatesOfDealer.Count > 1)
    //                        {
    //                            template = availableTemplatesOfDealer.Where(x => x.ProductCode == policies.PolicyCore.PlanCode).FirstOrDefault()?.Template;
    //                        }
    //                        else
    //                            template = availableTemplatesOfDealer.FirstOrDefault()?.Template;
    //                        if (template == null)
    //                        {
    //                            OCService oc = new OCService();
    //                            oc.SendDownloadNotificationToBackOffice(nuDealerCode, nucertificateNumber, "middleware database", "Description: " + "Dealer not found in middleware database");

    //                            return CreatePdfGenerationFailedResult(result, nucertificateNumber);
    //                        }
    //                        //Build data XML string
    //                        string getCustomerData = nuData.GetXMLFromObject(policies);

    //                        //Make Xpression call
    //                        pdfByte = xPressClient.ReturnPDFDoc(template, getCustomerData, nuDealerCode, nucertificateNumber);
    //                    }

    //                    if (pdfByte == null)
    //                    {
    //                        return CreatePdfGenerationFailedResult(result, nucertificateNumber);
    //                    }

    //                    SetPdfResult(result, windowsServiceHeader, pdfByte);

    //                    #endregion ELITA
    //                }
    //                else
    //                    throw new System.Exception("Policy detail not found in Elita.");
    //            }
    //            else
    //            {
    //                var policiesALB = _pilService.GetCertificate(nucertificateNumber, _language, countryCode, nuDealerCode, ConfigurationManager.AppSettings["AlboradaSystem"]);
    //                if (policiesALB != null)
    //                {
    //                    #region ALBORADA

    //                    var asposeService = new AsposePDFService();

    //                    byte[] pdfByte = asposeService.GenAlboradaPDF(policiesALB);

    //                    if (pdfByte == null)
    //                    {
    //                        return CreatePdfGenerationFailedResult(result, nucertificateNumber);
    //                    }

    //                    SetPdfResult(result, windowsServiceHeader, pdfByte);

    //                    #endregion ALBORADA
    //                }
    //                else
    //                    throw new System.Exception("Policy detail not found in Alborada.");
    //            }
    //        }
    //        catch (System.Exception e)
    //        {
    //            result.StatusCode = HttpStatusCode.InternalServerError;
    //            result.Content = new StringContent("there are some technical difficulties. please try again later.");
    //            _logService.LogError("Download Certificate error", "", $"token : {certificateNumber} message : {e.Message} exception: {e.ToString()}");
    //        }
    //        // WriteLog(nucertificateNumber,nuDealerCode)
    //        return result;
    //    }

    //    //public static readonly List<string> elitaDealerCode = new List<string>() {           

    //    //    "MOAA",
    //    //    "MOAB",
    //    //    "MOAC",
    //    //    "MOAD",
    //    //    "MOAE",
    //    //    "MOAF",
    //    //    "MOAG",
    //    //    "MOAH",
    //    //    "MOAI",
    //    //    "MOAJ",
    //    //    "MOAK",
    //    //    "MOAL",

    //    //    "HWAA",
    //    //    "HWAB",
    //    //    "HWAC",
    //    //    "HWAD",
    //    //    "HWAE",
    //    //    "HWAF",
    //    //    "HWAG",
    //    //    "HWAH",
    //    //    "HWAI",
    //    //    "HWAJ",
    //    //    "HWAK",
    //    //    "HWAL",
    //    //    "HWAM",
    //    //    "HWAN",
    //    //    "HWAO",
    //    //    "HWAP",
            
    //    //    "BCAA",
    //    //    "BCAB",
    //    //    "BCAE",
    //    //    "BCAF",

    //    //    "GEBA",
    //    //    "GEBB",
    //    //    "GEBC",
    //    //    "GEBD",
    //    //    "GEBE",
    //    //    "GEBF",
    //    //    "RDAA",
    //    //    "RDAB",
    //    //    "RDAC",
    //    //    "RDAE",
    //    //    "RDAF",
    //    //    "RDAG",
    //    //    "RDAH",
    //    //    "RDAI",
    //    //    "RDBA",
    //    //    "RDBB",
    //    //    "RDBC",

    //    //    "MO01",
    //    //    "MO03",
    //    //    "PICA",
    //    //    "PICK",
    //    //    "PICM",
    //    //    "PICO",
    //    //    "PICQ",
    //    //    "PICC",
    //    //    "PICI",
    //    //    "PICE",
    //    //    "PICH",
    //    //    "MO02",

    //    //    "MLAA",
    //    //    "MLPB",
    //    //    "0011"
    //    //};
    }
}