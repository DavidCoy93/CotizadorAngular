using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class DataEvaluation
    {
        public int Id { get; set; }
        public string DataEvaluations { get; set; }
        public string CrudHistryLust { get; set; }
        public bool Active { get; set; }
    }
}