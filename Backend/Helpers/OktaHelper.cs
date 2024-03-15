using MSSPAPI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MSSPAPI.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class OktaHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool OktaValidation()
        {
        
            try
            {
                var client = new RestClient("https://dev-assurant.oktapreview.com/oauth2/default/v1/token");
                //client.Timeout = -1;
                var request = new RestRequest(client.Options.BaseUrl, Method.Post);
                // request.AddHeader("Ocp-Apim-Subscription-Key", "3018fb5eac4c4dc59059ac4f14e6f247");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("client_id", "0oa18srxbdcp9cSVa0h8");
                request.AddParameter("client_secret", "2rH2G8ZoU3vnTxLH8DFW_XhLEn5FgFwn3Ubl4MRR");
                request.AddParameter("scope", "read");//openid  read
                RestResponse response = client.Execute(request);


                if (response.StatusCode.ToString() == "OK")
                {
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion = "Se ha creado conexión con Okta correctamente. " + response.Content, Plataforma = "OKTA" };
                    DBOperations.InsertBitacora(btresp);
                    return true;
                }
                else
                {
                    Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")), Usuario = Environment.UserName, Descripcion =response.Content, Plataforma = "OKTA" };
                    DBOperations.InsertBitacora(btresp);
                    log.Error("Error al conectar con OKTA" + response.Content);
                    return false;
                }

            }
            catch(Exception ex)
            {
                //Bitacora btresp = new Bitacora() { IP = HttpContext.Current.Request.UserHostAddress, Navegador = HttpContext.Current.Request.Browser.Browser, Fecha = DateTime.Now, Usuario = Environment.UserName, Descripcion = ex.Message+" "+response.Content, Plataforma = "OKTA" };
                //DBOperations.InsertBitacora(btresp);
                log.Error("Error al conectar con OKTA" + ex.Message);
                return false;
            }
        }
        
    }
}