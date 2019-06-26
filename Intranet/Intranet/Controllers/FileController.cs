using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.Models;

namespace Intranet.Controllers
{
    public class FileController : Controller
    {
        string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
        private SolutionDBContext db = new SolutionDBContext();

        // GET: Files
        public ActionResult Index(int id)
        {
            var fileToRetrieve = db.File.Find(id);
            //return File(fileToRetrieve.ContentFile.ToArray(), "application/octet-stream", fileToRetrieve.FileName);
            return File(fileToRetrieve.ContentFile, fileToRetrieve.ContentType);
        }

        public ActionResult Download(int id)
        {
            var file = db.File.First(f => f.FileID == id);
            return File(file.ContentFile.ToArray(), "application/octet-stream", file.FileName);
        }
    }
}