using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MSSPAPI.Models
{
    [XmlRoot("VSC_QUOTE_ENGINE")]
    public class VSC_QUOTE_ENGINE
    {
        [XmlElement("VSC_QUOTE_HEADER")]
        public VSC_QUOTE_HEADER VSC_QUOTE_HEADER { get; set; }
        [XmlElement("VSC_QUOTE_ITEM")]
        public VSC_QUOTE_ITEM[] VSC_QUOTE_ITEM { get; set; }
        [XmlElement("PAYMENT_TYPES")]
        public PAYMENT_TYPES[] PAYMENT_TYPES { get; set; }
    }

    public class VSC_QUOTE_HEADER
    {
        [XmlElement("QUOTE_NUMBER")]
        public int QUOTE_NUMBER { get; set; }

        [XmlElement("MAKE")]
        public string MAKE { get; set; }

        [XmlElement("MODEL")]
        public string MODEL { get; set; }

        [XmlElement("DEALER")]
        public string DEALER { get; set; }

        [XmlElement("MODEL_YEAR")]
        public short MODEL_YEAR { get; set; }

        [XmlElement("CLASS_CODE")]
        public string CLASS_CODE { get; set; }

        [XmlElement("VIN")]
        public string VIN { get; set; }

        [XmlElement("ODOMETER")]
        public int ODOMETER { get; set; }

        [XmlElement("VEHICLE_LICENSE_TAG")]
        public string VEHICLE_LICENSE_TAG { get; set; }

        [XmlElement("ENGINE_VERSION")]
        public string ENGINE_VERSION { get; set; }

        [XmlElement(ElementName = "IN_SERVICE_DATE", DataType = "dateTime")]
        public DateTime IN_SERVICE_DATE { get; set; }

        [XmlElement("NEW_USED")]
        public string NEW_USED { get; set; }

        [XmlElement("REMAINING_MFG_DAYS")]
        public short REMAINING_MFG_DAYS { get; set; }

        [XmlElement("SALES_TAXES")]
        public double SALES_TAXES {  get; set; }
    }

    public class VSC_QUOTE_ITEM
    {
        [XmlElement("PLAN")]
        public string PLAN { get; set; }

        [XmlElement("PLAN_CODE")]
        public int PLAN_CODE { get; set; }

        [XmlElement("DEDUCTIBLE")]
        public double DEDUCTIBLE { get; set; }

        [XmlElement("TERM_MONTHS")]
        public int TERM_MONTHS { get; set; }

        [XmlElement("TERM_KM_MI")]
        public int TERM_KM_MI { get; set; }

        [XmlElement("ENGINE_WARR_MONTHS")]
        public int ENGINE_WARR_MONTHS { get; set; }

        [XmlElement("ENGINE_WARR_KM_MI")]
        public int ENGINE_WARR_KM_MI { get; set; }

        [XmlElement("RATE")]
        public double RATE { get; set; }

        [XmlElement("QUOTE_ITEM_NUMBER")]
        public int QUOTE_ITEM_NUMBER { get; set; }

        [XmlElement("MAX_INSTALLMENTS_ALLOWED")]
        public int MAX_INSTALLMENTS_ALLOWED { get; set; }

        [XmlElement("MFG_KM_MI")]
        public int MFG_KM_MI { get; set; }

        [XmlElement("MFG_MONTHS")]
        public int MFG_MONTHS { get; set; }

        [XmlElement("MINIMUM_RETAIL_PRICE")]
        public double MINIMUM_RETAIL_PRICE { get; set; }

        [XmlElement("MAXIMUM_RETAIL_PRICE")]
        public double MAXIMUM_RETAIL_PRICE { get; set; }
    }

    public class PAYMENT_TYPES
    {
        [XmlElement("COLLECTION_METHOD_CODE")]
        public int COLLECTION_METHOD_CODE { get; set; }

        [XmlElement("COLLECTION_METHOD")]
        public string COLLECTION_METHOD { get; set; }

        [XmlElement("PAYMENT_INSTRUMENT_CODE")]
        public string PAYMENT_INSTRUMENT_CODE { get; set; }

        [XmlElement("PAYMENT_INSTRUMENT")]
        public string PAYMENT_INSTRUMENT { get; set; }
    }
}