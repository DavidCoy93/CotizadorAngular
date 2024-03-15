using System.Web.Http;
using MSSPAPI.Globals;
using System;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Web;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

namespace MSSPAPI.Controllers
{

    public class NewCertificateController : ApiController
    {
        [HttpGet]
        [Route("api/GetCertificatePurshateDate/{poliza}/{certificate}/{id}")]
        public IHttpActionResult GetCertificatePurshateDate(string poliza, string certificate, string id)
        {
            
            string jsonresponse = string.Empty;
            string jsonrequest = string.Empty;
            string jsonConfig = string.Empty;

            var claimServiceClient = new PolicyServiceWS.PolicyServiceClient(Constants.ENPTNAMEPOLICY, Constants.URLPOLICY);
            Cliente cl = DBOperations.GetClientesById(Convert.ToInt32(id)); //Constantes cuando haya mas clientes
            jsonConfig = cl.Configuraciones;
            dynamic jsonObj = JsonConvert.DeserializeObject(jsonConfig);
            claimServiceClient.ClientCredentials.UserName.UserName = jsonObj["User"];
            claimServiceClient.ClientCredentials.UserName.Password = jsonObj["Password"];


            //var claimlookup = new PolicyServiceWS.SearchPolicyByMembershipNumber
            //{
            //    MembershipNumber = "",
            //    CompanyCode = jsonObj["CompanyCode"],
            //    DealerCode = "EA127"
            //};

            //var searchRequest = new PolicyServiceWS.SearchRequest
            //{
            //    PolicyLookup = claimlookup
            //};

            //var claimlookup = new PolicyServiceWS.SearchPolicyByCertificateNumber

            //{
            //    CertificateNumber = "6164AF73-4C26-11ED-9C10-005056B2FFE6-XX1",
            //    CompanyCode = "ASM",
            //    DealerCode = "EA127"

            //};

            //var searchRequest = new PolicyServiceWS.SearchRequest
            //{
            //    PolicyLookup = claimlookup
            //};

            //var response = claimServiceClient.Search(searchRequest);
            //jsonresponse = JsonConvert.SerializeObject(response);

            var requestPolicy = new PolicyServiceWS.GetDetailsRequest
            {
                CertificateNumber = certificate,
                DealerCode = "EA127"
            };

            var response = claimServiceClient.GetDetails(requestPolicy);
            jsonresponse = JsonConvert.SerializeObject(response);


            return Ok(jsonresponse);
        }
    }
}