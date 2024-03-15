using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace MSSPAPI.Helpers
{
    public class ElitaHelper
    {
        public static string GetDetailsElita ()
        {

            try
            {
                string res = CallWebService();
                return res;
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string CallWebService()
        {
            var _url = "http://elitaplus-modl.a2.assurant.com/ElitaPolicyServiceA2/PolicyService.svc";
            var _action = "http://elitaplus-modl.a2.assurant.com/ElitaPolicyServiceA2/PolicyService.svc?op=PolicyLookup";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                return (soapResult);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(
            @"<soapenv:Envelope
                xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                xmlns:pol=""http://elita.assurant.com/Elita/PolicyService""
                xmlns:pol1=""http://elita.assurant.com/Elita/PolicyService/PolicySearch""
                xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                <soapenv:Header/>
                    <soapenv:Body>
                        <pol:Search>
                            <pol:request>
                                <pol1:PolicyLookup i:type=""pol1:SearchPolicyBySerialNumber"">
                                    <pol1:CompanyCode>ASM</pol1:CompanyCode>
                                    <pol1:SerialNumber>TEST2IMEI2333</pol1:SerialNumber>
                                </pol1:PolicyLookup>
                            </pol:request>
                        </pol:Search>
                </soapenv:Body>
            </soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}