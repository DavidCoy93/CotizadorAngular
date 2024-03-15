using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Country
    {
        public Country()
        {
            City = new List<City>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<City> City { get; set; }
    }
}