using System;

namespace MSSPAPI.Models
{
    public class OpenPay_Response
    {
        private OpenPay_Error _Error = new OpenPay_Error();

        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public int IvrKey { get; set; }
        public OpenPay_Error Error {
            get { return _Error; }
            set { _Error = value; }
        }
    }


    public class OpenPay_Error
    {
        public int Number { get; set; }
        public string Description { get; set; }
    }

    public class OpenPay_Cargo
    {
        public int IdClient { get; set; }
        public int IdBrand { get; set; }
        public int IdMethod { get; set; }
        public int IdBanner { get; set; }
        public bool AIZBrandVisible { get; set; }
        public bool ClientBrandVisible { get; set; }
        public string Amount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }
        public long CustomerPhone { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        //public DateTime? ExpirationDate { get; set; }
        public string TrackingKey { get; set; }
        public int MonthlyPayments { get; set; }
        public int Status { get; set; }
        public object IdOpenPay { get; set; }
        public object OpenPayResponse { get; set; }
        public object OpenPaySource_id { get; set; }
        public object OpenPayDevice_session_id { get; set; }
    }

}