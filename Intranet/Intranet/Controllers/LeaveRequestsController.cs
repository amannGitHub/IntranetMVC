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

using System.Web.UI;
using System.Globalization;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Intranet.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private SolutionDBContext db = new SolutionDBContext();

        protected string currentDate;
        int numDaysInMonth;

        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<LeaveRequestsViewModel>("EXEC [DBO].[spGetLeaveRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /LeaveRequests/Index
        [HttpPost]
        public ActionResult Index(LeaveRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Request Leave":
                        return RedirectToAction("Create", "LeaveRequests");
                    case "Manage Staff Requests":
                        return RedirectToAction("ManageIndex", "LeaveRequests");
                    case "Vacation Calendar":
                        return RedirectToAction("VacationCalendar", "LeaveRequests");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        #region Project Codes Index
        //
        //GET: /Projects/Codes
        public ActionResult Codes()
        {
            try
            {
                var myList = db.Database.SqlQuery<ProjectsViewModel>("EXEC [DBO].[spGetProjects]").ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Projects/Codes
        [HttpPost]
        public ActionResult Codes(Projects model)
        {
            try
            {
                //switch (Buttons)
                //{
                //    case "Create Project":
                //        return RedirectToAction("Create", "Projects");
                //    case "Manage Projects":
                //        return RedirectToAction("ManageIndex", "Projects");
                //}

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }
        #endregion

        //
        // GET: /LeaveRequests/Create
        public ActionResult Create()
        {
            try
            {
                LeaveRequests model = db.LeaveRequest.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

                //if the user clicked the Check Balance button, they already had values set for Start Date, End Date, and Total Hours
                if(TempData["LeaveModel"] != null)
                {
                    model = (LeaveRequests)TempData["LeaveModel"];
                }

                TempData["BalanceCheck"] = BalanceCheck(userName);
                PopulateLeaveTypes();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /LeaveRequests/Create
        [HttpPost]
        public ActionResult Create(LeaveRequests model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Create":

                        if (ModelState.IsValid)
                        {
                            model.create_date = DateTime.Now;
                            model.status_dt = DateTime.Now;
                            model.status = "Submitted";

                            if (ModelState.IsValid)
                            {
                                db.LeaveRequest.Add(model);
                                db.SaveChanges();
                                TempData["Message"] = "Your leave request has been submitted.";

                                Emails.SendLeaveRequest(model.emp_id, 
                                    model.leave_start_dt.ToShortDateString() + " " + model.leave_start_dt.ToShortTimeString(), 
                                    model.leave_end_dt.ToShortDateString() + " " + model.leave_end_dt.ToShortTimeString(),
                                    model.totalhours, model.leavetype_id, model.leave_reason, model.leaverequest_id);

                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["BalanceCheck"] = BalanceCheck(userName);
                                PopulateLeaveTypes();
                            }
                        }

                        break;

                    case "Check Balance":

                        TempData["BalanceCheck"] = BalanceCheck(userName);
                        TempData["FutureBalanceCheck"] = FutureBalanceCheck(userName, model.totalhours, model.leave_start_dt);
                        //TempData["LeaveStart"] = model.leave_start_dt.ToString("yyyyMMdd");
                        PopulateLeaveTypes();

                        return RedirectToAction("Create", new RouteValueDictionary(
                            new { controller = "LeaveRequests", action = "Create", leave_start_dt = model.leave_start_dt,
                                leave_end_dt = model.leave_end_dt, totalhours = model.totalhours }));

                        //break;
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
        // GET: /LeaveRequests/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                TempData["BalanceCheck"] = BalanceCheck(userName);

                LeaveRequests model = db.LeaveRequest.Where(x => x.leaverequest_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateLeaveTypes(model.leavetype_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /LeaveRequests/Edit/5
        [HttpPost]
        public ActionResult Edit(LeaveRequests model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.status == "Approved")
                    {
                        TempData["Message"] = "This leave request has already been approved and cannot be edited except by the manager.";
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Message"] = "Record saved.";

                        //ToDo could add an email that it was updated but am skipping for now
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["BalanceCheck"] = BalanceCheck(userName);
                    PopulateLeaveTypes(model.leavetype_id);
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
        // GET: /LeaveRequests/Details
        public ActionResult Details(int id)
        {
            try
            {
                LeaveRequests model = db.LeaveRequest.Where(x => x.leaverequest_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateLeaveTypes(model.leavetype_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // GET: /LeaveRequests/Delete
        public ActionResult Delete(int id)
        {
            try
            {
                LeaveRequests model = db.LeaveRequest.Where(x => x.leaverequest_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateLeaveTypes(model.leavetype_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /LeaveRequests/Delete/5
        [HttpPost]
        public ActionResult Delete(LeaveRequests model)
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

        private string DeleteRequest(LeaveRequests model)
        {
            string sMessage = "";
            try
            {
                bool oldValidateOnSaveEnabled = db.Configuration.ValidateOnSaveEnabled;

                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(model).State = EntityState.Deleted;
                db.SaveChanges();

                db.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;

                Emails.DeleteLeaveRequest(model.emp_id, model.leave_start_dt.ToShortDateString() + " " + model.leave_start_dt.ToShortTimeString(),
                    model.leave_end_dt.ToShortDateString() + " " + model.leave_end_dt.ToShortTimeString(),
                    model.totalhours, model.leavetype_id, model.leave_reason, model.leaverequest_id);

                return sMessage = "Record deleted.";
            }
            catch (Exception ex)
            {
                return sMessage = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
            }
        }

        #region Manage Requests
        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageIndex()
        {
            try
            {
                var myList = db.Database.SqlQuery<LeaveRequestsViewModel>("EXEC [DBO].[spManageRequests] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageDelete(int id)
        {
            try
            {
                LeaveRequests model = db.LeaveRequest.Where(x => x.leaverequest_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateLeaveTypes(model.leavetype_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /LeaveRequests/ManagerDelete/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        [HttpPost]
        public ActionResult ManageDelete(LeaveRequests model)
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
        // GET: /LeaveRequests/ManageRequest/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(int id)
        {
            try
            {
                LeaveRequests model = db.LeaveRequest.Where(x => x.leaverequest_id == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                //TODO this check should try to figure out how much leave they'll have by then
                Employees mEmployee = db.Employee.Where(x => x.emp_id == model.emp_id).FirstOrDefault();
                string sBalanceCheck = "Their vacation balance: " + mEmployee.vacationhours + " hours. Accrual rate: " + mEmployee.vacationrate
                    + ". Sick balance: " + mEmployee.sickhours + " hours. Accrual rate: " + mEmployee.sickrate;

                ViewBag.EmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                TempData["BalanceCheck"] = sBalanceCheck;

                PopulateLeaveTypes(model.leavetype_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /LeaveRequests/ManageRequest/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        //Test Roles
        //[Authorize(Roles = "SCGCORP\\Developers")]
        public ActionResult ManageRequest(LeaveRequests model, string Buttons)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    switch (Buttons)
                    {
                        case "Approve":
                            model.status = "Approved";
                            break;
                        case "HR Approve":
                            model.HRApproved = true;
                            break;
                        case "Reject":
                            model.status = "Rejected";
                            break;
                        case "Delete":
                            return RedirectToAction("ManageDelete", new { id = model.leaverequest_id } );
                    }

                    model.status_dt = DateTime.Now;

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    Emails.ManageLeaveRequest(model.emp_id, model.leave_start_dt.ToShortDateString() + " " + model.leave_start_dt.ToShortTimeString(),
                        model.leave_end_dt.ToShortDateString() + " " + model.leave_end_dt.ToShortTimeString(),
                        model.totalhours, model.leavetype_id, model.leave_reason, model.leaverequest_id, model.status);

                    TempData["Message"] = "Record updated.";
                    return RedirectToAction("ManageIndex");
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

        private void PopulateLeaveTypes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<LeaveTypes>("EXEC [DBO].[spGetLeaveTypes]").ToList();
                ViewBag.leavetype_id = new SelectList(myList, "leavetype_id", "leavetype", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

        #region Balance Checks

        private string BalanceCheck(string username)
        {
            //This gets current balance and puts it onscreen
            Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
            string sBalanceCheck = "Your vacation balance: " + mEmployee.vacationhours + " hours. Accrual rate: " + mEmployee.vacationrate
                + ". Sick balance: " + mEmployee.sickhours + " hours. Accrual rate: " + mEmployee.sickrate;

            return sBalanceCheck;
        }

        private string FutureBalanceCheck(string username, Single TotalHoursRequested, DateTime LeaveStateDate)
        {
            //This gets their balance based on the requests they're putting in
            string sBalanceCheck = "Unable to calculate.";

            try
            {
                LeaveBalance LeaveBalance = db.Database.SqlQuery<LeaveBalance>("EXEC [DBO].[spCheckLeaveBalance] {0}, {1}, {2}",
                    userName, TotalHoursRequested, LeaveStateDate).FirstOrDefault();

                sBalanceCheck = "You will have approximately " + LeaveBalance.AccruedVacationHours + " vacation hours and " +
                    LeaveBalance.AccruedSickHours + " sick hours.";

            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "FutureBalanceCheck");
            }

            return sBalanceCheck;
        }



        [HttpGet]
        public ActionResult VacationCalendar()
        {
            string dtToday = DateTime.Now.ToString("MM/dd/yyyy");
            currentDate = dtToday;
            displayCalendarDate(dtToday, "today");

            try
            {
                var myList = db.Database.SqlQuery<LeaveRequestsViewModel>("EXEC [DBO].[spGetEmployee]").ToList();
                return View(myList);
            }

            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        [HttpPost]
        public ActionResult VacationCalendar(FormCollection fc, LeaveRequests model, string Buttons)
        {
            string HDName = fc["hdnMonthYear"]; // Basic Helper

            try
            {
                switch (Buttons)
                {
                    case "Previous Month":
                        displayCalendarDate(HDName, "previous");
                        break;
                    case "Next Month":
                        displayCalendarDate(HDName, "next");
                        break;
                }

                var myList = db.Database.SqlQuery<LeaveRequestsViewModel>("EXEC [DBO].[spGetEmployee]").ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }



        protected void displayCalendarDate(string getDate, string dateSelect)
        {
            switch (dateSelect)
            {
                //get previous month
                case "previous":
                    DateTime prevDate = DateTime.ParseExact(getDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    prevDate = prevDate.AddMonths(-1);
                    currentDate = prevDate.ToString("MM/dd/yyyy");
                    break;
                //get next month
                case "next":
                    DateTime nextDate = DateTime.ParseExact(getDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    nextDate = nextDate.AddMonths(1);
                    currentDate = nextDate.ToString("MM/dd/yyyy");
                    break;
                //get current year and month
                default:
                    currentDate = getDate;
                    break;
            }

            string selMonth = currentDate.Split('/')[0].ToString();
            string selYear = currentDate.Split('/')[2].ToString();

            //pass in values to get weekend dates
            int monthNum = Convert.ToInt32(selMonth);
            int yearNum = Convert.ToInt32(selYear);
            getWeekendDates(monthNum, yearNum);

            numDaysInMonth = System.DateTime.DaysInMonth(Convert.ToInt32(selYear), Convert.ToInt32(selMonth));
            ViewBag.hdnMonthYear = currentDate;
            ViewBag.MonthYear = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNum) + " " + selYear.ToString();
            ViewBag.DaysInMonth = numDaysInMonth;

        }

        protected void getWeekendDates(int Month, int Year)
        {
            List<int> weekendDates = new List<int>();
            DateTime firstOfMonth = new DateTime(Year, Month, 1);
            DateTime nextDay = firstOfMonth;

            ViewBag.dtMonth = DateTime.DaysInMonth(firstOfMonth.Year, firstOfMonth.Month);

            for (int i = 1; i <= DateTime.DaysInMonth(firstOfMonth.Year, firstOfMonth.Month); i++)
            {
                if (nextDay.DayOfWeek == DayOfWeek.Saturday || nextDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendDates.Add(i);
                }
                nextDay = firstOfMonth.AddDays(i);
            }
            ViewBag.weekendDateList = weekendDates;
        }
    }

    #endregion
}
