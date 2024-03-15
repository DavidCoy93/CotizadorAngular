using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class BDEOGenerateAgentUrlRequest
    {
        public string user_id { get; set; }
        public string intervention_id { get; set; }
        public int expiration_time { get; set; }
        public string external_agent_name { get; set; }
        public string external_agent_surname { get; set; }
    }

    public class BDEOGenerateAgentUrlResponse
    {
        public string agent_url { get; set; }
    }
}