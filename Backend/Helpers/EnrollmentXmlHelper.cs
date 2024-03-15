using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MSSPAPI.Helpers
{
    /// <summary>
    /// Clase para el manejo de clases relacionadas con los metodos de enrollment
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EnrollmentHelper<T>
    {
        /// <summary>
        /// Metodo para convertir una clase en un string en formato xml
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<string> CreateXmlRequest(T request)
        {
            string resultXml = "";

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Auto, Async = true };
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                XElement reqXmlModel = null;

                StringWriterUTF8 stream = new StringWriterUTF8();
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, request, ns);
                    string ModelXml = stream.ToString();
                    reqXmlModel = XElement.Parse(ModelXml);
                    XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                    reqXmlModel.Add(new XAttribute(XNamespace.Xmlns + "xsi", xsi));
                    reqXmlModel.Add(new XAttribute(xsi + "noNamespaceSchemaLocation", "schema.xsd"));
                    writer.Close();
                    stream = null;
                }

                XElement xmlWithoutNullProperties = null;

                if (request.GetType() == typeof(Models.VSCQuoteDs))
                {
                    PropertyInfo property = request.GetType().GetProperty("VSCQuote");
                    Models.VSCQuote data = (Models.VSCQuote)property.GetValue(request, null);
                    xmlWithoutNullProperties = getXmlWithoutNullProperties<Models.VSCQuote>(reqXmlModel, data);
                }
                else if (request.GetType() == typeof(Models.VSCEnrollmentDs))
                {
                    PropertyInfo property = request.GetType().GetProperty("VSCEnrollment");
                    Models.VSCEnrollment data = (Models.VSCEnrollment)property.GetValue(request, null);
                    xmlWithoutNullProperties = getXmlWithoutNullProperties<Models.VSCEnrollment>(reqXmlModel, data);
                }

                stream = new StringWriterUTF8();
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
                {
                    if (xmlWithoutNullProperties != null)
                    {
                        await xmlWriter.WriteCDataAsync(xmlWithoutNullProperties.ToString());
                    }
                    xmlWriter.Close();
                }

                resultXml = stream.ToString();

                resultXml = resultXml.Replace(Environment.NewLine, "").Replace("\"", "'");
                resultXml = Regex.Unescape(resultXml);

                return resultXml;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static XElement getXmlWithoutNullProperties<R>(XElement xml, R request)
        {
            XElement result = null;
            string parentNode = (request.GetType() == typeof(Models.VSCQuote)) ? "VSCQuote" : "VSCEnrollment";
            foreach (PropertyInfo property in request.GetType().GetProperties())
            {
                if (property.PropertyType.IsGenericType && property.GetValue(request, null) == null)
                {
                    XElement mainElement = xml.Element(parentNode);
                    XElement elementToDelete = mainElement.Element(property.Name);
                    if (elementToDelete != null)
                    {
                        elementToDelete.Remove();
                    }
                }
                else if (property.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)property.GetValue(request, null)))
                    {
                        XElement mainElement = xml.Element(parentNode);
                        XElement elementToDelete = mainElement.Element(property.Name);
                        if (elementToDelete != null)
                        {
                            elementToDelete.Remove();
                        }
                    }
                }
                else if (property.PropertyType == typeof(Nullable<decimal>) && property.GetValue(request, null) == null)
                {
                    XElement mainElement = xml.Element(parentNode);
                    XElement elementToDelete = mainElement.Element(property.Name);
                    if (elementToDelete != null)
                    {
                        elementToDelete.Remove();
                    }
                }
            }

            result = xml;

            return result;
        }

        public static T GetDeserializedObject<T>(string xmlResponse) where T : class, new()
        {
            T result = new T();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                byte[] byteArray = Encoding.UTF8.GetBytes(xmlResponse);

                MemoryStream stream = new MemoryStream(byteArray);

                result = (T)serializer.Deserialize(stream);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}