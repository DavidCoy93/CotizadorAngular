using Microsoft.Extensions.DependencyInjection;
using MSSPAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MSSPAPI
{
    /// <summary>
    ///
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web

            // Rutas de API web
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new ValidatorTokenHandler());
        }
    }
}