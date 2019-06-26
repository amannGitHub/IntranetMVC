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
    public class VideoRequestsController : Controller
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
                var myList = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spGetVideoRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /VideoRequests/Index
        [HttpPost]
        public ActionResult Index(VideoRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Video":
                        return RedirectToAction("Create", "VideoRequests");
                    case "Manage Video Requests":
                        return RedirectToAction("ManageIndex", "VideoRequests");
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
        // GET: /VideoRequests/Create
        public ActionResult Create()
        {
            try
            {
                VideoRequests model = db.VideoRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

                PopulateVideoFileTypes();
                PopulateVideoTypes();
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
        // POST: /VideoRequests/Create
        [HttpPost]
        public ActionResult Create(VideoRequests model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.StatusDate = DateTime.Now;
                model.VideoRequestStatusID = 1; //Submitted

                if (ModelState.IsValid)
                {
                    db.VideoRequest.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    Emails.VideoRequestSubmitted(model.VideoRequestID, model.VideoDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateVideoFileTypes();
                    PopulateVideoTypes();
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
        // GET: /VideoRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                VideoRequests model = db.VideoRequest.Where(x => x.VideoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.VideoRequestStatusID);
                PopulateVideoFileTypes(model.VideoFileTypeID);
                PopulateVideoTypes(model.VideoTypeID);
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
        // POST: /VideoRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(VideoRequests model, string Buttons)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (Buttons)
                    {
                        case "Approve":
                            model.RequestorApproved = true;
                            break;
                            //case "Save":
                            //    model.RequestorApproved = model.RequestorApproved;
                            //    break;
                    }
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    Emails.VideoRequestEdited(model.VideoRequestID, model.VideoDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateRequestStatus(model.VideoRequestStatusID);
                    PopulateVideoFileTypes(model.VideoFileTypeID);
                    PopulateVideoTypes(model.VideoTypeID);
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
        // GET: /VideoRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                VideoRequests model = db.VideoRequest.Where(x => x.VideoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.VideoRequestStatusID);
                PopulateVideoFileTypes(model.VideoFileTypeID);
                PopulateVideoTypes(model.VideoTypeID);
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
        // GET: /VideoRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                VideoRequests model = db.VideoRequest.Where(x => x.VideoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.VideoRequestStatusID);
                PopulateVideoFileTypes(model.VideoFileTypeID);
                PopulateVideoTypes(model.VideoTypeID);
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
        // POST: /VideoRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(VideoRequests model)
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

        private string DeleteRequest(VideoRequests model)
        {
            string sMessage = "";
            try
            {
                #region Prep Deletion Email

                //this must be done before it's deleted or there's no way to craft the email, but sending should occur after

                Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                VideoRequestsViewModel sRequest = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spGetVideoRequest] {0}", model.VideoRequestID).First();

                string Subject = "Deleted SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                    + " - " + sRequest.VideoRequestID;

                string MessageToTechs = "A Video Request for " + sEmployeeName + " has been deleted.<br><br>Ticket Number: " + sRequest.VideoRequestID 
                    + "<br>Request Type: " + sRequest.VideoType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Description: " + sRequest.VideoDescription;

                #endregion

                #region Delete

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                #endregion

                Emails.VideoRequestDeleted(MessageToTechs, Subject, EmployeeEmail);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        #region Manage Requests
        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\Video, SCGCORP\\Developers")]
        public ActionResult ManageIndex()
        {
            try
            {
                var myList = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spManageVideoRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /VideoRequests/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\Video, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                VideoRequests model = db.VideoRequest.Where(x => x.VideoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.VideoRequestStatusID);
                PopulateVideoFileTypes(model.VideoFileTypeID);
                PopulateVideoTypes(model.VideoTypeID);
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
        // POST: /VideoRequests/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Video, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(VideoRequests model, string Buttons)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (Buttons)
                    {
                        case "Update":
                                model.VideoRequestStatusID = 2;
                                model.StatusDate = DateTime.Now;
                            Emails.VideoRequestInProgress(model.VideoRequestID, model.VideoDescription, model.emp_id);
                            break;
                        case "Mark Complete":
                            model.VideoRequestStatusID = 3;
                            model.StatusDate = DateTime.Now;
                            Emails.VideoRequestCompleted(model.VideoRequestID, model.VideoDescription, model.emp_id);
                            break;
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Record updated.";
                    return RedirectToAction("ManageIndex");
                }
                else
                {
                    PopulateRequestStatus(model.VideoRequestStatusID);
                    PopulateVideoFileTypes(model.VideoFileTypeID);
                    PopulateVideoTypes(model.VideoTypeID);
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
        // GET: /VideoRequests/ManageDelete
        [Authorize(Roles = "SCGCORP\\Video, SCGCORP\\Developers")]
        public ActionResult ManageDelete(int id)
        {
            try
            {
                VideoRequests model = db.VideoRequest.Where(x => x.VideoRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.VideoRequestStatusID);
                PopulateVideoFileTypes(model.VideoFileTypeID);
                PopulateVideoTypes(model.VideoTypeID);
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
        // POST: /VideoRequests/ManageDelete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Video, SCGCORP\\Developers")]
        public ActionResult ManageDelete(VideoRequests model)
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

        private void PopulateVideoTypes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<VideoTypes>("EXEC [DBO].[spGetVideoTypes]").ToList();
                ViewBag.VideoTypeID = new SelectList(myList, "VideoTypeID", "VideoType", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVideoTypes");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateVideoFileTypes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<VideoFileTypes>("EXEC [DBO].[spGetVideoFileType]").ToList();
                ViewBag.VideoFileTypeID = new SelectList(myList, "VideoFileTypeID", "VideoFileType", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVideoFileTypes");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateRequestStatus(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<VideoRequestStatuses>("EXEC [DBO].[spGetVideoRequestStatuses]").ToList();
                ViewBag.VideoRequestStatusID = new SelectList(myList, "VideoRequestStatusID", "VideoRequestStatus", selected);
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