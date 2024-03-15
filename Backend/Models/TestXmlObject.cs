using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace MSSPAPI.Models
{
    [Serializable]
    [XmlRoot(ElementName = "TestXmlObject",Namespace = "http://www.sat.gob.mx/cfd/4")]
    public class TestXmlObject
    {
        private string _schemaLocation = "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd";

        [XmlAttribute(Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string schemaLocation { get { return _schemaLocation; } set { _schemaLocation = value; } }

        [XmlElement]
        public TReceptorXml TReceptorXml { get; set; }


        [XmlArray(ElementName = "Drinks")]
        [XmlArrayItem(ElementName = "Drink")]
        public List<Drinks> Drinks { get; set; }
    }


    public class TReceptorXml
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int Age { get; set; }
        [XmlAttribute]
        public string Gender { get; set; }
    }

    public class Drinks
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public int Content { get; set; }
    }


    public class StringWriterUTF8 : System.IO.StringWriter 
    {
        public override Encoding Encoding {  get { return Encoding.UTF8; } }

    }
}