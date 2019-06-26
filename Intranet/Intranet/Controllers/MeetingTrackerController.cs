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
    public class MeetingTrackerController : Controller
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
                var myList = db.Database.SqlQuery<MeetingsViewModel>("EXEC [DBO].[spGetMeetings]").ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /MeetingTracker/Index
        [HttpPost]
        public ActionResult Index(MeetingTracker model)
        {
            try
            {
                return RedirectToAction("Create", "MeetingTracker");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /MeetingTracker/Create
        public ActionResult Create()
        {
            try
            {
                MeetingTracker model = db.Meetings.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code
                model.FirstName = true;
                model.FirstNameReq = true;
                model.LastName = true;
                model.LastNameReq = true;
                model.Email = true;
                model.EmailReq = true;
                model.Organization = true;
                model.OrganizationReq = true;

                PopulateClients();
                PopulateProjects();
                PopulateEmployees(model.emp_id);
                PopulateLocations(model.LocationID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /MeetingTracker/Create
        [HttpPost]
        public ActionResult Create(MeetingTracker model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Meetings.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your meeting has been saved.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateClients();
                    PopulateProjects();
                    PopulateEmployees();
                    PopulateLocations(model.LocationID);
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
        // GET: /MeetingTracker/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                MeetingTracker model = db.Meetings.Where(x => x.MeetingID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjects(model.project_id);
                PopulateClients(model.ClientID);
                PopulateEmployees(model.MeetingPlannerID);
                PopulateLocations(model.LocationID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /MeetingTracker/Edit/5
        [HttpPost]
        public ActionResult Edit(MeetingTracker model)
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
                    PopulateProjects(model.project_id);
                    PopulateClients(model.ClientID);
                    PopulateEmployees(model.MeetingPlannerID);
                    PopulateLocations(model.LocationID);
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
        // GET: /MeetingTracker/Details
        public ActionResult Details(int id)
        {
            try
            {
                MeetingTracker model = db.Meetings.Where(x => x.MeetingID == id).FirstOrDefault();

                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateProjects(model.project_id);
                PopulateClients(model.ClientID);
                PopulateEmployees(model.MeetingPlannerID);
                PopulateLocations(model.LocationID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #region Populate DDLs

        private void PopulateEmployees(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<MeetingPlanners>("EXEC [DBO].[spGetMeetingPlanners]").ToList();
                ViewBag.MeetingPlannerID = new SelectList(myList, "MeetingPlannerID", "MeetingPlanner", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
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

        private void PopulateClients(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Clients>("EXEC [DBO].[spGetClientsActive]").ToList();
                ViewBag.ClientID = new SelectList(myList, "ClientID", "Acronym", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateClients");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateLocations(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Locations>("EXEC [DBO].[spGetMeetingLocations]").ToList();
                ViewBag.LocationID = new SelectList(myList, "LocationID", "Name", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateLocations");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion
    }
}