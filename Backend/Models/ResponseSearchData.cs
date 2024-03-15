using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class ResponseSearchData
    {
        public string CustomerDescription { get; set; }
        public int? AuthorizationCount { get; set; }
        public string AuthorizationNumber { get; set; }
        public decimal? AuthorizedAmount { get; set; }
        public string CallerEmail { get; set; }
        public string CallerName { get; set; }
        public string CallerPhoneNumber { get; set; }
        public DateTime? CaseCreatedDate { get; set; }
        public string CaseDenialReason { get; set; }
        public string CaseNumber { get; set; }
        public string CaseStatus { get; set; }
        public string CertificateNumber { get; set; }
        public string ClaimCreatedBy { get; set; }
        public dynamic ClaimEquipment { get; set; }
        public dynamic ClaimEquipmentList { get; set; }
        public Guid ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public string ClaimStatus { get; set; }
        public string ClosedReasonCode { get; set; }
        public string CompanyCode { get; set; }
        public string CoverageType { get; set; }
        public string CoverageTypeCode { get; set; }
        public DateTime? DateOfLoss { get; set; }
        public string DealerCode { get; set; }
        public string DealerGroup { get; set; }
        public decimal? Deductible { get; set; }
        public string DeviceNickname { get; set; }
        public string DeviceSerialNumber { get; set; }
        public DateTime? ExpectedRepairDate { get; set; }
        public dynamic ExtensionData { get; set; }
        public string ExtStatusCode { get; set; }
        public string ExtStatusDescription { get; set; }
        public string FulfillmentOptionCode { get; set; }
        public dynamic IssueStatus { get; set; }
        public string MethodOfRepair { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal? PaymentAmountWithoutConseqDamage { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? RepairDate { get; set; }
        public DateTime? ReportedDate { get; set; }
        public string RiskType { get; set; }
        public string SerialNumber { get; set; }
        public dynamic ShippingAddress { get; set; }
        public string StatusCode { get; set; }
        public DateTime? VisitDate { get; set; }

    }
}