using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// Cotrolador donde se encuetran los enpoints para el manejo de casos
    /// </summary>
    /// 
    [RoutePrefix("Case")]
    public class CaseController : ApiController
    {
        /// <summary>
        /// Endpoint para obtener un caso por identificador o por numero de telefono
        /// </summary>
        /// <param name="CaseIdentifier">Identificador del caso</param>
        /// <param name="PhoneNumber">Numero de telefono con el que se inicio el caso</param>
        /// <returns>Un objeto de tipo caso</returns>
        [HttpGet]
        [Authorize]
        [Route("")]
        [ResponseType(typeof(List<Case>))]
        public IHttpActionResult GetCaseByIdentifierOrPhoneNumber(string CaseIdentifier = "", string PhoneNumber = "")
        {
            if (string.IsNullOrEmpty(CaseIdentifier) && string.IsNullOrEmpty(PhoneNumber))
            {
                return BadRequest("Se requiere almenos un parametro");
            } 
            else
            {
                try
                {
                    List<Case> caseList = DBOperations.getCaseByIdentifierOrPhoneNumber(CaseIdentifier, PhoneNumber);
                    return Ok(caseList);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            } 
        }

        /// <summary>
        /// Endpoint para crear un nuevo caso
        /// </summary>
        /// <param name="customerData">Objecto JSON con los datos del cliente</param>
        /// <param name="PhoneNumber">Número telefonico del cliente con el cual se va a crear el caso</param>
        /// <response code="200">Devuelve un objecto de tipo caso con el identificador con el cual se va a dar seguimiento</response>
        /// <response code="400">Devuelve un mensaje de error cuando ocurre una excepción</response>
        /// <response code="400">Devuelve un error si ya existe un caso asociado con el numero telefonico y este se encuentra con estatus creado ó en progreso</response>
        /// <returns></returns>
        [HttpPost]
        [Route("{PhoneNumber}")]
        [Authorize]
        [ResponseType(typeof(Case))]
        public IHttpActionResult CreateCase(object customerData, string PhoneNumber)
        {
            try
            {
                Case _case = new Case() 
                { 
                    Id = 0,
                    PhoneNumber = PhoneNumber,
                    StatusId = 1,
                    Status = "",
                    CaseIdentifier = "",
                    CustomerData = "",
                    Active = true
                };

                if (customerData.GetType() == typeof(JObject))
                {
                    JObject customerDataJSON = (JObject)customerData;
                    _case.CustomerData = JsonConvert.SerializeObject(customerDataJSON, Formatting.Indented);
                } 
                else if (customerData.GetType() == typeof(string))
                {
                    _case.CustomerData = (string)customerData;
                }

                DBOperations.CRUDCase(ref _case, "C");

                return Ok(_case); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Enpoint para actualizar un caso
        /// </summary>
        /// <param name="CustomerData">JSON con la información proporcionada por el cliente</param>
        /// <param name="CaseIdentifier">Identificador del caso</param>
        /// <param name="Status">Estatus del caso, para cambiar el estatus del caso, enviar este parametro</param>
        /// <response code="200">Devuelve un objeto de tipo caso con la información actualizada</response>
        /// <response code="400">Ocurre cuando no se envia un objecto de tipo JSON válido</response>
        /// <response code="400">Ocurre cuando se produce una excepción</response>
        /// <returns></returns>
        [HttpPatch]
        [Route("{CaseIdentifier}")]
        [Authorize]
        [ResponseType(typeof(Case))]
        public IHttpActionResult UpdateCase(object CustomerData, string CaseIdentifier, int Status = 0)
        {
            Type type = CustomerData.GetType();

            if (type == typeof(JObject))
            {
                try
                {
                    JObject CustomerJSON = (JObject)CustomerData;

                    List<Case> caseList = DBOperations.getCaseByIdentifierOrPhoneNumber(CaseIdentifier);

                    if (caseList.Count > 0)
                    {
                        Case _case = caseList.ElementAt(0);
                        _case.StatusId = (Status > 0 && _case.StatusId != Status) ? Status : _case.StatusId;
                        if (!string.IsNullOrEmpty(_case.CustomerData))
                        {
                            JObject context = JObject.Parse(_case.CustomerData);

                            foreach (JProperty property in CustomerJSON.Children())
                            {
                                JProperty contextProp = context.Properties().Where(p => p.Name == property.Name).FirstOrDefault();

                                if (contextProp == null)
                                {
                                    context.Add(property.Name, CustomerJSON.SelectToken(property.Name));
                                } 
                                else
                                {
                                    if (contextProp.Value.Type == JTokenType.Array)
                                    {
                                        JArray arrayProp = (JArray)contextProp.Value;

                                        foreach (JToken elementLvl1 in property.Value.Children())
                                        {
                                            arrayProp.Add(elementLvl1);
                                        }
                                    }
                                    else
                                    {
                                        contextProp.Replace(property);
                                    }
                                }
                            }
                            _case.CustomerData = context.ToString();
                        } 
                        else
                        {
                            _case.CustomerData = CustomerJSON.ToString();
                        }

                        DBOperations.CRUDCase(ref _case, "U");

                        return Ok(_case);
                    } 
                    else
                    {
                        return BadRequest("No se encontro un caso con el identificador proporcionado");
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            } 
            else
            {
                return BadRequest("Por favor envie un JSON válido");
            }
        }
    
    }
}
