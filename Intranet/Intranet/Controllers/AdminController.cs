using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.BLL;

namespace Intranet.Controllers
{
    public class AdminController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //TODO: remove test roles (after commas)
        public ActionResult Index()
        {
            return View();
        }
    }
}