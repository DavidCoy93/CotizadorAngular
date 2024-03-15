using iTextSharp.text;

namespace MSSPAPI.Models
{

    public class BDEOExternalSelfAdjustCreateRequest
    {
        public string @case { get; set; }
        public string body_class { get; set; }
        public Imagess[] images { get; set; }
        public string Custom_Field { get; set; }

    }

    public class Imagess
    {
        public string name { get; set; }
        public string type { get; set; }
        public string vehiclePart { get; set; }
        public string filename { get; set; }
        public string file_key { get; set; }
    }

}