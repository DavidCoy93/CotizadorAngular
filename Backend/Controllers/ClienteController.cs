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
    public class ClienteController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetAllClientes")]
        public IHttpActionResult GetAllClientes()
        {
            try
            {
                List<Cliente> cls = DBOperations.GetAllClientes();

                return Ok(cls);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("api/GetCliente/{Id}")]
        public IHttpActionResult GetCliente(int Id)
        {
            try
            {
                Cliente cls = DBOperations.GetCliente(Id);
                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(Id);

                cls.Login = Fa.EnabledLogin;
                cls.Flujo = Fa.Enabled;
                cls.Multiple = Fa.EnabledMultiple;

                return Ok(cls);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertCliente")]
        public IHttpActionResult InsertCliente(Cliente cliente)
        {
            try
            {
                Cliente cl = new Cliente
                {
                    NombreCliente = cliente.NombreCliente,
                    Configuraciones = cliente.Configuraciones,
                    Apikey = cliente.Apikey,
                    Authorization = cliente.Authorization,
                    RiskTypeCode = cliente.RiskTypeCode,
                    Active = cliente.Active,
                    Payments = cliente.Payments,
                    URL = cliente.URL
                };
                DBOperations.InsertCliente(cl);
                // Funcion APP
                Cliente c = DBOperations.GetClientesLast();
                GenerarToken th = new GenerarToken();
                string clave = th.GeneraMSSPClave(c.IdCliente); //genera el string que sera encriptado para crear el token
                string token = th.BuildToken(clave); //crea el token
                DBOperations.InsertNewMSSPToken(token, c.IdCliente); //inserta el token mssp
                FlujoAlterno fa = new FlujoAlterno
                {
                    IdCliente = c.IdCliente,
                    Enabled = cliente.Flujo,
                    EnabledLogin = cliente.Login
                };
                DBOperations.InsertFlujoAlterno(fa);
                foreach (var cli in cliente.DealerCode)
                {
                    //insertar dealer code
                    DealerCodes dc = new DealerCodes
                    {
                        IdCliente = c.IdCliente,
                        DealerCode = cli.DealerCode,
                        ContactUs = cli.ContactUs,
                        TerminosCondiciones= cli.TerminosCondiciones,
                        Device= cli.Device,
                        TerminosDeUso = cli.TerminosDeUso,    
                        TerminosDeGarantia = cli.TerminosDeGarantia,
                        FrequentlyAskedQuestions = cli.FrequentlyAskedQuestions,
                        DownloadCertificate = cli.DownloadCertificate,
                        DownloadDocument = cli.DownloadDocument,                     
                        TextSurvey = cli.TextSurvey,
                        ExternalURL = cli.ExternalURL,
                        Idioma = cli.Idioma,                    
                        Okta = cli.Okta,
                        ServiceOption = cli.Okta,
                        PoliticaPrivacidad = cli.PoliticaPrivacidad,
                        DatosRequeridos = cli.DatosRequeridos,
                        IsSelected = false
                    };
                    DBOperations.InsertDealerCodes(dc);
                }
                return Ok(c);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UpdateCliente")]
        public IHttpActionResult UpdateCliente(Cliente cliente)
        {
            try
            {
                //Metodo para editar cliente
                Cliente cl = new Cliente
                {
                    IdCliente = cliente.IdCliente,
                    NombreCliente = cliente.NombreCliente,
                    Configuraciones = cliente.Configuraciones,
                    Apikey = cliente.Apikey,
                    Authorization = cliente.Authorization,
                    RiskTypeCode = cliente.RiskTypeCode,
                    Active = cliente.Active,
                    Payments = cliente.Payments,
                    URL = cliente.URL
                };
                DBOperations.UpdateCliente(cl);

                foreach (var cli in cliente.DealerCode)
                {
                    //insertar dealer code
                    DealerCodes dc = new DealerCodes
                    {
                        IdCliente = cli.IdCliente,
                        IdDealerCode = cli.IdDealerCode,
                        DealerCode = cli.DealerCode,
                        ContactUs = cli.ContactUs,
                        TerminosCondiciones = cli.TerminosCondiciones,
                        Device = cli.Device,
                        TerminosDeUso = cli.TerminosDeUso,
                        TerminosDeGarantia = cli.TerminosDeGarantia,
                        FrequentlyAskedQuestions = cli.FrequentlyAskedQuestions,
                        DownloadCertificate = cli.DownloadCertificate,
                        DownloadDocument = cli.DownloadDocument,
                        TextSurvey = cli.TextSurvey,
                        ExternalURL = cli.ExternalURL,
                        Idioma = cli.Idioma,
                        Okta = cli.Okta,
                        ServiceOption = cli.Okta,
                        PoliticaPrivacidad = cli.PoliticaPrivacidad,
                        DatosRequeridos = cli.DatosRequeridos,
                        ProductCodeIcon = cli.ProductCodeIcon,
                        IsSelected = false,
                        VariablesBusquedaNS = cli.VariablesBusquedaNS,
                        DealerGroup = cli.DealerGroup,
                        CompanyCode = cli.CompanyCode
                    };
                    DBOperations.UpdateDealerCodes(dc);
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DisableCliente")]
        public IHttpActionResult DisableCliente(int cliente)
        {
            try
            {
                DBOperations.DesactivaCliente(cliente);
                return Ok(cliente);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}