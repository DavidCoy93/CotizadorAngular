using System.Collections.Generic;

namespace MSSPAPI.Models
{

    public class BDEOInterventionListResponse
    {
        private List<Result> Result = new List<Result>();
        public int offset { get; set; }
        public int limit { get; set; }
        public string total { get; set; }
        public int size { get; set; }
        public List<Result> result {
            get { return Result; }
            set { Result = value; }
        }
    }

    public class Result
    {
        public string agent_id { get; set; }
        public bool failedPermissions { get; set; }
        public float lng { get; set; }
        public string remote_web_device { get; set; }
        public string created_by { get; set; }
        public int status { get; set; }
        public int billed_at { get; set; }
        public string updatedBy { get; set; }
        public string createdBy { get; set; }
        public string handyman_id { get; set; }
        public object insuredName { get; set; }
        public string master_company_code { get; set; }
        public string slave_user_desc { get; set; }
        public string code { get; set; }
        public string case_reference { get; set; }
        public int startedAt { get; set; }
        public object[] notification_ids { get; set; }
        public string id { get; set; }
        public string case_id { get; set; }
        public string insurance_company_name { get; set; }
        public string handymanPhone { get; set; }
        public string master_user_desc { get; set; }
        public string master_company_id { get; set; }
        public int scheduled_date { get; set; }
        public int callLength { get; set; }
        public int createdAt { get; set; }
        public object address { get; set; }
        public int lastCallTime { get; set; }
        public bool smsReceived { get; set; }
        public string prefixInput { get; set; }
        public long termsAndConditionsAcceptedAt { get; set; }
        public bool anon { get; set; }
        public int updatedAt { get; set; }
        public bool handyman_status { get; set; }
        public float lat { get; set; }
        public object type { get; set; }
        public string additionalEmail { get; set; }
        public string engineStatus { get; set; }
        public string maintenanceCard { get; set; }
        public Video[] videos { get; set; }
        public string engineInfo { get; set; }
        public string comments { get; set; }
        public Image[] images { get; set; }
        public string custom_field_1 { get; set; }
        public string vinOdometerInfo { get; set; }
        public int finishedAt { get; set; }
        public string finishedBy { get; set; }
        public string report { get; set; }
        public int attempts { get; set; }
        public int canceledAt { get; set; }
        public string canceledBy { get; set; }
        public object[] documents { get; set; }
        public string email { get; set; }
        public string remote_url { get; set; }
        public string url { get; set; }
        public string notes { get; set; }
        public string description { get; set; }
    }

    public class Video
    {
        public int createdAt { get; set; }
        public bool offline { get; set; }
        public int size { get; set; }
        public Coordinates coordinates { get; set; }
        public string video { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string key { get; set; }
    }

    public class Coordinates
    {
        public float lng { get; set; }
        public float accuracy { get; set; }
        public float lat { get; set; }
    }

    public class Image
    {
        public string image { get; set; }
        public int createdAt { get; set; }
        public bool offline { get; set; }
        public string thumbnail { get; set; }
        public int size { get; set; }
        public Coordinates coordinates { get; set; }
        public object caption { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uploadedBy { get; set; }
    }
}