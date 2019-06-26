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
    public class InventoriesController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        //TODO: remove test roles
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Index()
        {
            try
            {
                var myList = db.Database.SqlQuery<InventoryViewModel>("EXEC [DBO].[spGetInventory] {0}", userName).ToList();
                return View(myList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/Index
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult Index(Inventories model, string Buttons)
        {
            try
            {
                switch (Buttons)
                {
                    case "Add Staff Gear":
                        return RedirectToAction("CreateStaff", "Inventories");
                    case "Add Spare Gear":
                        return RedirectToAction("CreateSpare", "Inventories");
                    case "Add Printer Gear":
                        return RedirectToAction("CreatePrinter", "Inventories");
                    case "Add Video Gear":
                        return RedirectToAction("CreateVideo", "Inventories");
                    case "Add Laptop Gear":
                        return RedirectToAction("CreateLaptop", "Inventories");
                    case "Add Recording Gear":
                        return RedirectToAction("CreateRecorder", "Inventories");
                    case "Add Frederick Gear":
                        return RedirectToAction("CreateFrederick", "Inventories");
                    case "Add Donation":
                        return RedirectToAction("CreateDonation", "Inventories");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public Inventories GetCreate(int iEquipmentType)
        {
            //iEquipmentType 2 = computer gear; 

            Inventories model = db.Inventory.Create();

            try
            {
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                PopulateEverything(2, mEmployee.emp_id);
                return (model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "GetCreate");
                return (model);
            }
        }

        #region Staff Gear

        //
        // GET: /Inventories/CreateStaff
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateStaff()
        {
            try
            {
                Inventories model = GetCreate(2);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateStaff
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateStaff(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 1; //Staff
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(2, model.emp_id);
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
        // GET: /Inventories/EditStaff/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditStaff(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(2, model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditStaff/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditStaff(Inventories model)
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
                    //PopulateEquipmentTypes(2, model.EquipmentTypeID);  //computer gear
                    //PopulateEquipmentCategories(model.EquipmentCategoryID);
                    //PopulateEquipmentManufacturers(model.ManufacturerID);
                    //PopulateHDSizes(model.HardDriveSizeID);
                    //PopulateCPUs(model.CPUID);
                    //PopulateOperatingSystems(model.OperatingSystemID);
                    //PopulateRAMs(model.RAMID);
                    //PopulateEquipmentModels(0, 0);
                    //PopulateEmployees(model.emp_id);

                    PopulateEverythingEdit(2, model);
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

        #region Printers

        //
        // GET: /Inventories/CreatePrinter
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreatePrinter()
        {
            try
            {
                Inventories model = GetCreate(2);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreatePrinter
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreatePrinter(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 3; //printers
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(2, model.emp_id);
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
        // GET: /Inventories/EditPrinter/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditPrinter(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(2, model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditPrinter/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditPrinter(Inventories model)
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
                    PopulateEverythingEdit(2, model);
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

        #region Video

        //
        // GET: /Inventories/CreateVideoGear
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateVideo()
        {
            try
            {
                Inventories model = db.Inventory.Create();   //create a new, empty model to retain emp_id
                Employees mEmployee = db.Employee.Where(x => x.network_id == userName).FirstOrDefault();
                model.emp_id = mEmployee.emp_id;    //set for the saving code

                PopulateEquipmentTypes(1);  //video gear
                PopulateEquipmentManufacturers();
                PopulateEquipmentModels(0, 0);
                PopulateEmployees(model.emp_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateVideoGear
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateVideo(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 4; //video
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEquipmentTypes(1);  //video gear
                    PopulateEquipmentManufacturers();
                    PopulateEquipmentModels(0, 0);
                    PopulateEmployees(model.emp_id);
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
        // GET: /Inventories/EditVideoGear/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditVideo(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEquipmentTypes(1, model.EquipmentTypeID);  //video gear
                PopulateEquipmentCategories(model.EquipmentCategoryID);
                PopulateEquipmentManufacturers(model.ManufacturerID);
                PopulateEquipmentModels(model.ManufacturerID, model.EquipmentModelID);
                PopulateEmployees(model.emp_id);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditVideoGear/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditVideo(Inventories model)
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
                    PopulateEquipmentTypes(1, model.EquipmentTypeID);  //video gear
                    PopulateEquipmentCategories(model.EquipmentCategoryID);
                    PopulateEquipmentManufacturers(model.ManufacturerID);
                    PopulateEquipmentModels(0, 0);
                    PopulateEmployees(model.emp_id);
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

        #region Laptops

        //
        // GET: /Inventories/CreateLaptops
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateLaptop()
        {
            try
            {
                Inventories model = GetCreate(2);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateLaptops
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateLaptop(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 5; //Laptops
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(2, model.emp_id);
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
        // GET: /Inventories/EditLaptops/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditLaptop(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(2, model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditLaptops/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditLaptop(Inventories model)
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
                    PopulateEverythingEdit(2, model);
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

        #region Recorders

        //
        // GET: /Inventories/CreateRecorders
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateRecorder()
        {
            try
            {
                Inventories model = GetCreate(3);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateRecorders
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateRecorder(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 6; //Laptops
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(3, model.emp_id);
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
        // GET: /Inventories/EditRecorders/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditRecorder(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(3, model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditRecorders/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditRecorder(Inventories model)
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
                    PopulateEverythingEdit(3, model);
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

        #region Spare Gear

        //NOTE: these methods are exactly the same as the Staff ones

        //
        // GET: /Inventories/CreateSpare
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateSpare()
        {
            try
            {
                Inventories model = GetCreate(2);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateSpare
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateSpare(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 1; //Staff
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(2, model.emp_id);
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
        // GET: /Inventories/EditSpare/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditSpare(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(2, model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditSpare/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditSpare(Inventories model)
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
                    PopulateEverythingEdit(2, model);
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

        #region Frederick

        //
        // GET: /Inventories/CreateFrederick
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateFrederick()
        {
            try
            {
                Inventories model = GetCreate(2);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateFrederick
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateFrederick(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 8; //Frederick
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(2, model.emp_id);
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
        // GET: /Inventories/EditFrederick/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditFrederick(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(2, model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditFrederick/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditFrederick(Inventories model)
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
                    PopulateEverythingEdit(2, model);
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

        #region Donation

        //
        // GET: /Inventories/CreateDonation
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateDonation()
        {
            try
            {
                Inventories model = GetCreate(2);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/CreateDonation
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult CreateDonation(Inventories model)
        {
            try
            {
                model.EquipmentCategoryID = 9; //Donation
                model.CreatedDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Inventory.Add(model);
                    db.SaveChanges();
                    TempData["Message"] = "Your support request has been submitted.";

                    return RedirectToAction("Index");
                }
                else
                {
                    PopulateEverything(2, model.emp_id);
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
        // GET: /Inventories/EditDonation/5
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditDonation(int id)
        {
            try
            {
                Inventories model = db.Inventory.Where(x => x.InventoryID == id).FirstOrDefault();
                if (model == null)
                {
                    TempData["Message"] = "No such record found. Please try again.";
                    return RedirectToAction("ViewMessage", "Home");
                }

                PopulateEverythingEdit(2, model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), ControllerContext.RouteData.Values["action"].ToString());
                return RedirectToAction("ViewMessage", "Home");
            }
        }

        //
        // POST: /Inventories/EditDonation/5
        [HttpPost]
        [Authorize(Roles = "SCGCORP\\SCG Computer Support, SCGCORP\\Developers")]
        public ActionResult EditDonation(Inventories model)
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
                    PopulateEverythingEdit(2, model);
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

        //Populate all ddls whether needed or not. One universal method is easier
        private void PopulateEverything(int iEquipmentType, int? EmployeeID)
        {
            PopulateEquipmentTypes(iEquipmentType);
            PopulateEquipmentManufacturers();
            PopulateHDSizes();
            PopulateCPUs();
            PopulateOperatingSystems();
            PopulateRAMs();
            PopulateEquipmentModels(0, 0);
            PopulateEmployees(EmployeeID);
            PopulateOpticalDrives();
            PopulateToners();
        }

        //Populate all ddls for Edit pages whether needed or not. One universal method is easier
        private void PopulateEverythingEdit(int iEquipmentType, Inventories model)
        {
            PopulateEquipmentTypes(iEquipmentType, model.EquipmentTypeID);  //computer gear
            PopulateEquipmentCategories(model.EquipmentCategoryID);
            PopulateEquipmentManufacturers(model.ManufacturerID);
            PopulateHDSizes(model.HardDriveSizeID);
            PopulateCPUs(model.CPUID);
            PopulateOperatingSystems(model.OperatingSystemID);
            PopulateRAMs(model.RAMID);
            PopulateEquipmentModels(model.ManufacturerID, model.EquipmentModelID);
            PopulateEmployees(model.emp_id);
            PopulateOpticalDrives(model.OpticalDriveID);
            PopulateToners(model.TonerID);
        }

        public JsonResult GetEquipmentModels(string id)
        {
            if (id != String.Empty)
            {
                var myList = db.Database.SqlQuery<EquipmentModels>("EXEC [DBO].[spGetEquipmentModels] {0}", id).ToList();
                return Json(new SelectList(myList, "EquipmentModelID", "EquipmentModel"));
            }
            else
            {
                return Json(String.Empty);
            }
        }

        private void PopulateEmployees(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Employee>("EXEC [DBO].[spGetEmployee]").ToList();
                ViewBag.emp_id = new SelectList(myList, "emp_id", "EmployeeName", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateVendors");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateHDSizes(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<HardDriveSizes>("EXEC [DBO].[spGetHardDriveSizes]").ToList();
                ViewBag.HardDriveSizeID = new SelectList(myList, "HardDriveSizeID", "HardDriveSize", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateHDSizes");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateCPUs(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<CPUs>("EXEC [DBO].[spGetCPUs]").ToList();
                ViewBag.CPUID = new SelectList(myList, "CPUID", "CPU", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateCPUs");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateOperatingSystems(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<OperatingSystems>("EXEC [DBO].[spGetOSes]").ToList();
                ViewBag.OperatingSystemID = new SelectList(myList, "OperatingSystemID", "OperatingSystem", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateOperatingSystems");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateRAMs(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<RAMs>("EXEC [DBO].[spGetRamSizes]").ToList();
                ViewBag.RAMID = new SelectList(myList, "RAMID", "RAM", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateRAMs");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateOpticalDrives(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<OpticalDrives>("EXEC [DBO].[spGetOpticalDrives]").ToList();
                ViewBag.OpticalDriveID = new SelectList(myList, "OpticalDriveID", "OpticalDrive", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateOpticalDrives");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateToners(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Toners>("EXEC [DBO].[spGetToners]").ToList();
                ViewBag.TonerID = new SelectList(myList, "TonerID", "Toner", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateToners");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEquipmentModels(int? ManufacturerID, int? ModelID, object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentModels>("EXEC [DBO].[spGetEquipmentModels] {0}", ManufacturerID).ToList();
                ViewBag.EquipmentModelID = new SelectList(myList, "EquipmentModelID", "EquipmentModel", ModelID);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEquipmentModels");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEquipmentTypes(int myClass, object selected = null)
        {
            try
            {
                //myClass 0 = all, 1 = video, 2 = computer, 3 = recorders

                var myList = db.Database.SqlQuery<EquipmentTypes>("EXEC [DBO].[spGetEquipmentTypes] {0}", myClass).ToList();
                ViewBag.EquipmentTypeID = new SelectList(myList, "EquipmentTypeID", "EquipmentType", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEquipmentTypes");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEquipmentCategories(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<EquipmentCategories>("EXEC [DBO].[spGetEquipmentCategories]").ToList();
                ViewBag.EquipmentCategoryID = new SelectList(myList, "EquipmentCategoryID", "EquipmentCategory", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEquipmentCategories");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        private void PopulateEquipmentManufacturers(object selected = null)
        {
            try
            {
                var myList = db.Database.SqlQuery<Manufacturers>("EXEC [DBO].[spGetManufacturers]").ToList();
                ViewBag.ManufacturerID = new SelectList(myList, "ManufacturerID", "Manufacturer", selected);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Helpers.LogError(ex.Message, ex.StackTrace, userName, ControllerContext.RouteData.Values["controller"].ToString(), "PopulateEquipmentManufacturers");
                RedirectToAction("ViewMessage", "Home");
            }
        }

        #endregion

    }
}