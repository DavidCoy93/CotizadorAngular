using System;
using System.ComponentModel.DataAnnotations;


namespace MSSPAPI.Models
{
    public class Logs
    {
        [Key]
        public int IdLog { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string IP { get; set; }
        public string Browser { get; set; }
        public string Exception { get; set; }
    }
}