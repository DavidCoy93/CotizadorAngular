using System.Collections.Generic;

namespace MSSPAPI.Models
{

    public class BDEOInterventionResponse
    {
        public string id { get; set; }
        public string code { get; set; }
        public string case_id { get; set; }
        public string case_reference { get; set; }
        public string master_company_id { get; set; }
        public string agent_id { get; set; }
        public string master_user_desc { get; set; }
        public string remote_web_device { get; set; }
        public string handyman_id { get; set; }
        public bool handyman_status { get; set; }
        public string handymanOhone { get; set; }
        public bool anon { get; set; }
        public bool smsReceived { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public string scheduled_date { get; set; }
        public string startedAt { get; set; }
        public string lastCallTime { get; set; }
        public int callLength { get; set; }
        public string comments { get; set; }
        public string notes { get; set; }
        public int rate { get; set; }
        public string url { get; set; }
        public Image[] images { get; set; }
        public Video[] videos { get; set; }
        public Document[] documents { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public string updatedAt { get; set; }
        public string updatedBy { get; set; }
    }
}