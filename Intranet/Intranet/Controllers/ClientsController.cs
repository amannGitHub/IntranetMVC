using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.ViewModels;
using Intranet.Models;
using Intranet.BLL;
using System.Data.Entity;

namespace Intranet.Controllers
{
    public class ClientsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<Clients>("EXEC [DBO].[spGetClients] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Clients/Index
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Index(Clients model)
        {
            try
            {
                return RedirectToAction("Create", "Clients");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /Clients/Create
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Clients/Create
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create(Clients model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Client.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }

            return View(model);
        }

        //
        // GET: /Clients/Edit/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Edit(int id)
        {
            try
            {
                Clients model = db.Client.Where(x => x.ClientID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Clients/Edit/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Edit(Clients model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }

            return View(model);
        }
    }
}