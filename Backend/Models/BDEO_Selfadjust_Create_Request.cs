namespace MSSPAPI.Models
{

    public class BDEOSelfAdjustCreateRequest
    {
        public string @case { get; set; }
        public string remote_phone { get; set; }
        public string deadlineSelfadjust { get; set; }
        public string vehicleType { get; set; }
        public string template { get; set; }
        public string notificationChannel { get; set; }
        public string email { get; set; }
        public string insured_name { get; set; }
        public string insured_surname { get; set; }
        public string policy_number { get; set; }
        public string lang { get; set; }
        public string Custom_Field { get; set; }
    }

}