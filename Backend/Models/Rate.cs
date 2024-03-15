using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
	public class Rate
	{
		public int Id { get; set; }
		public int Term { get; set; }
		public int IdProduct { get; set; }
		public int IdClassType { get; set; }
		public string ClassCode { get; set; }
		public int MaxKm { get; set; }
		public decimal Deductible { get; set; }
		public decimal PriceCert { get; set; }
		public decimal PriceEW { get; set; }
		public decimal PriceWP { get; set; }
		public decimal MKT { get; set; }
		public int PlanCodeEW { get; set; }
		public decimal PriceRA { get; set; }
		public int PlanCodeRA { get; set; }
		public decimal Prime { get; set; }
		public decimal AgencyCommission { get; set; }
		public decimal VendorCommission { get; set; }
		public decimal DealerCommission { get; set; }
		public decimal AdminFee { get; set; }
		public decimal InsurancePolicy { get; set; }
		public decimal CommAnzen { get; set; }
		public decimal Rewards { get; set; }
		public decimal CommNR { get; set; }
		public decimal CommPlant { get; set; }
		public decimal RiskPremium { get; set; }
		public decimal Profit {  get; set; }
		public int IdProgram { get; set; }
		public decimal Taxes_Percent { get; set; }
		public string CrudHistoryList { get; set; }
		public bool Active { get; set; }
    }
}