using System;

namespace MSSPAPI.Helpers
{
    public class DateHelper
    {
        public static  string GetDateString(DateTime dt)
        {
            string dts = string.Empty;
            if (dt != null)
            {
               dts =  dt.ToString("s");             
            }
            return dts;
        }

        public static string GetDateParse(string date)
        {
            string[] datos = date.Split('/');
            string dia = datos[0];
            string mes = datos[1];
            string anio = datos[2].Substring(0,4);
            DateTime nuevafecha = new DateTime(Convert.ToInt32(anio), Convert.ToInt32(mes),Convert.ToInt32(dia));
            string formatted = nuevafecha.ToString("dd/M/yyyy");

            return formatted;
        }
    }
}
