using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DeviceInfo
    {
        public int IdCliente { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string  Capacity { get; set; }
        public string Color { get; set; }
        public string Carrier { get; set; }
        public string Description { get; set; }
        public string ModelFamily { get; set; }
        public string ManufacturerIdentifier { get; set; }
        public string SerialNumber { get; set; }
        public string RegisteredItemName { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string RiskTypeCode { get; set; }
        public string CaseNumber { get; set; }
        public string InteractionNumber { get; set; }
        public string DealerCode { get; set; }




    }
}