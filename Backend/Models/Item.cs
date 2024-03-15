using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Item
    {
        public int IdCliente { get; set; }
        public string CertificateNumber { get; set; }
        public string DeviceTypeCode { get; set; }
        public string IndixID { get; set; }
        public string ItemDescription { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PurchasedDate { get; set; }
        public string RegisteredItemName { get; set; }
        public string RegistrationDate { get; set; }
        public decimal RetailPrice { get; set; }
        public string SerialNumber { get; set; }
        public string R3Fechaservicio { get; set; }
        public string Poliza { get; set; }
        public string DealerCode { get; set; }
    }
}