using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Description;
using System.Web.Mvc;


namespace MSSPAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // GET: LoginModel
        public ActionResult Index()
        {
            ViewBag.Title = "MSSP";

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="User"></param>
        /// 
        /// <param name="lgn"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Index(LoginModel ln)
        {
        

            if (ln.Usr==""||ln.Usr==null)
            {
                return View();
            }
            if (ln.Pd == "" || ln.Pd == null)
            {
                return View();
            }
            if (DBOperations.GetLogin(ln.Usr, ln.Pd))
            {
                return new  RedirectResult("/swagger/ui/index");
            }
            else return View();
        }

        void ValidateRequestHeader(HttpRequestMessage request)
        {
            string cookieToken = "";
            string formToken = "";

            IEnumerable<string> tokenHeaders;
            if (request.Headers.TryGetValues("RequestVerificationToken", out tokenHeaders))
            {
                string[] tokens = tokenHeaders.First().Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }



    }
}