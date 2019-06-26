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
    public class HolidaysController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();


        public ActionResult Index()
        {
            try
            {
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
        // POST: /Clients/Index
        [HttpPost]
        public ActionResult Index(Holidays model)
        {
            try
            {
                return RedirectToAction("Create", "Holidays");
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }
        //TODO: remove test roels
        //
        // GET: /Clients/Create
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create()
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
        // POST: /Clients/Create
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Create(Holidays model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Holiday.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your holiday has been submitted.";

                    return RedirectToAction("Index");
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
        // GET: /Clients/Edit/5
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Edit(int id)
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
        // POST: /Clients/Edit/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\Managers, SCGCORP\\Developers")]
        public ActionResult Edit(Holidays model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Record saved.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }

            return View(model);
        }
    }
}