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
    public class EquipmentForRequestsController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentForRequestsViewModel>("EXEC [DBO].[spGetEquipmentForRequests]").ToList();
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
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Index(EquipmentForRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Add Equipment":
                        return RedirectToAction("Create", "EquipmentForRequests");
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
        // GET: /EquipmentForRequests/Create
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Create()
        {
            try
            {
                EquipmentForRequests model = db.EquipmentForRequest.Create();   //create a new, empty model to retain emp_id

                //TODO populate needed drop lists
                PopulateEquipmentConditions();
                PopulateEquipmentCategories();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentForRequests/Create
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Create(EquipmentForRequests model)
        {
            try
            {
                //TODO set any needed fields prior to creating a record
                //model.CreatedDate = DateTime.Now;
                //model.StatusDate = DateTime.Now;
                //model.ITRequestStatusID = 1; //Submitted

                if (ModelState.IsValid)
                {
                    db.EquipmentForRequest.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your equipment has been added.";

                    //We don't need an email for this system
                    //Emails.ITRequestSubmitted(model.ITRequestID, model.RequestDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    //TODO populate any needed droplists
                    PopulateEquipmentConditions();
                    PopulateEquipmentCategories();
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
        // GET: /EquipmentForRequests/Edit/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Edit(int id)
        {
            try
            {
                EquipmentForRequests model = db.EquipmentForRequest.Where(x => x.EquipmentID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                //TODO populate any lists
                PopulateEquipmentConditions(model.EquipmentConditionID);
                PopulateEquipmentCategories(model.EquipmentCategoryID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentForRequests/Edit/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Edit(EquipmentForRequests model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    //TODO replace with new email
                    //Emails.ITRequestEdited(model.ITRequestID, model.RequestDescription, model.emp_id);

                    return RedirectToAction("Index");
                }
                else
                {
                    //Populate Dropdowns
                    PopulateEquipmentConditions();
                    PopulateEquipmentCategories();
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
        // GET: /EquipmentForRequests/Details
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Details(int id)
        {
            try
            {
                EquipmentForRequests model = db.EquipmentForRequest.Where(x => x.EquipmentID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                //TODO populate any drop down lists
                PopulateEquipmentConditions(model.EquipmentConditionID);
                PopulateEquipmentCategories(model.EquipmentCategoryID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /EquipmentForRequests/Delete
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Delete(int id)
        {
            try
            {
                EquipmentForRequests model = db.EquipmentForRequest.Where(x => x.EquipmentID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                //Populate dropdowns
                PopulateEquipmentConditions();
                PopulateEquipmentCategories();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /EquipmentForRequests/Delete/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Delete(EquipmentForRequests model)
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

        private string DeleteRequest(EquipmentForRequests model)
        {
            string sMessage = "";
            try
            {
                //TODO fix this for a system that has no assigning/emailing(?)
                #region Prep Deletion Email

                //this must be done before it's deleted or there's no way to craft the email, but sending should occur after

                //Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                //string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                //string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                //TODO create stored procedure
                //EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", model.EquipmentID).First();

                //TODO create email
                //string Subject = "Deleted SCG IT Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                //    + " - " + sRequest.ProjectCode;

                //string MessageToTechs = "An IT Support Request for " + sEmployeeName + " has been deleted.<br><br>Project Code: " + 
                //    sRequest.ProjectCode + ".<br>Request Type: " + sRequest.ITRequestType + ".<br>Date: " + 
                //    sRequest.CreatedDate.ToShortDateString() + ".<br>Description: " + sRequest.RequestDescription;

                #endregion

                #region Delete

                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                #endregion

                //TODO send email
                //Emails.ITRequestDeleted(MessageToTechs, Subject, EmployeeEmail);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        //There are no Manage screens, so I removed that region

        #region Populate DDLs
        
        //TODO create all the ones needed here

        private void PopulateEquipmentConditions(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentConditionsForRequests>("EXEC [DBO].[spGetEquipmentConditions]").ToList();
                ViewBag.EquipmentConditionID = new SelectList(myList, "EquipmentConditionID", "EquipmentCondition", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(),"PopulateEquipmentConditions");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEquipmentCategories(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentCategoriesForRequests>("EXEC [DBO].[spGetEquipmentCategoriesForRequests]").ToList();
                ViewBag.EquipmentCategoryID = new SelectList(myList, "EquipmentCategoryID", "EquipmentCategory", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEquipmentCategories");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

    }
}