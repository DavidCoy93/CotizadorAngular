namespace MSSPAPI.Models
{

    public class BDEOInterventionCreateResponse
    {
        public string id { get; set; }
        public string code { get; set; }
        public string case_id { get; set; }
        public string case_reference { get; set; }
        public string master_company_id { get; set; }
        public string master_company_code { get; set; }
        public string agent_id { get; set; }
        public string master_user_desc { get; set; }
        public string remote_web_device { get; set; }
        public string handyman_id { get; set; }
        public bool handyman_status { get; set; }
        public string handymanPhone { get; set; }
        public bool anon { get; set; }
        public bool smsReceived { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public int scheduled_date { get; set; }
        public int startedAt { get; set; }
        public int lastCallTime { get; set; }
        public int callLength { get; set; }
        public string comments { get; set; }
        public string notes { get; set; }
        public int rate { get; set; }
        public string url { get; set; }
        public Image[] images { get; set; }
        public Video[] videos { get; set; }
        public Document[] documents { get; set; }
        public int createdAt { get; set; }
        public string createdBy { get; set; }
        public int updatedAt { get; set; }
        public string updatedBy { get; set; }
    }

    //public class Image
    //{
    //    public string id { get; set; }
    //    public Coordinates coordinates { get; set; }
    //    public bool offline { get; set; }
    //    public string image { get; set; }
    //    public string thumbnail { get; set; }
    //    public string type { get; set; }
    //    public int size { get; set; }
    //    public string uploadedBy { get; set; }
    //    public int createdAt { get; set; }
    //}

    //public class Coordinates
    //{
    //    public float lat { get; set; }
    //    public float lng { get; set; }
    //}

    //public class Video
    //{
    //    public string id { get; set; }
    //    public bool offline { get; set; }
    //    public string type { get; set; }
    //    public string video { get; set; }
    //    public string key { get; set; }
    //    public int createdAt { get; set; }
    //}

    public class Document
    {
        public string id { get; set; }
        public string uuid { get; set; }
        public string name { get; set; }
        public int index { get; set; }
        public bool offline { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public int createdAt { get; set; }
        public long uploadedAt { get; set; }
    }

}