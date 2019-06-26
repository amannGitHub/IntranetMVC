using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Intranet.Models;
using System.Web.Script.Serialization;
using System.Web.Mvc;

namespace Intranet.BLL
{
    public static class Helpers
    {
        internal static string LogError(string Message, string StackTrace, string Username, string Controller, string Page)
        {
            SolutionDBContext db = new SolutionDBContext();

            ErrorModel model = db.ErrorModel.Create();
            model.ErrorMessage = Message;
            model.StackTrace = StackTrace;
            model.UserName = Username;
            model.Controller = Controller;
            model.Page = Page;
            model.ErrorTime = DateTime.Now;

            db.ErrorModel.Add(model);
            db.SaveChanges();

            return "There has been an error. Please try again or contact IT Support. Error Number: " + model.ErrorID;
        }
        
        //add active class to the active nav link
        public static string IsActive(this HtmlHelper html, string control, string action)
        {
            var routeData = html.ViewContext.RouteData;
            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
            //both controller and action must match
            var returnActive = control == routeControl && action == routeAction;
            return returnActive ? "active" : "";
        }
        //top level nav bar active method
        public static string IsActiveTop(this HtmlHelper html, string control)
        {
            var routeData = html.ViewContext.RouteData;
            var routeControl = (string)routeData.Values["controller"];
            //controller must match
            var returnActive = control == routeControl;
            return returnActive ? "active" : "";
        }
    }
}