namespace MSSPAPI.Models
{

    public class BDEOInterventionUpdateRequest
    {
        public bool resendSMS { get; set; }
        public int status { get; set; }
        public int scheduled_date { get; set; }
        public string agent_id { get; set; }
        public string external_agent_id { get; set; }
    }

}