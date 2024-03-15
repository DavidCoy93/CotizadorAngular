namespace MSSPAPI.Models
{
    public class BDEO_Intervention_Request
    {
        public string @case { get; set; }
        public string remote_phone { get; set; }
        public string type { get; set; }
        public string sms { get; set; }
        public string sms_text { get; set; }
        public string scheduled_date { get; set; }
        public string agent_id { get; set; }
        public string external_agent_id { get; set; }
        public string external_agent_name { get; set; }
        public string external_agent_surname { get; set; }
        public string insured_name { get; set; }
        public string insured_surname { get; set; }
        public string policy_number { get; set; }
        public string master_company_id { get; set; }
        public string lang { get; set; }
        public string email { get; set; }
        public string notes { get; set; }
    }
}