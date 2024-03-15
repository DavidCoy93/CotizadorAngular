namespace MSSPAPI.Controllers
{
    internal class CustomServiceFault
    {
        public string ErrorMessage { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string Target { get; set; }
        public string InnerExceptionMessage { get; set; }
    }
}