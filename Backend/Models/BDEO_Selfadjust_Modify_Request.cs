namespace MSSPAPI.Models
{

    public class BDEOSelfAdjustModifyRequest
    {
        public string remote_phone { get; set; }
        public string email { get; set; }
        public string notificationChannel { get; set; }
        public string deadlineSelfadjust { get; set; }
        public string action { get; set; }
        public int status { get; set; }
        public int over_excess { get; set; }
        public string comments { get; set; }
        public reopenInfo reopenInfo { get; set; }
    }
    public class reopenInfo
    {
        public string[] multimedia { set; get; }
        public string instructions { get; set; }
    }
}