namespace MSSPAPI.Models
{
    public class DealerCodes
    {
        public int IdDealerCode { get; set; }
        public int IdCliente { get; set; }
        public string DealerCode { get; set; }
        public string TerminosDeUso { get; set; }
        public string TerminosDeGarantia { get; set; }
        public string FrequentlyAskedQuestions { get; set; }
        public string DownloadCertificate { get; set; }
        public string DownloadDocument { get; set; }
        public string ContactUs { get; set; }
        public string TextSurvey { get; set; }
        public string ExternalURL { get; set; }
        public string Idioma { get; set; }
        public string Device { get; set; }
        public string Okta { get; set; }
        public string ServiceOption { get; set; }
        public string PoliticaPrivacidad { get; set; }
        public string TerminosCondiciones { get; set; }
        public string DatosRequeridos { get; set; }
        public bool IsSelected { get; set; }
        public string ProductCodeIcon { get; set; }
        public string VariablesBusquedaNS { get; set; }
        public bool MultipleBusquedaNS { get; set; }
        public string DealerGroup { get; set; }
        public string CompanyCode { get; set; }
    }
}