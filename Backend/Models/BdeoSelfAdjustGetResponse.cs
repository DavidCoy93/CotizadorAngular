using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSPAPI.Models
{
    public class BDEOSelfAdjustGetResponse
    {
        public string statusIndex { get; set; }
        public decimal lng { get; set; }
        public BdeoRemoteWebDevice remote_web_device { get; set; }
        public int status {  get; set; }
        public string finishedAt { get; set; }
        public string sender { get; set; }
        public string feedbackComment { get; set; }
        public BdeoFraudSummary fraudSummary { get; set; }
        public string[] files { get; set; }
        public string policy_number { get; set; }
        public BdeoQualitySummary qualitySummary { get; set; }
        public string case_id { get; set; }
        public string phone { get; set; }
        public bool waitingForAI { get; set; }
        public bool reportedDamage { get; set; }
        public string email_confirm { get; set; }
        public BdeoProfilerSummary profilerSummary { get; set; }
        public string address { get; set; }
        public string lang { get; set; }
        public string prefixInput { get; set; }
        public int finishedAtTimestamp { get; set; }
        public string company_name { get; set; }
        public bool cameraPermissionsAccepted { get; set; }
        public BdeoDamageSummaryItem[] damageSummary { get; set; }
        public string notificationChannel { get; set; }
        public string selfadjustProgress { get; set; }
        public string created_by { get; set; }
        public string report { get; set; }
        public string securityKey { get; set; }
        public string service_code { get; set; }
        public int billed_at { get; set; }
        public int openedAt { get; set; }
        public string email { get; set; }
        public float damageScore { get; set; }
        public bool damage_summary_modified { get; set; }
        public string numRef { get; set; }
        public string caseRef { get; set; }
        public int startedAt { get; set; }
        public string insured_name { get; set; }
        public int isOpen { get; set; }
        public string id { get; set; }
        public int grade { get; set; }
        public int damagedParts { get; set; }
        public string master_company_id { get; set; }
        public bool GPSPermissionsAccepted { get; set; }
        public int termsAndConditionsAcceptedAt { get; set; }
        public BdeoCostEstimationProvider costEstimationProvider { get; set; }
        public string insured_surname { get; set; }
        public string[] images { get; set; }
        public decimal lat { get; set; }
        public BdeoBasket basket { get; set; }
        public BdeoLog[] logs { get; set; }
        public BdeoMultimedia[] multimedia { get; set; }

        public BdeoResultsAI resultsAI { get; set; }
        public string selfadjustLink { get; set; }
    }

    #region RemoteWebDevice
    public class BdeoRemoteWebDevice
    {
        public string platform { get; set; }
        public string userAgent { get; set; }
        public string browser { get; set; }
    }

    #endregion

    #region BdeoFraudSummary
    public class BdeoFraudSummary
    {
        public BdeoFraudSummaryGpsLocation gpsLocation { get; set; }
        public BdeoFraudSummaryVinDetection vinDetection { get; set; }
        public BdeoFraudSummaryUniqueVehicle uniqueVehicle { get; set; }
        public BdeoFraudSummaryOdometer odometer { get; set; }
        public int fraud { get; set; }
        public BdeoFraudSummaryColorDetection colorDetection { get; set; }
        public BdeoFraudSummaryPlateDetection plateDetection { get; set; }
        public BdeoFraudSummaryScreenDetection screenDetection { get; set; }

    }

    #region GpsLocation
    public class BdeoFraudSummaryGpsLocation
    {
        public string reason { get; set; }
        public int min_accuracy { get; set; }
        public BdeoFraudSummaryGpsLocationDetail[] detail { get; set; }
        public float result_distance { get; set; }
        public int max_distance { get; set; }
        public int fraud { get; set; }
    }

    public class BdeoFraudSummaryGpsLocationDetail
    {
        public float[] coordinates { get; set; }
        public string image { get; set; }
        public float coordinatesAccuracy { get; set; }
        public string type { get; set; }
        public string image_uuid { get; set; }
    }
    #endregion

    #region VinDetection
    public class BdeoFraudSummaryVinDetection
    {
        public BdeoFraudSummaryVinDetectionMissingVin missingVin { get; set; }
        public BdeoFraudSummaryVinDetectionUndecodableVin undecodableVin { get; set; }
        public BdeoFraudSummaryVinDetectionDetail detail { get; set; }
        public BdeoFraudSummaryVinDetectionWrongVin wrongVin { get; set; }
        public BdeoFraudSummaryVinDetectionCaseCheck caseCheck { get; set; }
    }

    public class BdeoFraudSummaryVinDetectionMissingVin
    {
        public int fraud { get; set; }
    }

    public class BdeoFraudSummaryVinDetectionUndecodableVin
    {
        public int fraud { get; set; }
    }

    public class BdeoFraudSummaryVinDetectionDetail
    {
        public string image { get; set; }
        public int decoded_year { get; set; }
        public string detected_vin_text { get; set; }
        public string decoded_country { get; set; }
        public string decoded_body_type { get; set; }
        public bool detected_vin { get; set; }
        public string image_origin { get; set; }
        public string decoded_brand { get; set; }
        public string type { get; set; }
        public string image_uuid { get; set; }
        public float[] vin_bounding_box { get; set; }
        public string decoded_model { get; set; }
    }

    public class BdeoFraudSummaryVinDetectionWrongVin
    {
        public int fraud { get; set; }
    }

    public class BdeoFraudSummaryVinDetectionCaseCheck
    {
        public string real_text_vin { get; set; }
        public int fraud { get; set; }
    }
    #endregion

    #region UniqueVehicle
    public class BdeoFraudSummaryUniqueVehicle
    {
        public float result { get; set; }
        public int fraud { get; set; }
    }
    #endregion

    #region Odometer
    public class BdeoFraudSummaryOdometer
    {
        public BdeoFraudSummaryOdometerMissingOdometer missingOdometer { get; set; }
        public BdeoFraudSummaryOdometerDetail detail { get; set; }
    }

    public class BdeoFraudSummaryOdometerMissingOdometer
    {
        public int fraud { get; set; }
    }

    public class BdeoFraudSummaryOdometerDetail
    {
        public string detected_odometer_text { get; set; }
        public string image { get; set; }
        public bool detected_odometer { get; set; }
        public string image_origin { get; set; }
        public string type { get; set; }
        public float[] odometer_bounding_box { get; set; }
        public string odometer_unit { get; set; }
        public string image_uuid { get; set; }
    }
    #endregion

    #region ColorDetection
    public class BdeoFraudSummaryColorDetection
    {
        public string color { get; set; }
    }
    #endregion

    #region PlateDetection
    public class BdeoFraudSummaryPlateDetection
    {
        public BdeoFraudSummaryPlateDetectionMissingPlate missingPlate { get; set; }
        public BdeoFraudSummaryPlateDetectionDetail[] detail { get; set; }
        public BdeoFraudSummaryPlateDetectionPlateComparison plateComparison { get; set; }
        public int fraud { get; set; }
        public BdeoFraudSummaryPlateDetectionCaseCheck caseCheck { get; set; }
    }

    public class BdeoFraudSummaryPlateDetectionMissingPlate
    {
        public float fraud { get; set; }
    }

    public class BdeoFraudSummaryPlateDetectionDetail
    {
        public string image { get; set; }
        public bool case_fraud { get; set; }
        public bool missing_plate_fraud { get; set; }
        public string image_origin { get; set; }
        public float[] plate_bounding_box { get; set; }
        public string type { get; set; }
        public bool detected_plate { get; set; }
        public string detected_plate_text { get; set; }
        public string image_uuid { get; set; }
    }


    public class BdeoFraudSummaryPlateDetectionPlateComparison
    {
        public float fraud { get; set; }
    }

    public class BdeoFraudSummaryPlateDetectionCaseCheck
    {
        public float fraud { get; set; }
        public string real_text_plate { get; set; }
    }

    #endregion

    #region screenDetection

    public class BdeoFraudSummaryScreenDetection
    {
        public int fraud { get; set; }
        public BdeoFraudSummaryScreenDetectionDetail[] detail { get; set; }
    }

    public class BdeoFraudSummaryScreenDetectionDetail
    {
        public double result { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public string image_uuid { get; set; }
        public bool fraud { get; set; }
    }

    #endregion

    #endregion

    #region QualitySummary

    public class BdeoQualitySummary
    {
        public BdeoQualitySummaryOrientationDetection orientationDetection { get; set; }
        public int issue { get; set; }
        public BdeoQualitySummaryQualityDetection qualityDetection { get; set; }
        public BdeoQualitySummaryVehicleDetection vehicleDetection { get; set; }
    }

    #region OrientationDetection

    public class BdeoQualitySummaryOrientationDetection
    {
        public int issue { get; set; }
        public BdeoQualitySummaryOrientationDetectionDetail[] detail { get; set; }
    }

    public class BdeoQualitySummaryOrientationDetectionDetail
    {
        public bool result { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public bool issue { get; set; }
        public string image_uuid { get; set; }
    }

    #endregion

    #region QualityDetection

    public class BdeoQualitySummaryQualityDetection 
    {
        public float meanQuality { get; set; }
        public int quality_threshold { get; set; }
        public BdeoQualitySummaryQualityDetectionDetail[] detail { get; set; }
        public int issue { get; set; }
    }

    public class BdeoQualitySummaryQualityDetectionDetail
    {
        public float result { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public bool issue { get; set; }
        public string image_uuid { get; set; }
    }

    #endregion

    #region VehicleDetection

    public class BdeoQualitySummaryVehicleDetection
    {
        public int issue { get; set; }
        public BdeoQualitySummaryVehicleDetectionDetail[] detail { get; set; }
    }

    public class BdeoQualitySummaryVehicleDetectionDetail
    {
        public bool result { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public bool issue { get; set; }
        public float[] vehicle_bounding_box { get; set; }
        public string image_uuid { get; set; }
    }

    #endregion



    #endregion

    #region ProfilerSummary

    public class BdeoProfilerSummary
    {
        public BdeoProfilerSummaryCostEstimation cost_estimation { get; set; }
        public BdeoProfilerSummaryVehicleInformation vehicle_information { get; set; }
        public bool issue { get; set; }
        public BdeoProfilerSummaryDamages damages { get; set; }
        public BdeoProfilerSummaryFraud fraud { get; set; }
        public BdeoProfilerSummaryQuality quality { get; set; }
    }

    public class BdeoProfilerSummaryCostEstimation
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class BdeoProfilerSummaryVehicleInformation
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class BdeoProfilerSummaryDamages
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class BdeoProfilerSummaryFraud
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    public class BdeoProfilerSummaryQuality
    {
        public bool issue { get; set; }
        public string[] detail { get; set; }
    }

    #endregion

    #region DamageSummary
    public class BdeoDamageSummaryItem
    {
        public BdeoDamageSummaryItemMainDetection mainDetection { get; set; }
        public BdeoDamageSummaryItemMainDetection[] otherDetection { get; set; }
    }

    #region MainDetection
    
    public class BdeoDamageSummaryItemMainDetection
    {
        public BdeoDamageSummaryItemMainDetectionDamages[] damages { get; set; }
        public BdeoDamageSummaryItemMainDetectionPart part { get; set; }
        public BdeoDamageSummaryItemMainDetectionEstimation estimation { get; set; }
        public BdeoDamageSummaryItemMainDetectionImageInfo image_info { get; set; }
    }

    public class BdeoDamageSummaryItemMainDetectionDamages
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

    public class BdeoDamageSummaryItemMainDetectionPart
    {
        public string id { get; set; }
        public string description { get; set; }
    }

    public class BdeoDamageSummaryItemMainDetectionEstimation
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

    public class BdeoDamageSummaryItemMainDetectionImageInfo
    {
        public string type { get; set; }
        public string view { get; set; }
        public string image_uuid { get; set; }
        public string zoom { get; set; }
    }

    #endregion

    #endregion

    #region CostEstimationProvider

    public class BdeoCostEstimationProvider
    {
        public string name { get; set; }
        public string version { get; set; }
    }

    #endregion

    #region Basket

    public class BdeoBasket
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    #endregion

    #region Logs

    public class BdeoLog
    {
        public string action { get; set; }
        public int when { get; set; }
        public string rol { get; set; }
        public string who { get; set; }
        public BdeoLogMessageInfo messageInfo { get; set; }
        public BdeoLogImageInfo imageInfo { get; set; }
    }

    public class BdeoLogMessageInfo
    {
        public string type { get; set; }
        public string reciever { get; set; }
        public string text { get; set; }
    }

    public class BdeoLogImageInfo
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    #endregion

    #region Multimedia

    public class BdeoMultimedia
    {
        public string signedUrl { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int takePhotoAt { get; set; }
        public float[] coordinates { get; set; }
        public float coordinatesAccuracy { get; set; }
        public string image_uuid { get; set; }
    }

    #endregion

    #region ResultsAI

    public class BdeoResultsAI
    {
        public object front { get; set; }
        public object frontLeft { get; set; }
        public object rearLeft { get; set; }
        public object rear { get; set; }
        public object rearRight { get; set; }
        public object frontRight { get; set; }
    }

    #endregion
}