using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MSSPAPI.ClaimRecordingServiceWS;
using MSSPAPI.Models;
using Newtonsoft.Json;

namespace MSSPAPI.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class ClaimFolioHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string FolioGeneration()
        {
            try
            {
                #region Variables
                string currentdate = DateTime.Now.ToShortDateString();
                int cont = 1;
                string Anio = "";
                string Mes = "";
                string Dia = "";
                string Hora = "";
                string Minutos = "";
                string Folio = "";
                string Contador = "";
                #endregion
                //Se obtiene ultimo folio registrado
                ClaimFolio objClaimFolio = DBOperations.GetLastClaimFolio();
                if (objClaimFolio.Folio != null)
                {
                    //Codigo para comparar ultimo folio (FechaCreación)
                    if (objClaimFolio.FechaCreacion.ToShortDateString() == currentdate)
                    {
                        //Se obtiene el ultimo folio, se le agrega hora, minutos actual y se agrega +1 al folio.
                        #region Generate Folio
                        cont = Convert.ToInt32(objClaimFolio.Folio.Substring(10, 2));
                        cont++;
                        Contador = cont.ToString();
                        if (Contador.Length == 1)
                            Contador = Contador.PadLeft(2, '0');
                        Hora = DateTime.Now.Hour.ToString();
                        if (Hora.Length == 1)
                            Hora = Hora.PadLeft(2, '0');
                        Minutos = DateTime.Now.Minute.ToString();
                        if (Minutos.Length == 1)
                            Minutos = Minutos.PadLeft(2, '0');
                        if (Contador.Length == 1)
                            Contador = Contador.PadLeft(2, '0');
                        Folio = objClaimFolio.Folio.Substring(0, 6) + Hora + Minutos + Contador;
                        return Folio;
                        #endregion
                    }
                    else
                    {
                        //Se genera primer folio del dia
                        #region Generate Folio
                        Contador = cont.ToString();
                        Anio = DateTime.Now.Year.ToString().Substring(2, 2);
                        Mes = DateTime.Now.Month.ToString();
                        Dia = DateTime.Now.Day.ToString();
                        Hora = DateTime.Now.Hour.ToString();
                        Minutos = DateTime.Now.Minute.ToString();
                        if (Mes.Length == 1)
                            Mes = Mes.PadLeft(2, '0');
                        if (Dia.Length == 1)
                            Dia = Dia.PadLeft(2, '0');
                        if (Hora.Length == 1)
                            Hora = Hora.PadLeft(2, '0');
                        if (Minutos.Length == 1)
                            Minutos = Minutos.PadLeft(2, '0');
                        if (Contador.Length == 1)
                            Contador = Contador.PadLeft(2, '0');
                        Folio = Anio + Mes + Dia + Hora + Minutos + Contador;
                        return Folio;
                        #endregion
                    }
                }
                else
                {
                    //Se genera primer Folio de la tabla ClaimFolio
                    #region Generate Folio
                    Contador = cont.ToString();
                    Anio = DateTime.Now.Year.ToString().Substring(2, 2);
                    Mes = DateTime.Now.Month.ToString();
                    Dia = DateTime.Now.Day.ToString();
                    Hora = DateTime.Now.Hour.ToString();
                    Minutos = DateTime.Now.Minute.ToString();
                    if (Mes.Length == 1)
                        Mes = Mes.PadLeft(2, '0');
                    if (Dia.Length == 1)
                        Dia = Dia.PadLeft(2, '0');
                    if (Hora.Length == 1)
                        Hora = Hora.PadLeft(2, '0');
                    if (Minutos.Length == 1)
                        Minutos = Minutos.PadLeft(2, '0');
                    if (Contador.Length == 1)
                        Contador = Contador.PadLeft(2, '0');
                    Folio = Anio + Mes + Dia + Hora + Minutos + Contador;
                    return Folio;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return "000000000000";
            }
        }
        /// <summary>
        /// Metodo para generar Json Simulado en el Controlador de SubmitQuestionSet, ya que no admite más de 2 metodos el Swagger.
        /// </summary>
        /// <returns></returns>
        public static string GenerateJsonResponseSimulation()
        {
            ClaimFolio folio = DBOperations.GetLastClaimFolio();
            string data = "{\"Message\":\"DNOCO\",\"CompanyCode\":\"ASM\",\"Actions\":null,\"CaseNumber\":\"2022069726\",\"DecisionType\":1,\"InteractionNumber\":\"2022108160\",\"ClaimRecordingMessages\":null,\"Fields\":[],\"CustomerInfo\":{\"CustomerName\":\"GOMEZ ZAMARRIPA SONIA KARINA\",\"EmailAddress\":\"\",\"HomePhone\":\"08100943\",\"WorkPhone\":\"\"},\"MethodOfRepairCode\":null,\"ClaimStatus\":\"A\", \"ClaimNumber\":\"" + folio.Folio + "\"}";
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Preguntas(dynamic data)
        {
            dynamic datos = JsonConvert.SerializeObject(data);
            try
            {
                if (datos != null && datos != "")
                {
                    string preguntas = "";
                    dynamic jsonString = JsonConvert.DeserializeObject(datos);
                    int loop = 0;
                    foreach (var item in jsonString.Questions)
                    {
                        int type = Convert.ToInt32(item.AnswerType);
                        switch (type)
                        {
                            case 1:
                                string q1 = jsonString.Questions[loop]["Text"];
                                string a1 = jsonString.Questions[loop]["Answer"]["Answer"];
                                //DateAnswer da = new DateAnswer { Answer = Convert.ToDateTime(DateHelper.GetDateString(DateTime.ParseExact(a1, "dd/MM/yyyy", CultureInfo.InvariantCulture))) };
                                string[] ans = a1.Split(' ');
                                preguntas = preguntas + q1 + " : " + ans[0] + ", <br>";
                                loop++;
                                break;
                            case 2:
                                string q2 = jsonString.Questions[loop]["Text"];
                                string a2 = jsonString.Questions[loop]["Answer"]["Answer"];
                                preguntas = preguntas + q2 + " : " + a2 + ", <br>";
                                loop++;
                                break;
                            case 3:
                                string q3 = jsonString.Questions[loop]["Text"];
                                string a3 = jsonString.Questions[loop]["Answer"]["Answer"]["Text"];
                                preguntas = preguntas + q3 + " : " + a3 + ", <br>";
                                loop++;
                                break;
                        }
                    }
                    return preguntas;
                }
                else
                {
                    return "No existe Cuestionario";
                }
            }
            catch (Exception ex)
            {
                return "No existe Cuestionario";
            }
        }

        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionid"></param>
        /// <param name="serialnumber"></param>
        /// <returns></returns>
        public static string getArticuloBySessionId(string sessionid, string serialnumber)
        {
            try
            {
                TransaccionData trans = DBOperations.GetTransaccionByNumeroTransaccion(sessionid);
                if (trans.TransaccionJson != null)
                {
                    dynamic TransactionJson = trans.TransaccionJson;
                    dynamic jsonString = JsonConvert.DeserializeObject(TransactionJson);
                    foreach (var item in jsonString.RequestIntegration)
                    {
                        if (serialnumber == Convert.ToString(item.SRNumber))
                        {
                            string dataResponse = "SRNumber: " + item.SRNumber + ", <br>" + "Area: " + item.Area + ", <br>" + "SubArea: " + item.SubArea + ", <br>" + "Tipo Aclaración: " + item.TipoAclaracion + ", <br>" + "Abstract: " + item.Abstract;
                            return dataResponse;
                        }
                    }
                }
                else
                {
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}