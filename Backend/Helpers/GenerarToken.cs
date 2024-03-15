using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MSSPAPI.Helpers

{
    public class GenerarToken
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GENERA EL TOKEN PARA SISTEMA GSSP
        public string GeneraMSSPClave(int cliente)
        {
            try
            {
                List<Claves> cl = DBOperations.GetClaves(cliente);
                string result = cl.First().Clave + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day; //Hacerlo dinamico para los clientes, SWITCH
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public String BuildToken(String clave)
        {
            string token = "";
            clave = clave.Trim();
            if (clave.Length > 0)
            {
                try
                {
                    Calendar calendar = new GregorianCalendar();
                    DateTime today = DateTime.UtcNow;
                    int year = calendar.GetYear(today);
                    int month = calendar.GetMonth(today);
                    int day = calendar.GetDayOfMonth(today);
                    string strBase = clave.Trim();
                    char[] baseCharArray = strBase.ToCharArray();
                    string strMapa = " ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÃÕÑÂÊÎÔÛÄËÏÖÜÀÈÌÒÙabcdefghijklmnopqrstuvwxyz0123456789áéíóúãõñâêîôûäëïöüàèìòùÇç/";
                    string strEncripto = "89áéíxyz01ìòùÇç67óúãÒÙabcvw23lmJKLM ABCD45õñâpqrstuêîÜÀÈÌôûäëïöüÃÕÑàèÏÖdeÉÍÔÛÄYZÁÓÚÂfgOPQ/XÊÎËINSTVWhijkGHUnEFoR";

                    int posicao = 0;
                    char caracter;
                    for (int i = 0; i < strBase.Length; i++)
                    {
                        posicao = strMapa.IndexOf(strBase[i]);
                        caracter = (posicao > 0) ? strEncripto[posicao] : strBase[i];
                        token = token + charParaAsc(caracter, day); ;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return token;
        }

        private static int charParaAsc(char caracter, int incremento)
        {
            return (int)caracter + incremento;
        }
    }
}