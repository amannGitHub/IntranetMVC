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
    public class GraphicsRequestsController : Controller
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
                var myList = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spGetGraphicsRequests] {0}", userName).ToList().OrderByDescending(s => s.RequestDate);
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Graphic/Index
        [HttpPost]
        public ActionResult Index(GraphicsRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Graphic":
                        return RedirectToAction("Create", "GraphicsRequests");
                    case "Manage Graphics Requests":
                        return RedirectToAction("ManageIndex", "GraphicsRequests");
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
        // GET: /GraphicRequests/Create
        public ActionResult Create()
        {
            try
            {
                GraphicsRequests model = db.GraphicRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

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
        // POST: /GraphicRequests/Create
        [HttpPost]
        public ActionResult Create(GraphicsRequests model, IEnumerable<HttpPostedFileBase> newFiles)
        {
            try
            {
                model.RequestDate = DateTime.Now;
                model.StatusDate = DateTime.Now;
                model.GraphicRequestStatusID = 1; //Submitted

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
                                FileType = FileTypes.GraphicsRequestFileType,
                                ContentType = file.ContentType,
                                GraphicsRequestID = model.GraphicsRequestID,
                                WebRequestID = 0,
                                EditingRequestID = 0
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

                    db.GraphicRequest.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your graphic request has been submitted.";

                    Emails.GraphicsRequestSubmitted(model.GraphicsRequestID, model.Instructions, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
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
        // GET: /GraphicRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                //GraphicsRequests model = db.GraphicRequest.Where(x => x.GraphicsRequestID == id).FirstOrDefault();
                GraphicsRequests model = db.GraphicRequest.Include(x => x.Files).FirstOrDefault(x => x.GraphicsRequestID == id);

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateGraphicsStaff(model.AssigneeID);
                PopulateRequestStatus(model.GraphicRequestStatusID);
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
        // POST: /GraphicRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(GraphicsRequests model, IEnumerable<HttpPostedFileBase> newFiles, string Buttons)
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
                                FileType = FileTypes.GraphicsRequestFileType,
                                ContentType = file.ContentType,
                                GraphicsRequestID = model.GraphicsRequestID,
                                WebRequestID = 0,
                                EditingRequestID = 0
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
                    model.StatusDate = DateTime.Now;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    Emails.GraphicsRequestEdited(model.GraphicsRequestID, model.Instructions, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateGraphicsStaff(model.AssigneeID);
                    PopulateRequestStatus(model.GraphicRequestStatusID);
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
        // GET: /GraphicRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                //GraphicsRequests model = db.GraphicRequest.Where(x => x.GraphicsRequestID == id).FirstOrDefault();
                GraphicsRequests model = db.GraphicRequest.Include(x => x.Files).FirstOrDefault(x => x.GraphicsRequestID == id);

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.GraphicRequestStatusID);
                PopulateProjectCodes(model.project_id);
                PopulateGraphicsStaff(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /GraphicRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                GraphicsRequests model = db.GraphicRequest.Where(x => x.GraphicsRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.GraphicRequestStatusID);
                PopulateProjectCodes(model.project_id);
                PopulateGraphicsStaff(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /GraphicRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(GraphicsRequests model)
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

        private string DeleteRequest(GraphicsRequests model)
        {
            string sMessage = "";
            try
            {
                #region Prep Deletion Email

                //this must be done before it's deleted or there's no way to craft the email, but sending should occur after

                Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                GraphicsRequestsViewModel sRequest = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spGetGraphicsRequest] {0}", model.GraphicsRequestID).First();

                //ProjectTitle might need to be changed to project_id
                string Subject = "Deleted SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                    + " - " + sRequest.ProjectTitle;

                string MessageToDesigners = "A Graphics Request for " + sEmployeeName + " has been deleted.<br><br>Project Title: " + sRequest.ProjectTitle 
                    + ".<br>Request Date: " + sRequest.RequestDate.ToShortDateString() 
                    + ".<br>Instructions: " + sRequest.Instructions;

                #endregion

                #region Delete

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.File.RemoveRange(db.File.Where(x => x.GraphicsRequestID == model.GraphicsRequestID));
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                #endregion

                Emails.GraphicsRequestDeleted(MessageToDesigners, Subject, EmployeeEmail);

                return sMessage = "Record Deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
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

        #region Manage Requests
        //TODO: remove test roles
        //
        // GET: /GraphicRequests/ManageDelete
        [Authorize(Roles = "SCGCORP\\GRAPHICS, SCGCORP\\Developers")]
        public ActionResult ManageDelete(int id)
        {
            try
            {
                GraphicsRequests model = db.GraphicRequest.Where(x => x.GraphicsRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.GraphicRequestStatusID);
                PopulateProjectCodes(model.project_id);
                PopulateGraphicsStaff(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /GraphicRequests/ManageDelete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\GRAPHICS, SCGCORP\\Developers")]
        public ActionResult ManageDelete(GraphicsRequests model)
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

        [Authorize(Roles = "SCGCORP\\GRAPHICS, SCGCORP\\Developers")]
        //[Authorize(Roles = "SCGCORP\\DEVELOPERS")]
        public ActionResult ManageIndex()
        {
            try
            {
                var myList = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spManageGraphicsRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /GraphicRequests/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\GRAPHICS, SCGCORP\\Developers")]
        //[Authorize(Roles = "SCGCORP\\DEVELOPERS")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                //GraphicsRequests model = db.GraphicRequest.Where(x => x.GraphicsRequestID == id).FirstOrDefault();
                GraphicsRequests model = db.GraphicRequest.Include(x => x.Files).FirstOrDefault(x => x.GraphicsRequestID == id);
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateRequestStatus(model.GraphicRequestStatusID);
                PopulateProjectCodes(model.project_id);
                PopulateGraphicsStaff(model.AssigneeID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /GraphicRequests/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\GRAPHICS, SCGCORP\\Developers")]
        //[Authorize(Roles = "SCGCORP\\DEVELOPERS")]
        public ActionResult ManageRequest(GraphicsRequests model, string Buttons)
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
                                model.GraphicRequestStatusID = 2;
                                model.AssignDate = DateTime.Now;
                                model.StatusDate = DateTime.Now;
                                Emails.GraphicsRequestAssigned(model.GraphicsRequestID, model.Instructions, model.emp_id, model.AssigneeID);
                            }
                            break;
                        case "Mark Complete":
                            model.GraphicRequestStatusID = 3;
                            model.StatusDate = DateTime.Now;
                            Emails.GraphicsRequestCompleted(model.GraphicsRequestID, model.Instructions, model.emp_id);
                            break;
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Record updated.";
                    return RedirectToAction("ManageIndex");
                }
                else
                {
                    PopulateRequestStatus(model.GraphicRequestStatusID);
                    PopulateProjectCodes(model.project_id);
                    PopulateGraphicsStaff(model.AssigneeID);
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

        private void PopulateRequestStatus(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<GraphicRequestStatuses>("EXEC [DBO].[spGetGraphicRequestStatuses]").ToList();
                ViewBag.GraphicRequestStatusID = new SelectList(myList, "GraphicRequestStatusID", "GraphicRequestStatus", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateGraphicsStaff(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<RequestAssignee>("EXEC [DBO].[spGetGraphicsEmployees]").ToList();
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