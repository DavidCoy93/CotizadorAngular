using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MSSPAPI.Models
{
    [XmlRoot("Error")]
    public class ElitaProcessRequestResponseError
    {
        [XmlElement("Code")]
        public string Code { get; set; }
        [XmlElement("Message")]
        public string Message { get; set; }
        [XmlElement("ErrorInfo")]
        public string ErrorInfo { get; set; }
    }
}