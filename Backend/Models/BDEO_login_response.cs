namespace MSSPAPI.Models
{

    public class LoginResponse
    {
        public bool success { get; set; }
        public string access_token { get; set; }
        public Entity entity { get; set; }
        public bool expired { get; set; }
    }

    public class Entity
    {
        public string id { get; set; }
        public string company_id { get; set; }
        public string company_name { get; set; }
        public string company_code { get; set; }
        public string company_type { get; set; }
        public string service_code { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string document_number { get; set; }
        public string email { get; set; }
        public string[] type { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public int status { get; set; }
        public string avatar { get; set; }
        public string signature { get; set; }
        public string access_token { get; set; }
        public int createdAt { get; set; }
        public string createdBy { get; set; }
        public int updatedAt { get; set; }
        public string updatedBy { get; set; }
    }

}