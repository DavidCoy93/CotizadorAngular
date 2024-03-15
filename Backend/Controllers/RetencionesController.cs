using MSSPAPI.EmisionServiceDev;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Xml.Serialization;

namespace MSSPAPI.Controllers
{
    public class RetencionesController : ApiController
    {
        [HttpGet]
        [Route("api/retenciones/timbrar")]
        public IHttpActionResult Timbrar()
        {
            // <cfdi:Emisor Rfc="VSS160803DA0" Nombre="VIRGINIA SURETY SEGUROS DE MEXICO" RegimenFiscal="601" />
            Retenciones retenciones = new Retenciones();

            retenciones.Certificado = "";
            retenciones.Sello = string.Empty;
            retenciones.NoCertificado = string.Empty;

            retenciones.Version = "2.0";
            retenciones.FolioInt = "9";

            retenciones.FechaExp = DateTime.Now;
            retenciones.LugarExpRetenc = "45110";
            retenciones.CveRetenc = c_CveRetenc.Item01;

            retenciones.Emisor = new RetencionesEmisor()
            {
                NomDenRazSocE = "VIRGINIA SURETY SEGUROS DE MEXICO",
                RegimenFiscalE = c_RegimenFiscal.Item601,
                RfcE = "VSS160803DA0"
            };
            retenciones.Receptor = new RetencionesReceptor()
            {
                NacionalidadR = RetencionesReceptorNacionalidadR.Nacional,
                Item = null
            };
            retenciones.Periodo = new RetencionesPeriodo()
            {
                MesIni = c_Periodo.Item01,
                MesFin = c_Periodo.Item03,
                Ejercicio = c_Ejercicio.Item2023
            };
            retenciones.Totales = new RetencionesTotales()
            {
                MontoTotOperacion = (decimal)2000.00,
                MontoTotGrav = (decimal)2000.00,
                MontoTotExent = 0,
                MontoTotRet = (decimal)580.00,
                ImpRetenidos = new List<RetencionesTotalesImpRetenidos>()
                {
                    new RetencionesTotalesImpRetenidos() {
                        BaseRet = (decimal)2000,
                        ImpuestoRet = c_Impuesto.Item001,
                        MontoRet = (decimal)580.0,
                        TipoPagoRet = c_TipoPagoRet.Item03,
                        BaseRetSpecified = false,
                        ImpuestoRetSpecified = false,
                    }
                }.ToArray()
            };

            //retenciones.Complemento = null;
            //retenciones.DescRetenc = "";

            string xmlString = "";

            using (var sw = new StringWriter())
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                var serializer = new XmlSerializer(typeof(Retenciones));

                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                ns.Add("retenciones", "http://www.sat.gob.mx/esquemas/retencionpago/2");
                serializer.Serialize(sw, retenciones, ns);
                xmlString = sw.ToString();
            }

            EmisionServiceDev.EmisionClient emisionClient = new EmisionServiceDev.EmisionClient();

            ReporteProcesamiento reporte = new ReporteProcesamiento();

            //reporte.Resultado[0].Documentos[0].
            ComprobanteData cd = new ComprobanteData();


            string apiKey = "vkyjs7g9bvucdsmm3iccqehg7ekv22iavd5jrxdkudy8txnysdrnk5fyk49fhkpv6df4tpzwwc5askjd";
            ErrorMessageCode messageCode = new ErrorMessageCode();
            TransactionProperty[] transactionProp = new TransactionProperty[0];
            string xmlResult = string.Empty;

            Guid uuid = emisionClient.EmitirComprobante(
                apiKey,
                xmlString,
                null,
                ref transactionProp,
                null,
                out messageCode,
                out xmlResult
            );

            Console.WriteLine(uuid.ToString());
            Console.WriteLine(xmlResult);

            if (messageCode.Code == "200")
            {
                return Ok(Json(new { xmlString = xmlString }));
            }
            else
            {
                string detailedErrorMessage = string.Empty;

                foreach (var error in messageCode.InnerErrors) {
                    detailedErrorMessage += error.Message + "\r\n";
                }

                return BadRequest(detailedErrorMessage.Trim());
            }

            
        }
    }
}