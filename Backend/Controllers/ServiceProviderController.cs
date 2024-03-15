using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Controlador que contiene todos lo metodos CRUD para la manipulación de las configuraciones de los proveedores de servicios externos
    /// </summary>
    public class ServiceProviderController : ApiController
    {

        /// <summary>
        /// Endpoint para obtener la configuración de un proveedor
        /// </summary>
        /// <param name="Id">Identificador del proveedor de servicio</param>
        /// <param name="ProviderName">Nombre del proveedor a consultar</param>
        /// <response code="200">Se ha obtenido el objeto de configuración de un proveedor</response>
        /// <response code="400">Ocurrio un error al obtener el objecto de la base de datos</response>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetServiceProviderConfig")]
        [ResponseType(typeof(List<ServiceProviderConfiguration>))]
        public IHttpActionResult GetServiceProviderConfig(int? Id = null, string ProviderName = null)
        {
            try
            {
                List<ServiceProviderConfiguration> serviceProviders = DBOperations.GetServiceProviderConfiguration(Id, ProviderName);

                return Ok(serviceProviders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint para insertar una nueva configuración de un proveedor
        /// </summary>
        /// <param name="providerConfiguration"></param>
        /// <response code="200">Se ha creado correctamente el objecto de configuración</response>
        /// <response code="500">Ocurrio un error al crear el registro en la base de datos</response>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("AddServiceProviderConfig")]
        [ResponseType(typeof(ServiceProviderConfiguration))]
        public IHttpActionResult AddServiceProviderConfig(ServiceProviderConfiguration providerConfiguration)
        {
            try
            {

                providerConfiguration.SvcPwd = (!string.IsNullOrEmpty(providerConfiguration.SvcPwd)) ? EncDec.Encript(providerConfiguration.SvcPwd) : "";

                DBOperations.CRUDServiceProviderConfiguration(ref providerConfiguration, "C");
                return Ok(providerConfiguration);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new { Message = string.Format("{0}, Error: {1}", "Ocurrio un error inesperado al agregar la configuración", ex.Message), IsSuccess = false });
            }
        }


        /// <summary>
        /// Endpoint para actualizar la configuracíon de un proveedor de servicio
        /// </summary>
        /// <param name="providerConfiguration">Objeto tipo proveedor de servicio</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [Route("UpdateServiceProviderConfig")]
        [ResponseType(typeof(ServiceProviderConfiguration))]
        public IHttpActionResult UpdateServiceProviderConfig(ServiceProviderConfiguration providerConfiguration)
        {
            try
            {
                providerConfiguration.SvcPwd = (!string.IsNullOrEmpty(providerConfiguration.SvcPwd)) ? EncDec.Encript(providerConfiguration.SvcPwd) : "";

                DBOperations.CRUDServiceProviderConfiguration(ref providerConfiguration, "U");
                return Ok(providerConfiguration);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new { Message = string.Format("{0}, Error: {1}", "Ocurrio un error inesperado al modificar la configuración", ex.Message), IsSuccess = false });
            }
        }

        /// <summary>
        /// Endpoint para deshabilitar la configuracíon de un proveedor de servicio
        /// </summary>
        /// <param name="IdProviderConfig">Identificador del proveedor</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("DeleteServiceProviderConfig/{IdProviderConfig}")]
        public IHttpActionResult DeleteServiceProviderConfig(int IdProviderConfig)
        {
            try
            {
                ServiceProviderConfiguration providerConfiguration = new ServiceProviderConfiguration();
                providerConfiguration.Id = IdProviderConfig;

                DBOperations.CRUDServiceProviderConfiguration(ref providerConfiguration, "D");

                return Ok(new { Message = "Se ha eliminado la configuración correctamente", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new { Message = string.Format("{0}, Error: {1}", "Ocurrio un error inesperado al eliminar la configuración", ex.Message), IsSuccess = false });
            }
        }

    }
}
