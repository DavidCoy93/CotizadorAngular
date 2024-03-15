﻿using MSSPAPI.Helpers;
using MSSPAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace MSSPAPI.Controllers
{
    ///
    public class CountryController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET api/ApiKey/1-IQTAUX6
        /// <summary>
        /// Solicita el token, Dealer Code, Authorization y Risk Type Code y lo devuelve al portal
        /// </summary>
        /// <param name="dealercode"></param>
        /// <returns></returns>
        [Route("api/Country/{Id}")]
        public string GetCountry(int Id)
        {
            string js = null;
            try
            {
                js = DBOperations.GetCountry(Id);

                  return js;
            }
            catch (Exception ex)
            {
                log.Error("Error al consultar" + ex.Message);
                return ex.Message;
            }
}
    }
}