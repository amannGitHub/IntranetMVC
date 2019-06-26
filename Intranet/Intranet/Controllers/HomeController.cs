using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.BLL;

namespace Intranet.Controllers
{
    public class HomeController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Home/Index
        [HttpPost]
        public ActionResult Index(string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Leave Requests":
                        return RedirectToAction("Index", "LeaveRequests");
                    case "IT Support":
                        return RedirectToAction("Index", "ITRequests");
                    case "Visit HR":
                        return RedirectToAction("Index", "HR");
                    case "Graphics Requests":
                        return RedirectToAction("Index", "GraphicsRequests");
                    case "Web Requests":
                        return RedirectToAction("Index", "WebRequests");
                    case "Video Requests":
                        return RedirectToAction("Index", "VideoRequests");
                    case "Editing Requests":
                        return RedirectToAction("Index", "EditingRequests");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Staff()
        {
            ViewBag.Message = "Staff.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult NoAccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ViewMessage()
        {
            return View();
        }
    }
}