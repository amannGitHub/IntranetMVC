using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.ViewModels;
using Intranet.Models;
using Intranet.BLL;
using System.Data.Entity;
using System.IO;
using System.Web.Routing;

namespace Intranet.Controllers
{
    public class EditingRequestsController : Controller
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
                var myList = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditorRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditingRequests/Index
        [HttpPost]
        public ActionResult Index(EditingRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Editing":
                        return RedirectToAction("Create", "EditingRequests");
                    case "Manage Editing Requests":
                        return RedirectToAction("ManageIndex", "EditingRequests");
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
        // GET: /EditorRequests/Create
        public ActionResult Create()
        {
            try
            {
                EditingRequests model = db.EditingRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

                PopulateAlternateContacts();
                PopulateProjects();
                PopulateServicesRequested();
                PopulateStyleGuides();
                PopulateQAStaff();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/Create
        [HttpPost]
        public ActionResult Create(EditingRequests model, IEnumerable<HttpPostedFileBase> newFiles)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                model.StatusDate = DateTime.Now;
                model.CDRNApproved = false;
                model.EditingRequestStatusID = 1; //Submitted

                if (ModelState.IsValid)
                {
                    #region GetFiles

                    var myFiles = new List<Models.File>();

                    foreach (var file in newFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var newFile = new Models.File
                            {
                                FileName = Path.GetFileName(file.FileName),
                                FileType = FileTypes.EditingRequestFileType,
                                ContentType = file.ContentType
                            };

                            using (var reader = new BinaryReader(file.InputStream))
                            {
                                newFile.ContentFile = reader.ReadBytes(file.ContentLength);
                            }

                            myFiles.Add(newFile);
                        }
                    }

                    #endregion

                    model.Files = new List<Models.File>(myFiles);

                    db.EditingRequest.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";
                    Emails.EditingRequestSubmitted(model.EditingRequestID, model.RequestDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateAlternateContacts();
                    PopulateProjects();
                    PopulateServicesRequested();
                    PopulateStyleGuides();
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
        // GET: /EditorRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                EditingRequests model = db.EditingRequest.Include(x => x.Files).FirstOrDefault(x => x.EditingRequestID == id);
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateAllLists(model);

                //this is for the associated Drafts
                var Drafts = db.Database.SqlQuery<DraftsViewModel>("EXEC [DBO].[spGetDrafts] {0}", id).ToList();
                ViewData["Drafts"] = Drafts;

                #region find out if there's a final draft that has been completed

                string FinalDraftFound = "No";
                foreach (var item in Drafts)
                {
                    if (item.FinalDraft && item.EditorialReviewCompleted != null)
                    {
                        FinalDraftFound = "Yes";
                    }
                }
                ViewBag.FinalDraft = FinalDraftFound;

                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(EditingRequests model, string Buttons, IEnumerable<HttpPostedFileBase> newFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region GetFiles

                    var myFiles = new List<Models.File>();

                    foreach (var file in newFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var newFile = new Models.File
                            {
                                FileName = Path.GetFileName(file.FileName),
                                FileType = FileTypes.EditingRequestFileType,
                                ContentType = file.ContentType,
                                EditingRequestID = model.EditingRequestID,
                                WebRequestID = 0,
                                GraphicsRequestID = 0
                            };

                            using (var reader = new BinaryReader(file.InputStream))
                            {
                                newFile.ContentFile = reader.ReadBytes(file.ContentLength);
                            }

                            db.File.Add(newFile);
                            db.SaveChanges();
                        }
                    }

                    #endregion

                    if (Buttons == "Approve")
                    {
                        model.SubmitterApproved = true;
                    }
                    else
                    {
                        model.SubmitterApproved = false;
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    Emails.SubmitterReviewCompleted(model.EditingRequestID);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateAllLists(model);
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
        // GET: /EditorRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                //EditingRequests model = db.EditingRequest.Where(x => x.EditingRequestID == id).FirstOrDefault();
                EditingRequests model = db.EditingRequest.Include(x => x.Files).FirstOrDefault(x => x.EditingRequestID == id);

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateAllLists(model);

                //this is for the associated Drafts
                var Drafts = db.Database.SqlQuery<DraftsViewModel>("EXEC [DBO].[spGetDrafts] {0}", id).ToList();
                ViewData["Drafts"] = Drafts;

                #region find out if there's a final draft that has been completed

                string FinalDraftFound = "No";
                foreach (var item in Drafts)
                {
                    if (item.FinalDraft && item.EditorialReviewCompleted != null)
                    {
                        FinalDraftFound = "Yes";
                    }
                }
                ViewBag.FinalDraft = FinalDraftFound;

                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        public ActionResult Download(int id)
        {
            try
            {
                var file = db.File.First(f => f.FileID == id);
                return File(file.ContentFile.ToArray(), "application/octet-stream", file.FileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EditorRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                EditingRequests model = db.EditingRequest.Where(x => x.EditingRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateAllLists(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(EditingRequests model)
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

        private string DeleteRequest(EditingRequests model)
        {
            string sMessage = "";
            try
            {
                #region Prep Deletion Email

                //this must be done before it's deleted or there's no way to craft the email, but sending should occur after

                Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", model.EditingRequestID).First();

                string Subject = "Deleted SCG Editing Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                    + " - " + sRequest.EditingRequestID;

                string MessageToTechs = "An Editing Request for " + sEmployeeName + " has been deleted.<br><br>Request #: " + sRequest.EditingRequestID 
                    + ".<br>Service Requested: " + sRequest.ServiceRequested 
                    + ".<br>Date: " + sRequest.CreatedDate.ToShortDateString();

                #endregion

                #region Delete

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;
                db.Configuration.ValidateOnSaveEnabled = false;

                //Delete the attachments, files, and request
                db.File.RemoveRange(db.File.Where(x => x.EditingRequestID == model.EditingRequestID));
                db.Draft.RemoveRange(db.Draft.Where(x => x.EditingRequestID == model.EditingRequestID));
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                #endregion

                //Send the Email
                Emails.EditingRequestDeleted(MessageToTechs, Subject, EmployeeEmail);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        public ActionResult Assignments()
        {
            try
            {
                var myList = db.Database.SqlQuery<AssignmentsViewModel>("EXEC [DBO].[spGetMyDrafts] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #region Manage Requests

        [Authorize(Roles = "SCGCORP\\Editing, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageIndex()
        {
            try
            {
                var myList = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spManageEditorRequests]").ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditingRequests/ManageIndex
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Editing, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageIndex(EditingRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "View My Assignments":
                        return RedirectToAction("Assignments", "EditingRequests");
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
        // GET: /EditorRequests/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\Editing, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                EditingRequests model = db.EditingRequest.Include(x => x.Files).FirstOrDefault(x => x.EditingRequestID == id);
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateAllLists(model);

                //this is for the associated Drafts
                var Drafts = db.Database.SqlQuery<DraftsViewModel>("EXEC [DBO].[spGetDrafts] {0}", id).ToList();
                ViewData["Drafts"] = Drafts;

                #region find out if there's a final draft that has been completed

                string FinalDraftFound = "No";
                foreach (var item in Drafts)
                {
                    if (item.FinalDraft && item.EditorialReviewCompleted != null)
                    {
                        FinalDraftFound = "Yes";
                    }
                }
                ViewBag.FinalDraft = FinalDraftFound;

                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Editing, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(EditingRequests model, string Buttons, IEnumerable<HttpPostedFileBase> newFiles)
        {
            try
            {
                switch (Buttons)
                {
                    case "Add Draft":
                        //return RedirectToAction("AddDraft", new { id = model.EditingRequestID });
                        return RedirectToAction("AddDraft", new RouteValueDictionary(
                            new { controller = "EditingRequests", action = "AddDraft", EditingRequestID = model.EditingRequestID }));

                    case "Update":
                        if (ModelState.IsValid)
                        {
                            #region GetFiles

                            var myFiles = new List<Models.File>();

                            foreach (var file in newFiles)
                            {
                                if (file != null && file.ContentLength > 0)
                                {
                                    var newFile = new Models.File
                                    {
                                        FileName = Path.GetFileName(file.FileName),
                                        FileType = FileTypes.EditingRequestFileType,
                                        ContentType = file.ContentType,
                                        EditingRequestID = model.EditingRequestID,
                                        WebRequestID = 0,
                                        GraphicsRequestID = 0
                                    };

                                    using (var reader = new BinaryReader(file.InputStream))
                                    {
                                        newFile.ContentFile = reader.ReadBytes(file.ContentLength);
                                    }

                                    db.File.Add(newFile);
                                    db.SaveChanges();
                                }
                            }

                            #endregion

                            db.Entry(model).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["Message"] = "Record updated.";
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateAllLists(model);
                        }
                        break;

                    case "QA/QC Review Complete":
                        if (ModelState.IsValid)
                        {
                            model.QAStatusID = 4;   //Completed. Set it in case the user didn't. Same for next code
                            model.EditingRequestStatusID = 3;
                            model.StatusDate = DateTime.Now;
                            if (model.QADateCompleted.HasValue == false)
                            {
                                model.QADateCompleted = DateTime.Now;
                            }

                            db.Entry(model).State = EntityState.Modified;
                            db.SaveChanges();

                            Emails.QAQCReviewCompleted(model.EditingRequestID);

                            TempData["Message"] = "Record updated.";
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateAllLists(model);
                        }
                        break;
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
        // GET: /EditorRequests/ManageDelete
        [Authorize(Roles = "SCGCORP\\Editing, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageDelete(int id)
        {
            try
            {
                EditingRequests model = db.EditingRequest.Where(x => x.EditingRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateAllLists(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditingRequests/ManageDelete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Editing, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageDelete(EditingRequests model)
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

        #region Drafts

        //
        // GET: /EditorRequests/AddDraft
        public ActionResult AddDraft(int EditingRequestID)
        {
            try
            {
                Drafts model = db.Draft.Create();
                model.EditingRequestID = EditingRequestID;

                PopulateEditorStaff();
                PopulateEditorialReviewers();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/AddDraft
        [HttpPost]
        public ActionResult AddDraft(Drafts model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Draft.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your draft has been recorded.";

                    Emails.EditingRequestAssigned(model.EditingRequestID, model.EditorID, model.EditingRequestID);

                    //Mark the parent request "In Progress" if it isn't already
                    EditingRequests mEditingRequest = db.EditingRequest.Where(x => x.EditingRequestID == model.EditingRequestID).FirstOrDefault();
                    if (mEditingRequest.EditingRequestStatusID == 1)
                    {
                        mEditingRequest.EditingRequestStatusID = 2;
                        mEditingRequest.StatusDate = DateTime.Now;
                        db.Entry(mEditingRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("ManageRequest", new RouteValueDictionary(
                        new { controller = "EditingRequests", action = "ManageRequest", id = model.EditingRequestID, Manage = "Yes" }));
                }
                else
                {
                    PopulateEditorStaff();
                    PopulateEditorialReviewers();
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
        // GET: /EditorRequests/EditDraft/5
        public ActionResult EditDraft(int id)
        {
            try
            {
                Drafts model = db.Draft.Where(x => x.DraftID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                #region Determine which buttons to show

                //Defaults
                ViewBag.IsEditor = "No";
                ViewBag.IsEditorReviewer = "No";

                Employees mEmployee = db.Employee.Where(a => a.network_id == userName).FirstOrDefault(); //using UserName, get emp_id of logged in user

                //if user's emp_id is the EditorID, they're the editor. Show that button
                if (mEmployee.emp_id == model.EditorID)
                {
                    ViewBag.IsEditor = "Yes";
                }

                //if user's emp_id is the EditorID, they're the editor. Show that button
                if (mEmployee.emp_id == model.EditorialReviewerID)
                {
                    ViewBag.IsEditorReviewer = "Yes";
                }

                #endregion

                PopulateEditorStaff(model.EditorID);
                PopulateEditorialReviewers(model.EditorialReviewerID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/EditDraft/5
        [HttpPost]
        public ActionResult EditDraft(Drafts model, string Buttons)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (Buttons)
                    {
                        case "Editing Complete":
                            if ((model.EditorHours == 0) || model.DraftCompletedDate.HasValue == false)
                            {
                                TempData["ErrorMessage"] = "Please indicate Editor Hours and Draft Completed date.";
                            }
                            else
                            {
                                model.EditingCompleted = true;
                                db.Entry(model).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Message"] = "Editing marked complete.";
                                
                                Emails.DraftEdited(model.EditingRequestID);

                                return RedirectToAction("ManageRequest", new RouteValueDictionary(
                                    new { controller = "EditingRequests", action = "ManageRequest", id = model.EditingRequestID, Manage = "Yes" }));

                            }

                            PopulateEditorStaff(model.EditorID);
                            PopulateEditorialReviewers(model.EditorialReviewerID);


                            break;

                        case "Review Complete":

                            if ((model.EditorialHours == 0) || model.EditorialReviewCompleted.HasValue == false)
                            {
                                TempData["ErrorMessage"] = "Please indicate Reviewer Hours and Review Completed date.";
                            }
                            else
                            {
                                model.ReviewCompleted = true;
                                db.Entry(model).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Message"] = "Review marked complete.";

                                Emails.DraftReviewCompleted(model.EditingRequestID, model.DraftID);

                                return RedirectToAction("ManageRequest", new RouteValueDictionary(
                                    new { controller = "EditingRequests", action = "ManageRequest", id = model.EditingRequestID, Manage = "Yes" }));

                            }

                            PopulateEditorStaff(model.EditorID);
                            PopulateEditorialReviewers(model.EditorialReviewerID);

                            break;

                        case "Save":
                            db.Entry(model).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["Message"] = "Record saved.";
                            return RedirectToAction("ManageRequest", new RouteValueDictionary(
                                new { controller = "EditingRequests", action = "ManageRequest", id = model.EditingRequestID, Manage = "Yes" }));
                    }
                }
                else
                {
                    PopulateEditorStaff(model.EditorID);
                    PopulateEditorialReviewers(model.EditorialReviewerID);
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
        // GET: /EditorRequests/DraftDetails
        public ActionResult DraftDetails(int id)
        {
            try
            {
                Drafts model = db.Draft.Where(x => x.DraftID == id).FirstOrDefault();

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEditorStaff(model.EditorID);
                PopulateEditorialReviewers(model.EditorialReviewerID);


                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EditorRequests/DeleteDraft
        public ActionResult DraftDelete(int id)
        {
            try
            {
                Drafts model = db.Draft.Where(x => x.DraftID == id).FirstOrDefault();

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEditorStaff(model.EditorID);
                PopulateEditorialReviewers(model.EditorialReviewerID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EditorRequests/DeleteDraft/5
        [HttpPost]
        public ActionResult DraftDelete(Drafts model)
        {
            try
            {
                var EditingRequestID = model.EditingRequestID;

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                TempData["Message"] = "Draft deleted.";
                return RedirectToAction("ManageRequest", new RouteValueDictionary(
                        new { controller = "EditingRequests", action = "ManageRequest", id = EditingRequestID, Manage = "Yes" }));

            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

        #region Populate DDLs

        public void PopulateAllLists(EditingRequests model)
        {
            PopulateAlternateContacts(model.AlternateContactID);
            PopulateProjects(model.project_id);
            PopulateServicesRequested(model.ServiceRequestedID);
            PopulateStyleGuides(model.StyleGuideID);
            PopulateRequestStatus(model.EditingRequestStatusID);
            //PopulateFinalReviewers(model.FinalReviewerID);
            PopulateQAStaff(model.QAReviewerID);
            PopulateQAStatus(model.QAStatusID);
        }

        private void PopulateProjects(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<ProjectCodesViewModel>("EXEC [DBO].[spGetProjectCodes]").ToList();
                ViewBag.project_id = new SelectList(myList, "project_id", "ProjectCode", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateProjects");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateAlternateContacts(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<AlternateContacts>("EXEC [DBO].[spGetAlternateContacts]").ToList();
                ViewBag.AlternateContactID = new SelectList(myList, "AlternateContactID", "AlternateContactName", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateStyleGuides(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<StyleGuides>("EXEC [DBO].[spGetStyleGuides]").ToList();
                ViewBag.StyleGuideID = new SelectList(myList, "StyleGuideID", "StyleGuide", selected);
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
                var myList = db.Database.SqlQuery<EditingRequestStatuses>("EXEC [DBO].[spGetEditingRequestStatuses]").ToList();
                ViewBag.EditingRequestStatusID = new SelectList(myList, "EditingRequestStatusID", "EditingRequestStatus", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateRequestStatus");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateServicesRequested(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<ServicesRequested>("EXEC [DBO].[spGetServicesRequested]").ToList();
                ViewBag.ServiceRequestedID = new SelectList(myList, "ServiceRequestedID", "ServiceRequested", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateRequestStatus");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEditorStaff(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Editors>("EXEC [DBO].[spGetEditors]").ToList();
                ViewBag.EditorID = new SelectList(myList, "EditorID", "Editor", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEditorStaff");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEditorialReviewers(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<EditorialReviewers>("EXEC [DBO].[spGetEditorialReviewers]").ToList();
                ViewBag.EditorialReviewerID = new SelectList(myList, "EditorialReviewerID", "EditorialReviewer", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEditorStaff");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateQAStaff(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<QAStaff>("EXEC [DBO].[spGetQAStaff]").ToList();
                ViewBag.QAReviewerID = new SelectList(myList, "QAReviewerID", "QAReviewer", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEditorStaff");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateQAStatus(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<QAStatuses>("EXEC [DBO].[spGetQAStatuses]").ToList();
                ViewBag.QAStatusID = new SelectList(myList, "QAStatusID", "QAStatus", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateRequestStatus");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateFinalReviewers(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<FinalReviewers>("EXEC [DBO].[spGetFinalReviewers]").ToList();
                ViewBag.FinalReviewerID = new SelectList(myList, "FinalReviewerID", "FinalReviewer", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEditorStaff");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

    }
}