using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginSwagger : Controller
    {
        public ActionResult Signin()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}