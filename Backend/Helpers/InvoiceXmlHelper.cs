using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace MSSPAPI.Helpers
{
    public static class InvoiceXmlHelper
    {

        public static Guid getUUIDFromInvoice(string xml)
        {
            Guid uuid = Guid.Empty;

            try
            {
                XElement invoice = XElement.Parse((string)xml);
                XNamespace cfdi = invoice.Name.Namespace;


                XElement nodoComplemento = invoice.Element(cfdi + "Complemento");

                XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

                XElement nodoTimbreFiscal = nodoComplemento.Element(tfd + "TimbreFiscalDigital");

                XAttribute uuidTimbreFiscal = nodoTimbreFiscal.Attribute("UUID");



                uuid = Guid.Parse(uuidTimbreFiscal.Value);   
            }
            catch (Exception)
            {
                uuid = Guid.Empty;
            }

            return uuid;
        }

        public static string getBase64FromXML(string xml)
        {

            string base64Str = "";
            try
            {
                byte[] bytesXml = Encoding.UTF8.GetBytes(xml);
                base64Str = Convert.ToBase64String(bytesXml);
            }
            catch (Exception)
            {
                base64Str = "";
            }

            return base64Str;
        }
    }
}