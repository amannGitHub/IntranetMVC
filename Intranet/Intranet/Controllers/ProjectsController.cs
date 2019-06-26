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
    public class ProjectsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<ProjectsViewModel>("EXEC [DBO].[spGetProjects]").ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Projects/Index
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult Index(Projects model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Create Project":
                        return RedirectToAction("Create", "Projects");
                    case "Manage Projects":
                        return RedirectToAction("ManageIndex", "Projects");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /Projects/Create
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create()
        {
            try
            {
                PopulateClients();
                PopulateDivisions();

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Projects/Create
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create(Projects model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Project.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "The project has been created.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateClients(model.ClientID);
                    PopulateDivisions(model.DivisionID);
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
        // GET: /Projects/Edit/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult Edit(int id)
        {
            try
            {
                Projects model = db.Project.Where(x => x.project_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateClients(model.ClientID);
                PopulateDivisions(model.DivisionID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Projects/Edit/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult Edit(Projects model)
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
                else
                {
                    PopulateClients(model.ClientID);
                    PopulateDivisions(model.DivisionID);
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
        // GET: /Projects/Details
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Details(int id)
        {
            try
            {
                Projects model = db.Project.Where(x => x.project_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateClients(model.ClientID);
                PopulateDivisions(model.DivisionID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /Projects/Delete
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Delete(int id)
        {
            try
            {
                Projects model = db.Project.Where(x => x.project_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateClients(model.ClientID);
                PopulateDivisions(model.DivisionID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Projects/Delete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Delete(Projects model)
        {
            try
            {
                TempData["Message"] = DeleteProject(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        private string DeleteProject(Projects model)
        {
            string sMessage = "";
            try
            {
                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        #region Populate DDLs

        private void PopulateClients(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Clients>("EXEC [DBO].[spGetClientsActive]").ToList();
                ViewBag.ClientID = new SelectList(myList, "ClientID", "Client", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateClients");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateDivisions(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Divisions>("EXEC [DBO].[spGetDivisionsActive]").ToList();
                ViewBag.DivisionID = new SelectList(myList, "DivisionID", "Division", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateDivisions");
                RedirectToAction("ViewMessage", "Home");
            }
        }


        #endregion

    }
}