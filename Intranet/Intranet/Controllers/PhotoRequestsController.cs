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
    //Shelfing/benching/back-burnering this system for now, notes on work for it below:
    //I have made the SP's for spGetPhotoRequests, spGetPhotoRequest, spGetPhotoRequestStatuses, spManagePhotoRequests
    //Details, Edit, Delete, ManageIndex, and ManageRequest all need updating
    //Create page displays but has an issue where the ModelState.IsValid is false but should be true, this needs to be fixed (emp_id is shown as
        //null for some reason and is the cause of the invalid state)
    //I'm not sure if all the dropdown functions have been created, but I know there are at least 3 currently.

    public class PhotoRequestsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<PhotoRequestsViewModel>("EXEC [DBO].[spGetPhotoRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /PhotoRequests/Index
        [HttpPost]
        public ActionResult Index(PhotoRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Create Photo Request":
                        return RedirectToAction("Create", "PhotoRequests");
                    case "Manage Photo Requests":
                        return RedirectToAction("ManageIndex", "PhotoRequests");
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
        // GET: /PhotoRequests/Create
        public ActionResult Create()
        {
            try
            {
                GraphicsRequests model = db.GraphicRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code
                PopulateClients();
                PopulateDivisions();
                PopulateProjectCodes();
                //ToDo: populate all drop down lists for photo requests
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /PhotoRequests/Create
        //IsValid is false for some reason, need to fix
        [HttpPost]
        public ActionResult Create(PhotoRequests model)
        {
            try
            {
                model.RequestDate = DateTime.Now;
                model.PhotoRequestStatusID = 1;
                if (ModelState.IsValid)
                {
                    db.PhotoRequests.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "The photo request has been created.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateClients(model.ClientID);
                    PopulateDivisions(model.DivisionID);
                    PopulateProjectCodes(model.project_id);
                    //ToDo: add more dropdowns
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
        // GET: /PhotoRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                PhotoRequests model = db.PhotoRequests.Where(x => x.PhotoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }
                PopulateClients();
                PopulateDivisions();
                PopulateProjectCodes();
                //ToDo: Populate all dropdowns for photo requests
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /PhotoRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(PhotoRequests model)
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
                    PopulateClients();
                    PopulateDivisions();
                    PopulateProjectCodes();
                    //ToDo: Populate all drop downs for photo requests
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
        // GET: /PhotoRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                PhotoRequests model = db.PhotoRequests.Where(x => x.PhotoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }
                PopulateClients(model.ClientID);
                PopulateDivisions(model.DivisionID);
                PopulateProjectCodes(model.project_id);
                //ToDo: populate drop down lists for photo requests
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /PhotoRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                PhotoRequests model = db.PhotoRequests.Where(x => x.PhotoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }
                PopulateClients(model.ClientID);
                PopulateDivisions(model.DivisionID);
                PopulateProjectCodes(model.project_id);
                //ToDo: Populate all dropdowns for photo requests
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /PhotoRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(PhotoRequests model)
        {
            try
            {
                TempData["Message"] = DeletePhotoRequest(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        private string DeletePhotoRequest(PhotoRequests model)
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

        private void PopulateProjectCodes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<ProjectCodesViewModel>("EXEC [DBO].[spGetProjectCodes]").ToList();
                ViewBag.project_id = new SelectList(myList, "project_id", "ProjectCode", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateProjectCodes");
                RedirectToAction("ViewMessage", "Home");
            }
        }
        //ToDo: Add more methods for each dropdown added (need Divisions, Clients, and Projects)

        #endregion

    }
}