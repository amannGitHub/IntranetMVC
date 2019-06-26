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
    public class HRController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Mailbox()
        {
            return View();
        }

        public ActionResult EmployeeIndex()
        {
            try
            {
                //TODO create procedure
                var myList = db.Database.SqlQuery<EmployeesViewModel>("EXEC [DBO].[spGetEmployeeDirectory]").ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #region Holidays

        public ActionResult HolidayIndex()
        {
            try
            {
                //TODO create procedure
                var myList = db.Database.SqlQuery<Holidays>("EXEC [DBO].[spGetHolidays] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Holidays/Index
        [HttpPost]
        public ActionResult HolidayIndex(Holidays model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Support":
                        return RedirectToAction("Create", "HR");
                    case "Manage IT Requests":
                        return RedirectToAction("ManageIndex", "HR");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }
        //TODO: remove test roles
        //
        // GET: /Holidays/Create
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult CreateHoliday()
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
        // POST: /Holidays/Create
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult CreateHoliday(Holidays model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Holiday.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "The holiday has been created.";

                    return RedirectToAction("HolidayIndex");
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
        // GET: /Holidays/Edit/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult EditHoliday(int id)
        {
            try
            {
                Holidays model = db.Holiday.Where(x => x.HolidayID == id).FirstOrDefault();
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
        // POST: /Holidays/Edit/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult EditHoliday(Holidays model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    return RedirectToAction("HolidayIndex");
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
        // GET: /Holidays/Delete
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult DeleteHoliday(int id)
        {
            try
            {
                Holidays model = db.Holiday.Where(x => x.HolidayID == id).FirstOrDefault();
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
        // POST: /ITRequests/DeleteHoliday/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult DeleteHoliday(Holidays model)
        {
            try
            {
                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                TempData["Message"] = "Record deleted.";
                return RedirectToAction("HolidayIndex");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion
    }
}