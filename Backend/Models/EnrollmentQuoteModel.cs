using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MSSPAPI.Models
{
    [XmlRoot("VSCQuoteDs", Namespace = "")]
    public class VSCQuoteDs
    {
        [XmlElement("VSCQuote")]
        public VSCQuote VSCQuote { get; set; }
    }

    [XmlRoot("VSCQuote")]
    public class VSCQuote
    {
        [XmlElement("Make")]
        public string Make { get; set; }

        [XmlElement("Year")]
        public int Year { get; set; }

        [XmlElement("Model")]
        public string Model { get; set; }

        [XmlElement("Engine_Version")]
        public string Engine_Version { get; set; }

        [XmlElement("VIN")]
        public string VIN {  get; set; }

        [XmlElement("Mileage")]
        public int Mileage { get; set; }

        [XmlElement("New_Used")]
        public string New_Used { get; set; }

        [XmlElement(ElementName = "In_Service_Date", DataType = "date")]
        public DateTime In_Service_Date { get; set; }

        [XmlElement("Dealer_Code")]
        public string Dealer_Code { get; set; }

        [XmlElement(ElementName = "Warranty_Date", DataType = "date")]
        public DateTime Warranty_Date { get; set; }

        [XmlElement("Vehicle_License_Tag")]
        public string Vehicle_License_Tag { get; set; }

        [XmlElement("Optional")]
        public VscQuoteOptional Optional { get; set; }

        [XmlElement("External_Car_Code")]
        public string External_Car_Code { get; set; }

        [XmlElement("Vehicle_Value")]
        public decimal? Vehicle_Value { get; set; }
    }

    [XmlRoot("Optional")]
    public class VscQuoteOptional
    {
        [XmlElement("Optional_Code")]
        public string Optional_Code { get; set; }
    }
}