using System.Web.Http;
using WebActivatorEx;
using MSSPAPI;
using Swashbuckle.Application;
using System.IO;
using MSSPAPI.Helpers;
using System.Data;
using System.Linq;
using MSSPAPI.Globals;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities.Collections;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace MSSPAPI
{
    /// <summary>
    ///
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected static string GetXmlCommentsPath()
        {
            return Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "bin", "MSSPAPI.xml");
        }

        /// <summary>
        ///
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration

                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "MSSAPI Web API.")
                        .Description("Web API  \n " + GetTokens() + " \n Conexión elita; \n " + Constants.URLPOLICY)
                        .TermsOfService("Términos de servicio")
                        .Contact(x => x
                            .Name("")
                            .Url("")
                            .Email(""))
                        .License(x => x
                            .Name("")
                            .Url("")
                        )
                        ;

                        c.IncludeXmlComments(GetXmlCommentsPath());
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        //c.ApiKey("tokenmssp")
                        //    .Description("")
                        //    .Name("Bearer")
                        //    .In("header");

                        c.PrettyPrint();

                        c.OperationFilter<BDEOTokenHeaderParameter>();

                    })

                .EnableSwaggerUi(c =>
                    {
                        //c.EnableApiKeySupport("tokenmssp", "header");
                    });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static string GetTokens()
        {
            string tokens = "";
           DataTable dt = DBOperations.GetTokens();

            foreach (DataRow cliente in dt.Rows)
            {
                tokens += " \n Id Cliente " + cliente["IdCliente"].ToString() + ":  " + cliente["NombreCliente"].ToString() + " Token " + cliente["Token"].ToString() + "\n";
            }

            return tokens;
        }

      
    }


    public class BDEOTokenHeaderParameter: IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {

            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            if (apiDescription.RelativePath != "GetToken" && apiDescription.RelativePath.Split('/')[0] != "SisNet")
            {
                operation.parameters.Add(new Parameter { name = "Authorization", @in = "header", type = "string", required = true, description = "Token de seguridad" });
                operation.parameters.Add(new Parameter { name = "ApiKey", @in = "header", type = "string", required = true, description = "Api Key Del cliente" });
            }
            else
            {
                Console.WriteLine(apiDescription.RelativePath);
            }

            if (apiDescription.RelativePath.Contains("BDEO") && apiDescription.RelativePath != "BDEO/Login")
            {
                operation.parameters.Add(new Parameter { name = "BDEOToken", @in = "header", type = "string", required = true });
            }

            switch (operation.tags[0].ToLower())
            {
                case "aizpay":
                case "auth":
                case "avisoprivacidad":
                case "azureblobstorage":
                case "bdeo":
                case "case":
                case "casestatus":
                case "dataevaluation":
                case "emision":
                case "encuesta":
                case "openpay":
                case "rate":
                case "program":
                case "serviceprovider":
                case "enrollment":
                    operation.tags[0] = "Kitt";
                    break;
                case "sisnet":
                    operation.tags[0] = "SisNet";
                    break;
                default:
                    operation.tags[0] = "Middleware";
                    break;

            }
        }
    }
}