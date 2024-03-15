using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Questions
    {
        public string Code { get; set; }
        public string Text { get; set; }
        public string AnswerType { get; set; }
        public string Mandatory { get; set; }
        public string Scale { get; set; }
        public string Precision { get; set; }
        public string Length { get; set; }
        public string SequenceNumber { get; set; }
        public string Preconditions { get; set; }
        public string Applicable { get; set; }
        public string ReEvaulateOnChange { get; set; }
        public string ChannelDisabled { get; set; }
        public string Validations { get; set; }
    }
}