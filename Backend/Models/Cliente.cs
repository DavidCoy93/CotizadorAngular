using System.Collections.Generic;

namespace MSSPAPI.Models
{
    /// <summary>
    ///
    /// </summary>
    public class Cliente
    {
        /// <summary>
        ///
        /// </summary>
        public Cliente()
        {
            DealerCode = new List<DealerCodes>();
        }

        public int IdCliente { get; set; }
        public string  IdCountry { get; set; }
        public string NombreCliente { get; set; }
        public string Configuraciones { get; set; }
        public string Apikey { get; set; }
        public string Authorization { get; set; }
        public string RiskTypeCode { get; set; }
        public bool Active { get; set; }
        public bool Login { get; set; }
        public bool Flujo { get; set; }
        public string Payments { get; set; }
        public bool Multiple { get; set; }
        public string URL { get; set; }

        public string URLHeader { get; set; }
        public string URLFooter { get; set; }
        public List<DealerCodes> DealerCode { get; set; }
    }
}