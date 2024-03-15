using System;
using System.ComponentModel.DataAnnotations;

namespace MSSPAPI.Models
{
    public class MethodRepairAdd
    {
        [Key]
      
        public string DealerCode { get; set; }
        public string RiskType { get; set; }
        public string Marca { get; set; }
        public string StateProvidence { get; set; }
        public string StateCode { get; set; }
        public string City { get; set; }
        public string ServiceCenterCode { get; set; }
        public string ServiceCenterName { get; set; }
        public string Output { get; set; }
        public DateTime CreationDate { get; set; }
        public string UserCreator { get; set; }
        public bool Active { get; set; }
        public DateTime HorarioS { get; set; }
        public DateTime HorarioF { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }

    }
}