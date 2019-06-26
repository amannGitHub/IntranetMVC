using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.ViewModels;
using Intranet.Models;
using Intranet.BLL;
using System.Data.Entity;
using System.Web.Routing;

namespace Intranet.Controllers
{
    public class EquipmentRequestsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentRequestsController/Index
        [HttpPost]
        public ActionResult Index(EquipmentRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Equipment":
                        return RedirectToAction("Create", "EquipmentRequests");
                    case "Manage Equipment Requests":
                        return RedirectToAction("ManageIndex", "EquipmentRequests");
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
        // GET: /EquipmentRequestsController/Create
        public ActionResult Create()
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Create();   //create a new, empty model to retain emp_id
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
        // POST: /EquipmentRequestsController/Create
        [HttpPost]
        public ActionResult Create(EquipmentRequests model)
        {
            try
            {
                model.RequestSubmittedDate = DateTime.Now;
                model.EquipmentRequestStatusID = 4; //Draft

                if (ModelState.IsValid)
                {
                    db.EquipmentRequest.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your request draft has been saved.";

                    return RedirectToAction("AddEquipment", new RouteValueDictionary(
                        new { controller = "EquipmentRequests", action = "AddEquipment", id = model.EquipmentRequestID }));
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
        // GET: /EquipmentRequestsController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Where(x => x.EquipmentRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjectCodes(model.project_id);
                PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);

                //this is for the associated EquipmentRequests
                var EquipmentRequests = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequested] {0}", id).ToList();
                ViewData["EquipmentRequests"] = EquipmentRequests;
                ViewBag.EquipmentCount = EquipmentRequests.Count;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        private void EditSave(string Message, EquipmentRequests model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        //
        // POST: /EquipmentRequestsController/Edit/5
        [HttpPost]
        public ActionResult Edit(EquipmentRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Save Draft":

                        if (ModelState.IsValid)
                        {
                            EditSave("Draft saved.", model);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Update":

                        if (ModelState.IsValid)
                        {
                            EditSave("Record updated.", model);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Submit":

                        if (ModelState.IsValid)
                        {
                            model.EquipmentRequestStatusID = 1; //Submitted
                            EditSave("Request submitted.", model);
                            Emails.EquipmentRequestSubmitted(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Extend Request":
                        return RedirectToAction("ExtendRequest", new RouteValueDictionary(
                            new { controller = "EquipmentRequests", action = "ExtendRequest", id = model.EquipmentRequestID }));

                    case "Add Equipment":
                        return RedirectToAction("AddEquipment", new RouteValueDictionary(
                            new { controller = "EquipmentRequests", action = "AddEquipment", id = model.EquipmentRequestID }));

                    default: return RedirectToAction("Index", "EquipmentRequests");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EquipmentRequestsController/Details
        public ActionResult Details(int id)
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Where(x => x.EquipmentRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjectCodes(model.project_id);
                PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);

                //this is for the associated EquipmentRequests
                var EquipmentRequests = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequested] {0}", id).ToList();
                ViewData["EquipmentRequests"] = EquipmentRequests;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EquipmentRequestsController/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Where(x => x.EquipmentRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjectCodes(model.project_id);
                PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);

                //this is for the associated EquipmentRequests
                var EquipmentRequests = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequested] {0}", id).ToList();
                ViewData["EquipmentRequests"] = EquipmentRequests;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentRequestsController/Delete/5
        [HttpPost]
        public ActionResult Delete(EquipmentRequests model)
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

        private string DeleteRequest(EquipmentRequests model)
        {
            string sMessage = "";
            try
            {
                #region Prep Deletion Email

                //this must be done before it's deleted or there's no way to craft the email, but sending should occur after

                Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", model.EquipmentRequestID).First();
                string Subject = "Deleted SCG Equipment Request for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString();

                string MessageToTechs = "An Equipment Request for " + sEmployeeName + " has been deleted.<br><br>Date: " +
                    sRequest.RequestSubmittedDate.ToShortDateString() + "<br>Request #" + sRequest.EquipmentRequestID;

                #endregion

                #region Delete

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.EquipmentRequestDetail.RemoveRange(db.EquipmentRequestDetail.Where(x => x.EquipmentRequestID == model.EquipmentRequestID));
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                #endregion

                Emails.EquipmentRequestDeleted(MessageToTechs, Subject, EmployeeEmail);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        //
        // GET: /EquipmentRequestsController/ExtendRequest/5
        public ActionResult ExtendRequest(int id)
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Where(x => x.EquipmentRequestID == id).FirstOrDefault();
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
        // POST: /EquipmentRequestsController/ExtendRequest/5
        [HttpPost]
        public ActionResult ExtendRequest(EquipmentRequests model, string Buttons)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EditSave("Extension submitted.", model);
                    Emails.EquipmentExtensionSubmitted(model.EquipmentRequestID, model.emp_id);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #region Equipment

        public ActionResult AddEquipment(int id)
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetAvailableEquipmentForRequests] {0}", id).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentRequestsController/AddEquipment
        [HttpPost]
        public ActionResult AddEquipment(int id, int[] EquipmentIDs)
        {
            try
            {
                foreach (var EquipmentID in EquipmentIDs)
                {
                    SaveEquipment(EquipmentID, id);
                }

                TempData["Message"] = "Equipment Added";
                return RedirectToAction("Edit", new RouteValueDictionary(
                        new { controller = "EquipmentRequests", action = "Edit", id = id }));
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        public void SaveEquipment(int EquipmentID, int EquipmentRequestID)
        {
            try
            {
                EquipmentRequestDetails model = db.EquipmentRequestDetail.Create();   //create a new, empty model to retain emp_id
                model.EquipmentID = EquipmentID;
                model.EquipmentRequestID = EquipmentRequestID;

                db.EquipmentRequestDetail.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        //
        // GET: /EquipmentRequestsController/DeleteEquipment
        public ActionResult RemoveEquipment(int id)
        {
            try
            {
                EquipmentRequestDetails model = db.EquipmentRequestDetail.Where(x => x.EquipmentRequestDetailID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                var EquipmentToRemove = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentToRemove] {0}", id).ToList();
                ViewData["EquipmentToRemove"] = EquipmentToRemove;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentRequestsController/DeleteEquipment/5
        [HttpPost]
        public ActionResult RemoveEquipment(EquipmentRequestDetails model)
        {
            try
            {
                TempData["Message"] = DeleteRequestedEquipment(model);
                return RedirectToAction("Edit", new RouteValueDictionary(
                            new { controller = "EquipmentRequests", action = "Edit", id = model.EquipmentRequestID }));
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        private string DeleteRequestedEquipment(EquipmentRequestDetails model)
        {
            string sMessage = "";
            try
            {
                
                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                return sMessage = "Equipment removed.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        #endregion

        #region Manage Requests

        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageIndex()
        {
            try
            {
                //this doesn't show drafts
                var myList = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetManageEquipmentRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EquipmentRequestsController/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Where(x => x.EquipmentRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjectCodes(model.project_id);
                PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);

                //this is for the associated EquipmentRequests
                var EquipmentRequests = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequested] {0}", id).ToList();
                ViewData["EquipmentRequests"] = EquipmentRequests;
                ViewBag.EquipmentCount = EquipmentRequests.Count;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentRequestsController/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageRequest(EquipmentRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    
                    case "Update":

                        if (ModelState.IsValid)
                        {
                            EditSave("Record updated.", model);
                            Emails.EquipmentUpdated(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Approve":

                        if (ModelState.IsValid)
                        {
                            model.EquipmentRequestStatusID = 2; //Approved
                            model.RequestDecisionDate = DateTime.Now;
                            EditSave("Request approved.", model);
                            Emails.EquipmentApproved(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Reject":

                        if (ModelState.IsValid)
                        {
                            model.EquipmentRequestStatusID = 3; //Rejected
                            model.RequestDecisionDate = DateTime.Now;
                            EditSave("Request rejected.", model);
                            Emails.EquipmentRejected(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Mark Returned":

                        if (ModelState.IsValid)
                        {
                            model.EquipmentRequestStatusID = 6;
                            EditSave("Record updated.", model);
                            Emails.EquipmentReturned(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Approve Extension":

                        if (ModelState.IsValid)
                        {
                            model.RequestEndDate = Convert.ToDateTime(model.ExtensionDate);
                            model.ExtensionDate = Convert.ToDateTime("1/1/2001"); //clear the date
                            EditSave("Extension approved.", model);
                            Emails.EquipmentExtensionApproved(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    case "Reject Extension":

                        if (ModelState.IsValid)
                        {
                            model.ExtensionDate = Convert.ToDateTime("1/1/2001"); //clear the date
                            EditSave("Extension rejected.", model);
                            Emails.EquipmentExtensionRejected(model.EquipmentRequestID, model.emp_id);
                            return RedirectToAction("ManageIndex");
                        }
                        else
                        {
                            PopulateProjectCodes(model.project_id);
                            PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);
                        }

                        return View(model);

                    default: return RedirectToAction("ManageIndex", "EquipmentRequests");
                }


            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EquipmentRequestsController/ManageDelete
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageDelete(int id)
        {
            try
            {
                EquipmentRequests model = db.EquipmentRequest.Where(x => x.EquipmentRequestID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjectCodes(model.project_id);
                PopulateEquipmentRequestStatuses(model.EquipmentRequestStatusID);

                //this is for the associated EquipmentRequests
                var EquipmentRequests = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequested] {0}", id).ToList();
                ViewData["EquipmentRequests"] = EquipmentRequests;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentRequestsController/ManageDelete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult ManageDelete(EquipmentRequests model)
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

        private void PopulateEquipmentRequestStatuses(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentRequestStatuses>("EXEC [DBO].[spGetEquipmentRequestStatuses]").ToList();
                ViewBag.EquipmentRequestStatusID = new SelectList(myList, "EquipmentRequestStatusID", "EquipmentRequestStatus", selected);
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