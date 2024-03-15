using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Controlador para el manejo de los estatus del caso
    /// </summary>
    [RoutePrefix("CaseStatus")]
    public class CaseStatusController : ApiController
    {
        /// <summary>
        /// Endpoint para obtener el listado de estatus
        /// </summary>
        /// <param name="Id">Identificador del estatus (opcional)</param>
        /// <param name="Description">Descripción del estatus (opcional)</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("")]
        [ResponseType(typeof(List<CaseStatus>))]
        public async Task<HttpResponseMessage> GetCaseStatusList(int? Id = null, string Description = null)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                List<CaseStatus> statuses = await DBOperations.GetCaseStatus(Id, Description);
                httpResponse.StatusCode = HttpStatusCode.OK;
                httpResponse.Content = new ObjectContent<List<CaseStatus>>(statuses, formatter);
            }
            catch (Exception ex)
            {

                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
            }

            return httpResponse;
        }


        /// <summary>
        /// Enpoint para agregar un nuevo estatus
        /// </summary>
        /// <param name="caseStatus">Objeto tipo estatus</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("")]
        [ResponseType(typeof(CaseStatus))]
        public async Task<HttpResponseMessage> AddCaseStatus(CaseStatus caseStatus)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));


            try
            {
                CaseStatus status = await DBOperations.CaseStatusCrud(caseStatus, "C");
                httpResponse.StatusCode = HttpStatusCode.OK;
                httpResponse.Content = new ObjectContent<CaseStatus>(status, formatter);
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
            }

            return httpResponse;
        }

        /// <summary>
        /// Enpoint para actualizar un estatus
        /// </summary>
        /// <param name="caseStatus">Objeto tipo estatus</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("")]
        [ResponseType(typeof(CaseStatus))]
        public async Task<HttpResponseMessage> UpdateCaseStatus(CaseStatus caseStatus)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                CaseStatus status = await DBOperations.CaseStatusCrud(caseStatus, "U");
                httpResponse.StatusCode = HttpStatusCode.OK;
                httpResponse.Content = new ObjectContent<CaseStatus>(status, formatter);
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode = HttpStatusCode.InternalServerError;
                httpResponse.Content = new StringContent(ex.Message);
            }

            return httpResponse;
        }


        /// <summary>
        /// Endpoint para deshabilitar un estatus
        /// </summary>
        /// <param name="Id">Identificador del estatus</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("{Id}")]
        [ResponseType(typeof(string))]
        public async Task<HttpResponseMessage> DisableCaseStatus(int Id)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            try
            {
                CaseStatus caseStatus = new CaseStatus() { Id = Id };
                await DBOperations.CaseStatusCrud(caseStatus, "D");

                httpResponse.StatusCode = HttpStatusCode.OK;
                httpResponse.Content = new StringContent($"Se ha deshabilitado el estutus con el Id {Id}");
            }
            catch (Exception ex)
            {
                httpResponse.StatusCode=HttpStatusCode.InternalServerError;
                httpResponse.Content= new StringContent(ex.Message);
            }

            return httpResponse;
        }
    }
}
