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

namespace Intranet.Controllers
{
    public class WebRequestsController : Controller
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
                var myList = db.Database.SqlQuery<WebRequestsViewModel>("EXEC [DBO].[spGetWebRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /WebRequests/Index
        [HttpPost]
        public ActionResult Index(WebRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Create Request":
                        return RedirectToAction("Create", "WebRequests");
                    case "Manage Web Requests":
                        return RedirectToAction("ManageIndex", "WebRequests");
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
        // GET: /WebRequests/Create
        public ActionResult Create()
        {
            try
            {
                WebRequests model = db.WebRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

                PopulateWorkTypes();
                PopulateProjectCodes();
                PopulateWebsites();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /WebRequests/Create
        [HttpPost]
        public ActionResult Create(WebRequests model, IEnumerable<HttpPostedFileBase> newFiles)
        {
            try
            {
                model.RequestDate = DateTime.Now;
                model.WebRequestStatusID = 1;

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
                                FileType = FileTypes.WebRequestFileType,
                                ContentType = file.ContentType,
                                WebRequestID = model.WebRequestID,
                                EditingRequestID = 0,
                                GraphicsRequestID = 0
                            };

                            using (var reader = new BinaryReader(file.InputStream))
                            {
                                newFile.ContentFile = reader.ReadBytes(file.ContentLength);
                            }

                            myFiles.Add(newFile);

                            //file.SaveAs(Path.Combine(Server.MapPath("/Files"), Guid.NewGuid() + Path.GetExtension(file.FileName)));
                        }
                    }

                    #endregion

                    model.Files = new List<Models.File>(myFiles);

                    db.WebRequest.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your leave request has been submitted.";

                    Emails.WebRequestSubmitted(model.WebRequestID, model.Comments, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateWorkTypes();
                    PopulateProjectCodes();
                    PopulateWebsites();
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
        // GET: /WebRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                //WebRequests model = db.WebRequest.Where(x => x.WebRequestID == id).FirstOrDefault();
                WebRequests model = db.WebRequest.Include(x => x.Files).FirstOrDefault(x => x.WebRequestID == id);

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateWorkTypes(model.WorkTypeID);
                PopulateProjectCodes(model.project_id);
                PopulateWebsites(model.website_id);
                PopulateDevelopers(model.AssigneeID);
                PopulateWebRequestStatuses(model.WebRequestStatusID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /WebRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(WebRequests model, IEnumerable<HttpPostedFileBase> newFiles, string Buttons)
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
                    #region GetFiles

                    var myFiles = new List<Models.File>();

                    foreach (var file in newFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var newFile = new Models.File
                            {
                                FileName = Path.GetFileName(file.FileName),
                                FileType = FileTypes.WebRequestFileType,
                                ContentType = file.ContentType,
                                WebRequestID = model.WebRequestID,
                                EditingRequestID = 0,
                                GraphicsRequestID = 0
                            };

                            using (var reader = new BinaryReader(file.InputStream))
                            {
                                newFile.ContentFile = reader.ReadBytes(file.ContentLength);
                            }

                            myFiles.Add(newFile);

                            //file.SaveAs(Path.Combine(Server.MapPath("/Files"), Guid.NewGuid() + Path.GetExtension(file.FileName)));
                        }
                    }

                    #endregion

                    model.Files = new List<Models.File>(myFiles);

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    //ToDo could add an email that it was updated but am skipping for now

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateWorkTypes(model.WorkTypeID);
                    PopulateProjectCodes(model.project_id);
                    PopulateWebsites(model.website_id);
                    PopulateDevelopers(model.AssigneeID);
                    PopulateWebRequestStatuses(model.WebRequestStatusID);
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
        // GET: /WebRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                //WebRequests model = db.WebRequest.Where(x => x.WebRequestID == id).FirstOrDefault();
                WebRequests model = db.WebRequest.Include(x => x.Files).FirstOrDefault(x => x.WebRequestID == id);
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateWorkTypes(model.WorkTypeID);
                PopulateProjectCodes(model.project_id);
                PopulateWebsites(model.website_id);
                PopulateWebRequestStatuses(model.WebRequestStatusID);
                PopulateDevelopers(model.AssigneeID);

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
        // GET: /WebRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                WebRequests model = db.WebRequest.Where(x => x.WebRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateWorkTypes(model.WorkTypeID);
                PopulateProjectCodes(model.project_id);
                PopulateWebsites(model.website_id);
                PopulateWebRequestStatuses(model.WebRequestStatusID);
                PopulateDevelopers(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /WebRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(WebRequests model)
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

        private string DeleteRequest(WebRequests model)
        {
            string sMessage = "";
            try
            {
                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.File.RemoveRange(db.File.Where(x => x.WebRequestID == model.WebRequestID));
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                //Emails.DeleteLeaveRequest(model.emp_id, model.leave_start_dt.ToShortDateString() + " " + model.leave_start_dt.ToShortTimeString(),
                //    model.leave_end_dt.ToShortDateString() + " " + model.leave_end_dt.ToShortTimeString(),
                //    model.totalhours, model.leavetype_id, model.leave_reason, model.leaverequest_id);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        #region Manage Requests

        [Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageIndex()
        {
            try
            {
                var myList = db.Database.SqlQuery<WebRequestsViewModel>("EXEC [DBO].[spManageWebRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /WebRequests/ManagerDelete
        [Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManagerDelete(int id)
        {
            try
            {
                WebRequests model = db.WebRequest.Where(x => x.WebRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateWorkTypes(model.WorkTypeID);
                PopulateProjectCodes(model.project_id);
                PopulateWebsites(model.website_id);
                PopulateDevelopers(model.AssigneeID);
                PopulateWebRequestStatuses(model.WebRequestStatusID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /WebRequests/ManagerDelete/5
        [Authorize(Roles = "SCGCORP\\Developers")]
        [HttpPost]
        public ActionResult ManagerDelete(WebRequests model)
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

        //
        // GET: /WebRequests/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                //WebRequests model = db.WebRequest.Where(x => x.WebRequestID == id).FirstOrDefault();
                WebRequests model = db.WebRequest.Include(x => x.Files).FirstOrDefault(x => x.WebRequestID == id);
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateWorkTypes(model.WorkTypeID);
                PopulateProjectCodes(model.project_id);
                PopulateWebsites(model.website_id);
                PopulateDevelopers(model.AssigneeID);
                PopulateWebRequestStatuses(model.WebRequestStatusID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }


        //POST: /WebRequests/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(WebRequests model, string Buttons, IEnumerable<HttpPostedFileBase> newFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (Buttons)
                    {
                        case "Update":
                            if (model.AssigneeID != null)
                            {
                                //if there's a developer assigned, set it to "In Progress"
                                model.WebRequestStatusID = 2;
                                model.AssignedDate = DateTime.Now;
                                Emails.WebRequestInProgress(model.WebRequestID, model.Comments, model.emp_id, model.AssigneeID);
                            }
                            break;
                        case "Mark Complete":
                            model.WebRequestStatusID = 3;
                            model.CompletedDate = DateTime.Now;
                            break;
                        case "Delete":
                            return RedirectToAction("ManagerDelete", new { id = model.WebRequestID });
                    }

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
                                WebRequestID = model.WebRequestID,
                                EditingRequestID = 0,
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

                    switch (Buttons)
                    {
                        case "Mark Complete":
                            Emails.WebRequestCompleted(model.WebRequestID, model.Comments, model.emp_id);
                            break;
                    }

                    TempData["Message"] = "Record updated.";
                    return RedirectToAction("ManageIndex");
                }
                else
                {
                    model = db.WebRequest.Include(x => x.Files).FirstOrDefault(x => x.WebRequestID == model.WebRequestID);

                    PopulateWorkTypes(model.WorkTypeID);
                    PopulateProjectCodes(model.project_id);
                    PopulateWebsites(model.website_id);
                    PopulateDevelopers(model.AssigneeID);
                    PopulateWebRequestStatuses(model.WebRequestStatusID);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }

            return View(model);
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

        private void PopulateWorkTypes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<WorkTypes>("EXEC [DBO].[spGetWorkTypes]").ToList();
                ViewBag.WorkTypeID = new SelectList(myList, "WorkTypeID", "WorkType", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateWebsites(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<WebsiteNames>("EXEC [DBO].[spGetWebsiteNames]").ToList();
                ViewBag.website_id = new SelectList(myList, "website_id", "websitename", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateWebRequestStatuses(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<WebRequestsStatuses>("EXEC [DBO].[spGetWebRequestStatuses]").ToList();
                ViewBag.WebRequestStatusID = new SelectList(myList, "WebRequestStatusID", "WebRequestStatus", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateDevelopers(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<RequestAssignee>("EXEC [DBO].[spGetWebEmployees]").ToList();
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