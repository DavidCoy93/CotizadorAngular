using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string WebsiteDescription { get; set; }
        public int IdProductType { get; set; }
        public string BrandCode { get; set; }
        public int PeriodLowerLimit { get; set; }
        public int PeriodUpperLimit { get; set; }
        public int KilometerLowerLimit { get; set; }
        public int KilometerUpperLimit { get; set; }
        public int IdProgram {  get; set; }
        public bool Services {  get; set; }
        public string TypeWarranty { get; set; }
        public string CrudHistoryList { get; set; }
        public string Template {  get; set; }
        public bool Active { get; set; }
        public List<Rate> Rates { get; set; }

    }
}