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
    public class SaveMethodRepairController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [HttpPost]
        [Route("api/InsertCliente")]
        public IHttpActionResult InsertMethodRepair(List<MethodRepair> mt)
        {
            try
            {
                MethodRepair mtList = new MethodRepair
                {
                    IdMethodRepair = mt.IdMethodRepair,
         DealerCode
      RiskType 
         Marca 
        StateProvidence 
         StateCode 
        City 
        ServiceCenterCode 
         ServiceCenterName 
         Output 
        CreationDate
        UserCreator 
       Active 
        HorarioS
         HorarioF 
         Email1 
         Email2
        Bairro 
        CEP 
       Phone1 
       Phone2 
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

        
    }
}