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
    public class ITRequestsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spGetITRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /ITRequests/Index
        [HttpPost]
        public ActionResult Index(ITRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Support":
                        return RedirectToAction("Create", "ITRequests");
                    case "Manage IT Requests":
                        return RedirectToAction("ManageIndex", "ITRequests");
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
        // GET: /ITRequests/Create
        public ActionResult Create()
        {
            try
            {
                ITRequests model = db.ITRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

                PopulateRequestTypes();
                PopulateProjectCodes();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /ITRequests/Create
        [HttpPost]
        public ActionResult Create(ITRequests model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.StatusDate = DateTime.Now;
                model.ITRequestStatusID = 1; //Submitted

                if (ModelState.IsValid)
                {
                    db.ITRequest.Add(model); //adds model to ITRequest
                    db.SaveChanges(); //saves to DB
                    TempData["Message"] = "Your support request has been submitted.";

                    Emails.ITRequestSubmitted(model.ITRequestID, model.RequestDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateRequestTypes();
                    PopulateProjectCodes();
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
        // GET: /ITRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                ITRequests model = db.ITRequest.Where(x => x.ITRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestTypes(model.ITRequestTypeID);
                PopulateRequestStatus(model.ITRequestStatusID);
                PopulateProjectCodes(model.project_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /ITRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(ITRequests model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    Emails.ITRequestEdited(model.ITRequestID, model.RequestDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateRequestTypes(model.ITRequestTypeID);
                    PopulateRequestStatus(model.ITRequestStatusID);
                    PopulateProjectCodes(model.project_id);
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
        // GET: /ITRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                ITRequests model = db.ITRequest.Where(x => x.ITRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestTypes(model.ITRequestTypeID);
                PopulateRequestStatus(model.ITRequestStatusID);
                PopulateProjectCodes(model.project_id);
                PopulateTechStaff(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /ITRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                ITRequests model = db.ITRequest.Where(x => x.ITRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestTypes(model.ITRequestTypeID);
                PopulateRequestStatus(model.ITRequestStatusID);
                PopulateProjectCodes(model.project_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /ITRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(ITRequests model)
        {
            try
            {
                TempData["Message"] = DeleteRequest(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        private string DeleteRequest(ITRequests model)
        {
            string sMessage = "";
            try
            {
                #region Prep Deletion Email

                //this must be done before it's deleted or there's no way to craft the email, but sending should occur after

                Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                ITRequestsViewModel sRequest = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spGetITRequest] {0}", model.ITRequestID).First();

                string Subject = "Deleted SCG IT Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                    + " - " + sRequest.ProjectCode;

                string MessageToTechs = "An IT Support Request for " + sEmployeeName + " has been deleted.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Description: " + sRequest.RequestDescription;

                #endregion

                #region Delete

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                #endregion

                Emails.ITRequestDeleted(MessageToTechs, Subject, EmployeeEmail);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        #region Manage Requests

        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageIndex()
        {
            try
            {
                var myList = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spManageITRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /ITRequests/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                ITRequests model = db.ITRequest.Where(x => x.ITRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestTypes(model.ITRequestTypeID);
                PopulateRequestStatus(model.ITRequestStatusID);
                PopulateProjectCodes(model.project_id);
                PopulateTechStaff(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /ITRequests/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageRequest(ITRequests model, string Buttons)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (Buttons)
                    {
                        case "Update":
                            if (model.AssigneeID != 0)
                            {
                                model.ITRequestStatusID = 2;
                                model.AssignDate = DateTime.Now;
                                model.StatusDate = DateTime.Now;
                                Emails.AssignITRequest(model.ITRequestID, model.RequestDescription, model.emp_id, model.AssigneeID);
                            }
                            break;
                        case "Mark Complete":
                            model.ITRequestStatusID = 3;
                            model.StatusDate = DateTime.Now;
                            Emails.ITRequestCompleted(model.ITRequestID, model.RequestDescription, model.emp_id);
                            break;
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Record updated.";
                    return RedirectToAction("ManageIndex");
                }
                else
                {
                    PopulateRequestTypes(model.ITRequestTypeID);
                    PopulateRequestStatus(model.ITRequestStatusID);
                    PopulateProjectCodes(model.project_id);
                    PopulateTechStaff(model.AssigneeID);
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
        // GET: /ITRequests/ManageDelete
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageDelete(int id)
        {
            try
            {
                ITRequests model = db.ITRequest.Where(x => x.ITRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestTypes(model.ITRequestTypeID);
                PopulateRequestStatus(model.ITRequestStatusID);
                PopulateProjectCodes(model.project_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /ITRequests/ManageDelete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageDelete(ITRequests model)
        {
            try
            {
                TempData["Message"] = DeleteRequest(model);
                return RedirectToAction("ManageIndex");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

        #region Populate DDLs

        private void PopulateProjectCodes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<ProjectCodesViewModel>("EXEC [DBO].[spGetProjectCodes]").ToList();
                ViewBag.project_id = new SelectList(myList, "project_id", "ProjectCode", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateRequestTypes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<ITRequestTypes>("EXEC [DBO].[spGetITRequestTypes]").ToList();
                ViewBag.ITRequestTypeID = new SelectList(myList, "ITRequestTypeID", "ITRequestType", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateRequestStatus(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<ITRequestStatuses>("EXEC [DBO].[spGetITRequestStatuses]").ToList();
                ViewBag.ITRequestStatusID = new SelectList(myList, "ITRequestStatusID", "ITRequestStatus", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateTechStaff(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<RequestAssignee>("EXEC [DBO].[spGetTechEmployees]").ToList();
                ViewBag.AssigneeID = new SelectList(myList, "AssigneeID", "EmployeeName", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

    }
}