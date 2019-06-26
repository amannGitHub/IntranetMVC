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
    public class DivisionsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<Divisions>("EXEC [DBO].[spGetDivisions] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Divisions/Index
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Index(Divisions model)
        {
            try
            {
                return RedirectToAction("Create", "Divisions");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /Divisions/Create
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
        // POST: /Divisions/Create
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create(Divisions model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Division.Add(model);
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
        // GET: /Divisions/Edit/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Edit(int id)
        {
            try
            {
                Divisions model = db.Division.Where(x => x.DivisionID == id).FirstOrDefault();
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
        // POST: /Divisions/Edit/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Edit(Divisions model)
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