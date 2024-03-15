using System;
using MSSPAPI.Models;
using MSSPAPI.Helpers;
using System.Web.Mvc;

namespace MSSPAPI.Controllers
{
    public class ConfigurationQuestionController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: ConfigurationQuestion
        public string GetConfiguration(string id)
        {
            try
            {
                string logs = DBOperations.GetQuestionTemplate(Convert.ToInt32(id));
                return (logs);
            }
            catch (Exception ex)
            {
                //TODO: AGREGAR LOS PARAMETROS PARA INSERT EN BASE DE DATOS DE LOGS
                //Logs lg = new Logs() { Descripcion = "TestInsert", IP = "10.1.2.3", Navegador = "Firefox", Request="jnkher4wdjioooouhskfyhuugtrfdestrfyuhytrfekdshcbslhDeiwufweuihfeuhfp9w8ehfjedcbzlhdbvlieugfwegfwiuduibcjzsdhbcvwiegq89yep9q38yeiuwcblxjhdbvluisygfpwe9fgp9w8yr3p98ye93yp923r937ryiweuflsh"};
                //bool insert = DBOperations.InsertLog(lg);
                log.Error(string.Format("{0}", ex.Message), ex);
                return null;
            }

        }
    }
}