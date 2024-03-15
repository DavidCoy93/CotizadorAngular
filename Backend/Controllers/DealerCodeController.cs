using System;
using System.Collections.Generic;
using System.Web.Http;
using MSSPAPI.Helpers;
using MSSPAPI.Models;

namespace MSSPAPI.Controllers
{
    /// <summary>
    ///
    /// </summary>
    public class DealerCodeController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el dealer code por el id del cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetDealerCode/{id}")]
        public IHttpActionResult GetDealerCode(int id)
        {
            try
            {
                List<DealerCodes> dc = DBOperations.GetDealerCodesByIdCliente(id);
                return Ok(dc);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertDealerCode")]
        public IHttpActionResult InsertDealerCode(DealerCodes dl)
        {
            try
            {
                DealerCodes dealer = new DealerCodes
                {
                    IdCliente = dl.IdCliente,
                    DealerCode = dl.DealerCode,
                    TerminosDeUso = dl.TerminosDeUso,
                    TerminosDeGarantia = dl.TerminosDeGarantia,
                    FrequentlyAskedQuestions = dl.FrequentlyAskedQuestions,
                    DownloadCertificate = dl.DownloadCertificate,
                    DownloadDocument = dl.DownloadDocument,
                    ContactUs = dl.ContactUs,
                    TextSurvey = dl.TextSurvey,
                    ExternalURL = dl.ExternalURL,
                    Idioma = dl.Idioma,
                    Device = dl.Device,
                    Okta = dl.Okta,
                    ServiceOption = dl.ServiceOption,
                    PoliticaPrivacidad = dl.PoliticaPrivacidad,
                    TerminosCondiciones = dl.TerminosCondiciones,
                    IsSelected = dl.IsSelected,
                };

                DBOperations.InsertDealerCodes(dealer);
                return Ok(dl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dl"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateDealerCode")]
        public IHttpActionResult UpdateDealerCode(DealerCodes dl)
        {
            try
            {
                DealerCodes dealer = new DealerCodes
                {
                    IdDealerCode = dl.IdDealerCode,
                    IdCliente = dl.IdCliente,
                    DealerCode = dl.DealerCode,
                    TerminosDeUso = dl.TerminosDeUso,
                    TerminosDeGarantia = dl.TerminosDeGarantia,
                    FrequentlyAskedQuestions = dl.FrequentlyAskedQuestions,
                    DownloadCertificate = dl.DownloadCertificate,
                    DownloadDocument = dl.DownloadDocument,
                    ContactUs = dl.ContactUs,
                    TextSurvey = dl.TextSurvey,
                    ExternalURL = dl.ExternalURL,
                    Idioma = dl.Idioma,
                    Device = dl.Device,
                    Okta = dl.Okta,
                    ServiceOption = dl.ServiceOption,
                    PoliticaPrivacidad = dl.PoliticaPrivacidad,
                    TerminosCondiciones = dl.TerminosCondiciones,
                    IsSelected = dl.IsSelected
                };
                DBOperations.UpdateDealerCodes(dealer);
                return Ok(dl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}