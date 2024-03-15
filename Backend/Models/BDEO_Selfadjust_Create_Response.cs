namespace MSSPAPI.Models
{

    public class BDEOSelfAdjustCreateResponse
    {
        public string id { get; set; }
        public string assigned_agent_name { get; set; }
        public string assigned_agent_id { get; set; }
        public int appLink { get; set; }
        public int termsAndConditionsAcceptedAt { get; set; }
        public int startedAt { get; set; }
        public int finishedAtTimestamp { get; set; }
        public int reopeningStartedAt { get; set; }
        public int reopeningFinishedAt { get; set; }
        public int billed_at { get; set; }
        public bool cameraPermissionsAccepted { get; set; }
        public string case_id { get; set; }
        public string caseRef { get; set; }
        public string comments { get; set; }
        public string company_name { get; set; }
        public string created_by { get; set; }
        public Costestimationinfo costEstimationInfo { get; set; }
        public Costestimationprovider costEstimationProvider { get; set; }
        public int damagedParts { get; set; }
        public float damageScore { get; set; }
        public Damagesummary[] damageSummary { get; set; }
        public Thirdpartyinfo thirdPartyInfo { get; set; }
        public string deadlineSelfadjust { get; set; }
        public Deductible deductible { get; set; }
        public Document[] documents { get; set; }
        public string email { get; set; }
        public string finishedAt { get; set; }
        public Fraudsummary fraudSummary { get; set; }
        public bool GPSPermissionsAccepted { get; set; }
        public int grade { get; set; }
        public string feedbackComment { get; set; }
        public string[] images { get; set; }
        public string insured_name { get; set; }
        public string insured_surname { get; set; }
        public bool isOpen { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public Log[] logs { get; set; }
        public string master_company_id { get; set; }
        public Multimedia[] multimedia { get; set; }
        public string[] files { get; set; }
        public string notificationChannel { get; set; }
        public string[] notification_ids { get; set; }
        public string numRef { get; set; }
        public int openedAt { get; set; }
        public int over_excess { get; set; }
        public Partproviderreports partProviderReports { get; set; }
        public string phone { get; set; }
        public string prefixInput { get; set; }
        public Profilersummary profilerSummary { get; set; }
        public Qualitysummary qualitySummary { get; set; }
        public string report { get; set; }
        public Resultsai resultsAI { get; set; }
        public int reviewedAt { get; set; }
        public string securityKey { get; set; }
        public string selfadjustLink { get; set; }
        public string selfadjustProgress { get; set; }
        public string service_code { get; set; }
        public int status { get; set; }
        public string statusIndex { get; set; }
        public string template { get; set; }
        public string vehicleType { get; set; }
        public bool waitingForIA { get; set; }
        public string url { get; set; }
        public string policy_number { get; set; }
        public int reopenedAt { get; set; }
        public string reviewedBy { get; set; }
        public string sender { get; set; }
        public string attempts { get; set; }
        public string remote_web_device { get; set; }
    }

    public class Costestimationinfo
    {
        public int paintHourPrice { get; set; }
        public int labourHourPrice { get; set; }
        public int taxPercentage { get; set; }
        public string taxName { get; set; }
        public string currency { get; set; }
    }

    public class Costestimationprovider
    {
        public string name { get; set; }
        public Params _params { get; set; }
    }

    public class Params
    {
        public string version { get; set; }
    }

    public class Thirdpartyinfo
    {
        public Estimation estimation { get; set; }
        public Vehicle vehicle { get; set; }
    }

    public class Estimation
    {
        public int createdAt { get; set; }
        public Data data { get; set; }
        public string provider { get; set; }
    }

    public class Data
    {
        public string report { get; set; }
        public Total total { get; set; }
        public string url { get; set; }
    }

    public class Total
    {
        public float cost { get; set; }
        public Labour labour { get; set; }
        public Parts parts { get; set; }
        public Paint paint { get; set; }
    }

    public class Labour
    {
        public float cost { get; set; }
        public float time { get; set; }
    }

    public class Parts
    {
        public float cost { get; set; }
        public float time { get; set; }
    }

    public class Paint
    {
        public float cost { get; set; }
        public float time { get; set; }
    }

    public class Vehicle
    {
        public int createdAt { get; set; }
        public Data1 data { get; set; }
        public string provider { get; set; }
    }

    public class Data1
    {
        public string id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string version { get; set; }
        public string vin { get; set; }
    }

    public class Deductible
    {
        public float minRecomendation { get; set; }
        public Zone[] zones { get; set; }
    }

    public class Zone
    {
        public string title { get; set; }
        public float value { get; set; }
    }

    public class Fraudsummary
    {
        public int fraud { get; set; }
        public Platedetection plateDetection { get; set; }
        public Uniquevehicle uniqueVehicle { get; set; }
        public Screendetection screenDetection { get; set; }
        public Gpslocation gpsLocation { get; set; }
        public Colordetection colorDetection { get; set; }
        public Odometer odometer { get; set; }
        public Vindetection vinDetection { get; set; }
    }

    public class Platedetection
    {
        public Casecheck caseCheck { get; set; }
        public Platecomparison plateComparison { get; set; }
        public Missingplate missingPlate { get; set; }
        public Detail[] detail { get; set; }
    }

    public class Casecheck
    {
        public int fraud { get; set; }
        public string real_text_plate { get; set; }
    }

    public class Platecomparison
    {
        public int fraud { get; set; }
    }

    public class Missingplate
    {
        public int fraud { get; set; }
    }

    public class Detail
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string image_original { get; set; }
        public string type { get; set; }
        public bool detected_plate { get; set; }
        public float[][] plate_bounding_box { get; set; }
        public string detected_plate_text { get; set; }
        public bool case_fraud { get; set; }
        public bool missing_plate_fraud { get; set; }
    }

    public class Uniquevehicle
    {
        public int fraud { get; set; }
        public float unique_vehicle_threshold { get; set; }
        public float result { get; set; }
    }

    public class Screendetection
    {
        public int fraud { get; set; }
        public Detail1[] detail { get; set; }
    }

    public class Detail1
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string type { get; set; }
        public bool fraud { get; set; }
        public bool result { get; set; }
    }

    public class Gpslocation
    {
        public int fraud { get; set; }
        public int max_distance { get; set; }
        public int min_accuracy { get; set; }
        public string reason { get; set; }
        public int result_distance { get; set; }
        public Detail2[] detail { get; set; }
    }

    public class Detail2
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string type { get; set; }
        public float[][] coordinates { get; set; }
        public int coordinatesAccuracy { get; set; }
    }

    public class Colordetection
    {
        public string color { get; set; }
    }

    public class Odometer
    {
        public Missingodometer missingOdometer { get; set; }
        public Detail3 detail { get; set; }
    }

    public class Missingodometer
    {
        public int fraud { get; set; }
    }

    public class Detail3
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string image_origin { get; set; }
        public string type { get; set; }
        public bool detected_odometer { get; set; }
        public float[][] odometer_bounding_box { get; set; }
        public string detected_odometer_text { get; set; }
        public string odometer_unit { get; set; }
    }

    public class Vindetection
    {
        public Wrongvin wrongVin { get; set; }
        public Missingvin missingVin { get; set; }
        public Casecheck1 caseCheck { get; set; }
        public Undecodablevin undecodableVin { get; set; }
        public Detail4 detail { get; set; }
    }

    public class Wrongvin
    {
        public int fraud { get; set; }
    }

    public class Missingvin
    {
        public int fraud { get; set; }
    }

    public class Casecheck1
    {
        public int fraud { get; set; }
        public string real_text_vin { get; set; }
    }

    public class Undecodablevin
    {
        public int fraud { get; set; }
    }

    public class Detail4
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string image_origin { get; set; }
        public string type { get; set; }
        public string decoded_body_type { get; set; }
        public string decoded_brand { get; set; }
        public string decoded_country { get; set; }
        public string decoded_model { get; set; }
        public int decoded_year { get; set; }
        public string detected_vin_text { get; set; }
        public float[][] vin_bounding_box { get; set; }
    }

    public class Partproviderreports
    {
        public string providerId { get; set; }
    }

    public class Profilersummary
    {
        public bool issue { get; set; }
        public Damages damages { get; set; }
        public Fraud fraud { get; set; }
        public Quality quality { get; set; }
        public Cost_Estimation cost_estimation { get; set; }
        public Vehicle_Information vehicle_information { get; set; }
    }

    public class Damages
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class Fraud
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class Quality
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class Cost_Estimation
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class Vehicle_Information
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class Qualitysummary
    {
        public int issue { get; set; }
        public Qualitydetection qualityDetection { get; set; }
        public Vehicledetection vehicleDetection { get; set; }
        public Orientationdetection orientationDetection { get; set; }
    }

    public class Qualitydetection
    {
        public int issue { get; set; }
        public int quality_threshold { get; set; }
        public Detail5[] detail { get; set; }
    }

    public class Detail5
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string type { get; set; }
        public int result { get; set; }
        public bool issue { get; set; }
    }

    public class Vehicledetection
    {
        public int issue { get; set; }
        public Detail6[] detail { get; set; }
    }

    public class Detail6
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string type { get; set; }
        public bool result { get; set; }
        public float[][] vehicle_bounding_box { get; set; }
    }

    public class Orientationdetection
    {
        public int issue { get; set; }
        public Detail7[] detail { get; set; }
    }

    public class Detail7
    {
        public string image { get; set; }
        public string image_uuid { get; set; }
        public string type { get; set; }
        public bool result { get; set; }
        public bool issue { get; set; }
    }

    public class Resultsai
    {
        public Vehicle_Part_Index vehicle_part_index { get; set; }
    }

    public class Vehicle_Part_Index
    {
        public string damageDetectionUrl { get; set; }
    }

    public class Damagesummary
    {
        public Maindetection mainDetection { get; set; }
        public Otherdetection[] otherDetection { get; set; }
    }

    public class Maindetection
    {
        public Damage[] damages { get; set; }
        public Part part { get; set; }
        public Estimation1 estimation { get; set; }
        public Image_Info image_info { get; set; }
    }

    public class Part
    {
        public string id { get; set; }
        public string description { get; set; }
    }

    public class Estimation1
    {
        public string operation { get; set; }
        public string operation_user { get; set; }
        public float paint_time { get; set; }
        public float paint_time_user { get; set; }
        public float labour_time { get; set; }
        public float labour_time_user { get; set; }
        public float paint_materials { get; set; }
        public float paint_materials_user { get; set; }
        public float strip_assembly { get; set; }
        public float strip_assembly_user { get; set; }
        public float part_price { get; set; }
        public float part_price_user { get; set; }
        public string part_provider { get; set; }
        public string part_provider_user { get; set; }
        public string part_reference { get; set; }
        public string part_reference_user { get; set; }
    }

    public class Image_Info
    {
        public string type { get; set; }
        public string view { get; set; }
        public string image_uuid { get; set; }
        public string zoom { get; set; }
    }

    public class Damage
    {
        public string typology { get; set; }
        public string severity { get; set; }
        public float area { get; set; }
        public float confidence { get; set; }
        public string origin { get; set; }
        public bool is_main { get; set; }
        public bool is_deleted { get; set; }
        public bool origin_user { get; set; }
        public string uuid { get; set; }
    }

    public class Otherdetection
    {
    }

    //public class Document
    //{
    //    public string name { get; set; }
    //    public int index { get; set; }
    //    public long uploadedAt { get; set; }
    //    public string uuid { get; set; }
    //    public string url { get; set; }
    //}

    public class Log
    {
        public string action { get; set; }
        public int when { get; set; }
        public string rol { get; set; }
        public string who { get; set; }
    }

    public class Multimedia
    {
        public string type { get; set; }
        public string name { get; set; }
        public string image_uuid { get; set; }
        public string signedUrl { get; set; }
        public int takePhotoAt { get; set; }
        public float[][] coordinates { get; set; }
        public int coordinatesAccuracy { get; set; }
        public string base64Content { get; set; }
    }

}