using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class Vehicles
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdProgram { get; set; }
        public string Cliente { get; set; }
        public int IdModel { get; set; }
        public string Model { get; set; }
        public int IdVehicleVersion { get; set; }
        public string VehicleVersion { get; set; }
        public string Year { get; set; }
        public int IdYear { get; set; }
        public string EngineNumber { get; set; }
        public long? Kilometers { get; set; }
        public string VIN { get; set; }
        public string VehicleUse { get; set; }
        public int IdVehicleUse { get; set; }
        public bool Active { get; set; }
    }
}