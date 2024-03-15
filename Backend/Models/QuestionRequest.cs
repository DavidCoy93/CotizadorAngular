using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class QuestionRequest
    {
        public string CaseNumber { get; set; }
        public string InteractionNumber { get; set; }
        public string QuestionSetCode { get; set; }
        public string Version { get; set; }   
        public object Questions[] { get; set; }



}

}