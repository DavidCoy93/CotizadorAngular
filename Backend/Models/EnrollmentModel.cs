using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MSSPAPI.Models
{
    
    [XmlRoot("VSCEnrollmentDs", Namespace = "")]
    public class VSCEnrollmentDs
    {
        [XmlElement("VSCEnrollment")]
        public VSCEnrollment VSCEnrollment { get; set; }
    }

    [XmlRoot("VSCEnrollment")]
    public class VSCEnrollment
    {
        [XmlElement("Certificate_Number")]
        public string Certificate_Number { get; set; }

        [XmlElement("Customer")]
        public EnrollmentCustomer Customer { get; set; }

        [XmlElement("Address")]
        public string Address { get; set; }

        [XmlElement("City")]
        public string City { get; set; }

        [XmlElement("Region")]
        public string Region { get; set; }

        [XmlElement("Postal_Code")]
        public string Postal_Code { get; set; }

        [XmlElement("Country_Code")]
        public string Country_Code { get; set; }

        [XmlElement("Home_Phone")]
        public string Home_Phone { get; set; }

        [XmlElement("Vehicle_Year")]
        public int Vehicle_Year { get; set; }

        [XmlElement("Vehicle_Make")]
        public string Vehicle_Make { get; set; }

        [XmlElement("Vehicle_Model")]
        public string Vehicle_Model { get; set; }

        [XmlElement("Engine_Version")]
        public string Engine_Version { get; set; }

        [XmlElement("Vehicle_Mileage")]
        public int Vehicle_Mileage { get; set; }

        [XmlElement("VIN")]
        public string VIN { get; set; }

        [XmlElement("Vehicle_Purchase_Price")]
        public decimal Vehicle_Purchase_Price { get; set; }

        [XmlElement(ElementName = "Vehicle_Purchase_Date", DataType = "date")]
        public DateTime Vehicle_Purchase_Date { get; set; }

        [XmlElement(ElementName = "Vehicle_In_Service_Date", DataType = "date")]
        public DateTime Vehicle_In_Service_Date { get; set; }

        [XmlElement(ElementName = "Vehicle_Delivery_Date", DataType = "date")]
        public DateTime Vehicle_Delivery_Date { get; set; }

        [XmlElement("Plan_Code")]
        public string Plan_Code { get; set; }

        [XmlElement("Plan_Amount")]
        public double Plan_Amount { get; set; }

        [XmlElement("Quote_Item_Number")]
        public int Quote_Item_Number { get; set; }

        [XmlElement("Term_Months")]
        public int Term_Months { get; set; }

        [XmlElement("Term_Miles")]
        public int Term_Miles { get; set; }

        [XmlElement("Deductible")]
        public decimal Deductible { get; set; }

        [XmlElement("Optional_Coverage")]
        public EnrollmentOptionalCoverage Optional_Coverage { get; set; }

        [XmlElement("Dealer_Code")]
        public string Dealer_Code { get; set; }

        [XmlElement("Agent_Number")]
        public string Agent_Number { get; set; }

        [XmlElement(ElementName = "Warranty_Sale_Date", DataType = "date")]
        public DateTime Warranty_Sale_Date { get; set; }

        [XmlElement("Quote_Number")]
        public string Quote_Number { get; set; }

        [XmlElement("Vehicle_License_Tag")]
        public string Vehicle_License_Tag { get; set; }

        [XmlElement("Document_Type")]
        public string Document_Type { get; set; }

        [XmlElement("Identity_document_No")]
        public string Identity_document_No { get; set; }

        [XmlElement("RG_No")]
        public string RG_No { get; set; }

        [XmlElement("ID_Type")]
        public string ID_Type { get; set; }

        [XmlElement("Issuing_agency")]
        public string Issuing_agency { get; set; }

        [XmlElement(ElementName = "Document_Issue_Date", DataType = "date")]
        public DateTime Document_Issue_Date { get; set; }

        [XmlElement(ElementName = "Birth_Date", DataType = "date")]
        public DateTime? Birth_Date { get; set; }

        [XmlElement("Work_Phone")]
        public string Work_Phone { get; set; }

        [XmlElement("Collection_Method_Code")]
        public string Collection_Method_Code { get; set; }

        [XmlElement("Payment_Instrument_Code")]
        public string Payment_Instrument_Code { get; set; }

        [XmlElement("Installments_Number")]
        public int Installments_Number { get; set; }

        [XmlElement("Credit_Card_Info")]
        public EnrollmentCreditCardInfo Credit_Card_Info { get; set; }

        [XmlElement("Bank_Account_Info")]
        public EnrollmentBankAccountInfo Bank_Account_Info { get; set; }

        [XmlElement("Payment_Authoriztion_Num")]
        public string Payment_Authoriztion_Num { get; set; }

        [XmlElement("External_Car_Code")]
        public string External_Car_Code { get; set; }

        [XmlElement("Is_CreditCard_AuthReq")]
        public string Is_CreditCard_AuthReq { get; set; }

        [XmlElement("Sales_Tax")]
        public double? Sales_Tax { get; set; }

        [XmlElement("Validate_Only")]
        public string Validate_Only { get; set; }
    }

    [XmlRoot("Customer")]
    public class EnrollmentCustomer
    {
        [XmlElement("Customer_Name")]
        public string Customer_Name { get; set; }

        [XmlElement("Customer_Occupation")]
        public string Customer_Occupation { get; set; }

        [XmlElement("PEP")]
        public string PEP { get; set; }

        [XmlElement("Income_Range_Code")]
        public string Income_Range_Code { get; set; }
    }

    [XmlRoot("Optional_Coverage")]
    public class EnrollmentOptionalCoverage
    {
        [XmlElement("Optional_Coverage_Code")]
        public string Optional_Coverage_Code { get; set; }

        [XmlElement("Optional_Coverage_Price")]
        public decimal Optional_Coverage_Price { get; set; }

        [XmlElement("OptionalCoverageQuote_Item_Number")]
        public int OptionalCoverageQuote_Item_Number { get; set; }
    }

    [XmlRoot("Credit_Card_Info")]
    public class EnrollmentCreditCardInfo
    {
        [XmlElement("Credit_Card_Type_Code")]
        public string Credit_Card_Type_Code { get; set; }

        [XmlElement("Name_On_Credit_Card")]
        public string Name_On_Credit_Card { get; set; }

        [XmlElement("Credit_Card_Number")]
        public string Credit_Card_Number { get; set; }

        [XmlElement("Expiration_Date")]
        public string Expiration_Date { get; set; }

        [XmlElement("Card_Security_Code")]
        public string Card_Security_Code { get; set; }
    }

    [XmlRoot("Bank_Account_Info")]
    public class EnrollmentBankAccountInfo
    {
        [XmlElement("Bank_ID")]
        public string Bank_ID { get; set; }

        [XmlElement("Account_Number")]
        public string Account_Number { get; set; }

        [XmlElement("Name_On_Account")]
        public string Name_On_Account { get; set; }
    }


}