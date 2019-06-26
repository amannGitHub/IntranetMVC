using Intranet.Models;
using Intranet.ViewModels;
using System.Linq;
using System;
using System.Web.Mail;

namespace Intranet.BLL
{
    public static class Emails
    {
        //This will check the IsDev key in Web.config and control the URLBase and Emails output for testing or live system
        private static bool IsDev = @System.Web.Configuration.WebConfigurationManager.AppSettings["IsDev"] == "true";
        private static string DevURL = @System.Web.Configuration.WebConfigurationManager.AppSettings["DevURL"];
        private static string ProdURL = @System.Web.Configuration.WebConfigurationManager.AppSettings["ProdURL"];
        //Email groups
        private static string VideoEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["VideoEmail"];
        private static string WebEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["WebEmail"];
        private static string TechEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["TechEmail"];
        private static string GraphicsEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["GraphicsEmail"];
        private static string EditEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["EditingEmail"];
        private static string WebSupervisor = @System.Web.Configuration.WebConfigurationManager.AppSettings["WebSupervisor"];
        private static string ITSupervisor = @System.Web.Configuration.WebConfigurationManager.AppSettings["ITSupervisor"];
        private static string GraphicsSupervisor = @System.Web.Configuration.WebConfigurationManager.AppSettings["GraphicsSupervisor"];
        private static string VideoSupervisor = @System.Web.Configuration.WebConfigurationManager.AppSettings["VideoSupervisor"];
        private static string EditingSupervisor = @System.Web.Configuration.WebConfigurationManager.AppSettings["EditingSupervisor"];
        //If IsDev is true, use DevURL; otherwise, use ProdURL
        private static string URLBase = IsDev ? DevURL : ProdURL;
        private static string TestEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["TestEmail"];
        //Also add these keys: ConferenceReviewSupervisors (Denise and Susie) and actual EditingSupervisors (Alicia and Maria)

        internal static void SendEmail(string Message, string Subject, string Recipient, string CC, string From, MailPriority Priority)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                var body = "<p>Email From: {0}</p><p>{1}</p>";

                var mail = new MailMessage();
                mail.From = From;
                mail.To = Recipient;
                mail.Cc = CC;
                mail.Subject = Subject;
                mail.Priority = MailPriority.Normal;
                mail.BodyFormat = MailFormat.Html;
                mail.Body = string.Format(body, From, Message);
                SmtpMail.SmtpServer = "192.168.0.11";
                SmtpMail.Send(mail);
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "Email Sending");
            }

        }
        
        #region Web Requests

        internal static void WebRequestSubmitted(int? RequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                WebRequestsViewModel sRequest = db.Database.SqlQuery<WebRequestsViewModel>("EXEC [DBO].[spGetWebRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Web Support Request has been submitted.<br><br>Project Code: " + sRequest.projnum 
                    + "<br>Work Type: " + sRequest.WorkType 
                    + "<br>Date: " + sRequest.RequestDate.Value.ToShortDateString() + " " + sRequest.RequestDate.Value.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "WebRequests/Details/" + sRequest.WebRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + " has submitted a Web Support Request.<br><br>Project Code: " + sRequest.projnum 
                    + "<br>Work Type: " + sRequest.WorkType 
                    + "<br>Date: " + sRequest.RequestDate.Value.ToShortDateString() + " " + sRequest.RequestDate.Value.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='"+ URLBase + "WebRequests/ManageRequest/" + sRequest.WebRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email defined above, switch it if it's not you while testing
                    SendEmail(MessageToUser, "Your SCG Web Request", EmployeeEmail, TestEmail, WebEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Web Request for " + sEmployeeName + " - " + sRequest.RequestDate.Value.ToShortDateString()
                        + " - " + sRequest.projnum, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Web Request", EmployeeEmail, "", WebEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Web Request for " + sEmployeeName + " - " + sRequest.RequestDate.Value.ToShortDateString()
                        + " - " + sRequest.projnum, WebEmail, WebSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "WebRequestSubmitted");
            }
        }

        internal static void WebRequestInProgress(int? RequestID, string RequestDescription, int? emp_id, int? assigneeID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                //Requestor
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                //Assigned employee
                Employees aEmployee = db.Employee.Where(x => x.emp_id == assigneeID).FirstOrDefault();
                string aEmployeeName = aEmployee.emp_fname + " " + aEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                string AssigneeEmail = aEmployee.network_id + "@scgcorp.com";

                WebRequestsViewModel sRequest = db.Database.SqlQuery<WebRequestsViewModel>("EXEC [DBO].[spGetWebRequest] {0}", RequestID).First();

                //Message to requestor
                string MessageToUser = sEmployeeName + ", your Web Support Request is in progress.<br><br>Project Code: " + sRequest.projnum 
                    + "<br>Work Type: " + sRequest.WorkType 
                    + "<br>Date: " + sRequest.RequestDate.Value.ToShortDateString() + " " + sRequest.RequestDate.Value.ToShortTimeString() 
                    + "<br>Assigned to: " + aEmployeeName 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "WebRequests/Details/" + sRequest.WebRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                //Message to group
                string MessageToTechs = sEmployeeName + " Web Support Request in progress.<br><br>Project Code: " + sRequest.projnum 
                    + "<br>Work Type: " + sRequest.WorkType 
                    + "<br>Date: " + sRequest.RequestDate.Value.ToShortDateString() + " " + sRequest.RequestDate.Value.ToShortTimeString() 
                    + "<br>Assigned to: " + aEmployeeName 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "WebRequests/ManageRequest/" + sRequest.WebRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Web Request In Progress", EmployeeEmail, TestEmail, WebEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Web Request In Progress: " + sEmployeeName + " - " + sRequest.RequestDate.Value.ToShortDateString()
                        + " - " + sRequest.projnum, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Web Request In Progress", EmployeeEmail, AssigneeEmail, WebEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Web Request in progress for " + sEmployeeName + " - " + sRequest.RequestDate.Value.ToShortDateString()
                        + " - " + sRequest.projnum, WebEmail, WebSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "WebRequestInProgress");
            }
        }

        internal static void WebRequestCompleted(int? RequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                WebRequestsViewModel sRequest = db.Database.SqlQuery<WebRequestsViewModel>("EXEC [DBO].[spGetWebRequest] {0}", RequestID).First();

                string Message = sEmployeeName + ", your Web Support Request has been completed.<br><br>Project Code: " + sRequest.projnum 
                    + "<br>Work Type: " + sRequest.WorkType 
                    + "<br>Date Requested: " + sRequest.RequestDate.Value.ToShortDateString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "WebRequests/Edit/" + sRequest.WebRequestID 
                    + "'>Please click this link to approve your request</a>.";

                if (IsDev)
                {
                    SendEmail(Message, "Your SCG IT Request is Complete", EmployeeEmail, TestEmail, WebEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(Message, "Your SCG IT Request is Complete", EmployeeEmail, WebSupervisor, WebEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "WebRequestCompleted");
            }
        }      

        #endregion

        #region Leave requests

        internal static void SendLeaveRequest(int emp_id, string LeaveStart, string LeaveEnd, Single Hours,
            int LeaveType, string Reason, int LeaveRequestID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                string CC = userName + "@scgcorp.com";
                string sLeaveType = "";
                string sAvailableHours = "";

                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                Employees mSupervisor = db.Employee.Where(x => x.emp_id == mEmployee.supervisor_id).FirstOrDefault();

                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string Recipient = mSupervisor.network_id + "@scgcorp.com";

                //If the leave type is Sick or Vacation, the email will go to Stacy and CC the manager; otherwise, the email will go to Ava for approval, then go to Stacy and the manager
                switch (LeaveType)
                {
                    case 1:
                        sLeaveType = "Sick";
                        sAvailableHours = mEmployee.sickhours.ToString();
                        Recipient += "; " + @System.Web.Configuration.WebConfigurationManager.AppSettings["LeaveManager"];
                        break;
                    case 2:
                        sLeaveType = "Vacation";
                        sAvailableHours = mEmployee.vacationhours.ToString();
                        Recipient += "; " + @System.Web.Configuration.WebConfigurationManager.AppSettings["LeaveManager"];
                        break;
                    case 3:
                        sLeaveType = "Other";
                        sAvailableHours = mEmployee.vacationhours.ToString();
                        //Ava gets the email from the conditional below the message
                        break;
                }

                string Message = sEmployeeName + " has submitted a request for " + sLeaveType + " leave.<br><br>" 
                    + "Start: " + LeaveStart 
                    + "<br>End: " + LeaveEnd 
                    + "<br>Total Hours: " + Hours 
                    + "<br>Available Hours: " + sAvailableHours 
                    + "<br>Reason: " + Reason 
                    + "<br><br><a href='" + URLBase + "LeaveRequests/ManageRequest/'" + LeaveRequestID 
                    + ">Please click this link to manage the request</a>.";

                if (LeaveType == 3)
                {
                    if (IsDev)
                    {
                        SendEmail(Message, "SCG Leave Request for " + sEmployeeName, TestEmail, "", CC, MailPriority.Normal);
                    }
                    else
                    {
                        SendEmail(Message, "SCG Leave Request for " + sEmployeeName, Recipient, @System.Web.Configuration.WebConfigurationManager.AppSettings["HRSuper"], CC, MailPriority.Normal);
                    }
                }
                else
                {
                    if (IsDev)
                    {
                        SendEmail(Message, "SCG Leave Request for " + sEmployeeName, TestEmail, CC, CC, MailPriority.Normal);
                    }
                    else
                    {
                        SendEmail(Message, "SCG Leave Request for " + sEmployeeName, Recipient, CC, CC, MailPriority.Normal);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "SendLeaveRequest");
            }
        }

        internal static void DeleteLeaveRequest(int emp_id, string LeaveStart, string LeaveEnd, Single Hours,
            int LeaveType, string Reason, int LeaveRequestID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            //this is used whether the employee or manager deleted the request

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                string CC = userName + "@scgcorp.com";
                string sLeaveType = "";

                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                Employees mSupervisor = db.Employee.Where(x => x.emp_id == mEmployee.supervisor_id).FirstOrDefault();

                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string Recipient = mSupervisor.network_id + "@scgcorp.com";

                //TODO remove hardcoding in favor of getting it from DB? Tried to avoid a DB hit
                switch (LeaveType)
                {
                    case 1:
                        sLeaveType = "Sick";
                        break;
                    case 2:
                        sLeaveType = "Vacation";
                        break;
                    case 3:
                        sLeaveType = "Other";
                        break;
                }

                string Message = sEmployeeName + "'s leave request for " + sLeaveType + " leave has been deleted.<br><br>" 
                    + "Start: " + LeaveStart 
                    + "<br>End: " + LeaveEnd 
                    + "<br>Total Hours: " + Hours;

                if (LeaveType == 3)
                {
                    if (IsDev)
                    {
                        SendEmail(Message, "SCG Deleted Leave Request for " + sEmployeeName, TestEmail, @System.Web.Configuration.WebConfigurationManager.AppSettings["HRSuper"], CC, MailPriority.Normal);
                    }
                    else
                    {
                        SendEmail(Message, "SCG Deleted Leave Request for " + sEmployeeName, Recipient, @System.Web.Configuration.WebConfigurationManager.AppSettings["HRSuper"], CC, MailPriority.Normal);
                    }
                }
                else
                {
                    if (IsDev)
                    {
                        SendEmail(Message, "SCG Deleted Leave Request for " + sEmployeeName, TestEmail, CC, CC, MailPriority.Normal);
                    }
                    else
                    {
                        SendEmail(Message, "SCG Deleted Leave Request for " + sEmployeeName, Recipient, CC, CC, MailPriority.Normal);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "DeleteLeaveRequest");
            }
        }

        internal static void ManageLeaveRequest(int emp_id, string LeaveStart, string LeaveEnd, Single Hours,
            int LeaveType, string Reason, int LeaveRequestID, string Status)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                string CC = userName + "@scgcorp.com";
                string sLeaveType = "";

                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                Employees mSupervisor = db.Employee.Where(x => x.emp_id == mEmployee.supervisor_id).FirstOrDefault();

                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                string Recipient = mEmployee.network_id + "@scgcorp.com";

                //TODO remove hardcoding in favor of getting it from DB? Tried to avoid a DB hit
                switch (LeaveType)
                {
                    case 1:
                        sLeaveType = "Sick";
                        break;
                    case 2:
                        sLeaveType = "Vacation";
                        break;
                    case 3:
                        sLeaveType = "Other";
                        break;
                }

                string Message = sEmployeeName + ", your request for " + Hours + " hours of " + sLeaveType + " leave has been " + Status 
                    + ".<br><br>" 
                    + "Start: " + LeaveStart 
                    + "<br>End: " + LeaveEnd 
                    + "<br>Total Hours Requested: " + Hours 
                    + "<br>Reason: " + Reason + "<br><br>";

                if (LeaveType == 3)
                {
                    if (IsDev)
                    {
                        SendEmail(Message, "SCG Leave Request Update for " + sEmployeeName, TestEmail, @System.Web.Configuration.WebConfigurationManager.AppSettings["HRSuper"], CC, MailPriority.Normal);
                    }
                    else
                    {
                        SendEmail(Message, "SCG Leave Request Update for " + sEmployeeName, Recipient, @System.Web.Configuration.WebConfigurationManager.AppSettings["HRSuper"], CC, MailPriority.Normal);
                    }
                }
                else
                {
                    if (IsDev)
                    {
                        SendEmail(Message, "SCG Leave Request Update for " + sEmployeeName, TestEmail, CC, CC, MailPriority.Normal);
                    }
                    else
                    {
                        SendEmail(Message, "SCG Leave Request Update for " + sEmployeeName, Recipient, CC, CC, MailPriority.Normal);
                    }
                }

                
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ManageLeaveRequest");
            }
        }

        #endregion

        #region IT Support

        internal static void ITRequestSubmitted(int? ITRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                ITRequestsViewModel sRequest = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spGetITRequest] {0}", ITRequestID).First();

                string MessageToUser = sEmployeeName + ", your IT Support Request has been submitted.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "ITRequests/Details/" + sRequest.ITRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + " has submitted an IT Support Request.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "ITRequests/ManageRequest/" + sRequest.ITRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG IT Request", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG IT Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.ProjectCode, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG IT Request", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG IT Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.ProjectCode, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ITRequestSubmitted");
            }
        }

        internal static void ITRequestEdited(int? ITRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                ITRequestsViewModel sRequest = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spGetITRequest] {0}", ITRequestID).First();

                string MessageToTechs = sEmployeeName + " has updated an IT Support Request.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "ITRequests/ManageRequest/" + sRequest.ITRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, "Updated SCG IT Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.ProjectCode, EmployeeEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, "Updated SCG IT Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.ProjectCode, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ITRequestEdited");
            }
        }

        internal static void ITRequestDeleted(string MessageToTechs, string Subject, string EmployeeEmail)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, Subject, TechEmail, ITSupervisor + "; " + EmployeeEmail, TechEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ITRequestDeleted");
            }
        }

        internal static void AssignITRequest(int? ITRequestID, string RequestDescription, int? emp_id, int? assigneeID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                //Requestor
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                //Assignee
                Employees aEmployee = db.Employee.Where(x => x.emp_id == assigneeID).FirstOrDefault();
                string aEmployeeName = aEmployee.emp_fname + " " + aEmployee.emp_lname;

                string RequestorEmail = mEmployee.network_id + "@scgcorp.com";
                string AssigneeEmail = aEmployee.network_id + "@scgcorp.com";

                ITRequestsViewModel sRequest = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spGetITRequest] {0}", ITRequestID).First();

                //Message to group
                string Message = "An IT Support Request has been assigned.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Assigned to: " + aEmployeeName 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "ITRequests/ManageRequest/" + sRequest.ITRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                //Send an email to the requestor as well
                string messageToRequestor = "Your IT Support request has been assigned.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Assigned to: " + aEmployeeName 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "ITRequests/Details/" + sRequest.ITRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(Message, "Assigned SCG IT Request: " + sRequest.EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.ProjectCode, TestEmail, "", AssigneeEmail, MailPriority.Normal);
                    SendEmail(messageToRequestor, "Your SCG IT Request has been assigned", RequestorEmail, TestEmail, TechEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(Message, "You've Been Assigned an SCG IT Request for " + sRequest.EmployeeName + " - " +sRequest.CreatedDate.ToShortDateString() + " - " + sRequest.ProjectCode, AssigneeEmail, ITSupervisor, TechEmail, MailPriority.Normal);
                    SendEmail(messageToRequestor, "Your SCG IT Request has been assigned", RequestorEmail, AssigneeEmail, TechEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "AssignITRequest");
            }
        }

        internal static void ITRequestCompleted(int? ITRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                ITRequestsViewModel sRequest = db.Database.SqlQuery<ITRequestsViewModel>("EXEC [DBO].[spGetITRequest] {0}", ITRequestID).First();

                string Message = sEmployeeName + ", your IT Support Request has been completed.<br><br>Project Code: " + sRequest.ProjectCode 
                    + "<br>Request Type: " + sRequest.ITRequestType 
                    + "<br>Date Requested: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Description: " + RequestDescription
                    //I think link below should point to the Details view, not sure if ManageRequest redirects if the request is completed
                    //+ "<br><br><a href='" + URLBase + "ITRequests/ManageRequest/" + sRequest.ITRequestID 
                    + "<br><br><a href='" + URLBase + "ITRequests/Details/" + sRequest.ITRequestID
                    + "'>Please click this link to view your request</a>.";

                if (IsDev)
                {
                    SendEmail(Message, "Your SCG IT Request is Complete", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(Message, "Your SCG IT Request is Complete", EmployeeEmail, ITSupervisor, TechEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ITRequestCompleted");
            }
        }

        #endregion

        #region Video Requests

        internal static void VideoRequestSubmitted(int? VideoRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                VideoRequestsViewModel sRequest = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spGetVideoRequest] {0}", VideoRequestID).First();

                string MessageToUser = sEmployeeName + ", your Video Support Request has been submitted.<br><br>Request Number: " + sRequest.VideoRequestID 
                    + "<br>Video Type: " + sRequest.VideoType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "VideoRequests/Details/" + sRequest.VideoRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + " has submitted a Video Support Request.<br><br>Charge Code: " + sRequest.projnum 
                    + "<br>Request Type: " + sRequest.VideoType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "VideoRequests/ManageRequest/" + sRequest.VideoRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Video Request", EmployeeEmail, TestEmail, VideoEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.VideoRequestID, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Video Request", EmployeeEmail, "", VideoEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.VideoRequestID, VideoEmail, VideoSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "VideoRequestSubmitted");
            }
        }

        internal static void VideoRequestInProgress(int? VideoRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                //Employees aEmployee = db.Employee.Where(x => x.emp_id == AssigneeID).FirstOrDefault();
                //string aEmployeeName = aEmployee.emp_fname + " " + aEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                //string AssigneeEmail = aEmployee.network_id + "@scgcorp.com";

                VideoRequestsViewModel sRequest = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spGetVideoRequest] {0}", VideoRequestID).First();

                string MessageToRequestor = sEmployeeName + ", your Video Support Request is in progress.<br><br>Request Number: " + sRequest.VideoRequestID 
                    + "<br>Video Type: " + sRequest.VideoType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    //+ "<br>Assigned to: " + aEmployeeName +
                    + "<br>Description: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "VideoRequests/Details/" + sRequest.VideoRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = "Video Support Request in progress.<br><br>Request #: " + sRequest.VideoRequestID 
                    + "<br/>Charge Code: " + sRequest.projnum 
                    + "<br>Request Type: " + sRequest.VideoType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    //+ "<br>Assigned to: " + aEmployeeName 
                    + "<br>Description: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "VideoRequests/ManageRequest/" + sRequest.VideoRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, "SCG Video Request in Progress", EmployeeEmail, TestEmail, VideoEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Assigned SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                    + " - " + sRequest.VideoRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToRequestor, "Your SCG Video Request is In Progress", EmployeeEmail, "", VideoEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Assigned SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                    + " - " + sRequest.VideoRequestID, VideoEmail, VideoSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "VideoRequestSubmitted");
            }
        }

        internal static void VideoRequestEdited(int? VideoRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                VideoRequestsViewModel sRequest = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spGetVideoRequest] {0}", VideoRequestID).First();

                string MessageToTechs = "A Video Request has been updated.<br/><br/>Request #: " + sRequest.VideoRequestID 
                    + "<br><br>Charge Code: " + sRequest.projnum 
                    + "<br>Video Type: " + sRequest.VideoType 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "VideoRequests/ManageRequest/" + sRequest.VideoRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, "Updated SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.VideoRequestID, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, "Updated SCG Video Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.VideoRequestID, VideoEmail, VideoSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "VideoRequestEdited");
            }
        }

        internal static void VideoRequestDeleted(string MessageToTechs, string Subject, string EmployeeEmail)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, TestEmail, VideoEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, VideoEmail, VideoSupervisor, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "VideoRequestDeleted");
            }
        }

        internal static void VideoRequestCompleted(int? VideoRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                VideoRequestsViewModel sRequest = db.Database.SqlQuery<VideoRequestsViewModel>("EXEC [DBO].[spGetVideoRequest] {0}", VideoRequestID).First();

                string Message = sEmployeeName + ", your Video Support Request has been completed.<br><br>Charge Code: " + sRequest.projnum 
                    + "<br>Video Type: " + sRequest.VideoType 
                    + "<br>Date Requested: " + sRequest.CreatedDate.ToShortDateString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "VideoRequests/Edit/" + sRequest.VideoRequestID 
                    + "'>Please click this link to approve your request</a>.";

                if (IsDev)
                {
                    SendEmail(Message, "Your SCG Video Request is Complete", EmployeeEmail, TestEmail, VideoEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(Message, "Your SCG Video Request is Complete", EmployeeEmail, VideoSupervisor, VideoEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "VideoRequestCompleted");
            }
        }

        #endregion

        #region Graphics Requests

        internal static void GraphicsRequestSubmitted(int? GraphicsRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                GraphicsRequestsViewModel sRequest = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spGetGraphicsRequest] {0}", GraphicsRequestID).First();

                string MessageToUser = sEmployeeName + ", your Graphics Support Request has been submitted.<br><br>Request Number: " + sRequest.GraphicsRequestID 
                    + "<br>Project Title: " + sRequest.ProjectTitle 
                    + "<br>Request Date: " + sRequest.RequestDate.ToShortDateString() + " " + sRequest.RequestDate.ToShortTimeString() 
                    + "<br>Draft Due Date: " + sRequest.DraftDueDate.ToShortDateString() + " " + sRequest.DraftDueDate.ToShortTimeString() 
                    + "<br>Final Due Date: " + sRequest.FinalDueDate.ToShortDateString() + " " + sRequest.FinalDueDate.ToShortTimeString()
                    + "<br>Instructions: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "GraphicsRequests/Details/" + sRequest.GraphicsRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToDesigners = sEmployeeName + ", your Graphics Support Request has been submitted.<br><br>Request Number: " + sRequest.GraphicsRequestID 
                    + "<br>Project Title: " + sRequest.ProjectTitle 
                    + "<br>Request Date: " + sRequest.RequestDate.ToShortDateString() + " " + sRequest.RequestDate.ToShortTimeString() 
                    + "<br>Draft Due Date: " + sRequest.DraftDueDate.ToShortDateString() + " " + sRequest.DraftDueDate.ToShortTimeString() 
                    + "<br>Final Due Date: " + sRequest.FinalDueDate.ToShortDateString() + " " + sRequest.FinalDueDate.ToShortTimeString()
                    + "<br>Instructions: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "GraphicsRequests/Details/" + sRequest.GraphicsRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Graphics Request", EmployeeEmail, TestEmail, GraphicsEmail, MailPriority.Normal);
                    SendEmail(MessageToDesigners, "SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                        + " - " + sRequest.GraphicsRequestID, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Graphics Request", EmployeeEmail, "", GraphicsEmail, MailPriority.Normal);
                    SendEmail(MessageToDesigners, "SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                        + " - " + sRequest.GraphicsRequestID, GraphicsEmail, GraphicsSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "GraphicsRequestSubmitted");
            }
        }

        internal static void GraphicsRequestAssigned(int? GraphicsRequestID, string RequestDescription, int? emp_id, int? AssigneeID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;
                Employees aEmployee = db.Employee.Where(x => x.emp_id == AssigneeID).FirstOrDefault();
                string aEmployeeName = aEmployee.emp_fname + " " + aEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                string AssigneeEmail = aEmployee.network_id + "@scgcorp.com";

                GraphicsRequestsViewModel sRequest = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spGetGraphicsRequest] {0}", GraphicsRequestID).First();

                string MessageToUser = sEmployeeName + ", your Graphics Support Request has been assigned.<br><br>Request Number: " + sRequest.GraphicsRequestID 
                    + "<br>Project Title: " + sRequest.ProjectTitle 
                    +  "<br>Assigned to: " + aEmployeeName 
                    + "<br>Request Date: " + sRequest.RequestDate.ToShortDateString() + " " + sRequest.RequestDate.ToShortTimeString() 
                    + "<br>Draft Due Date: " + sRequest.DraftDueDate.ToShortDateString() + " " + sRequest.DraftDueDate.ToShortTimeString() 
                    + "<br>Final Due Date: " + sRequest.FinalDueDate.ToShortDateString() + " " + sRequest.FinalDueDate.ToShortTimeString()
                    + "<br>Instructions: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "GraphicsRequests/Details/" + sRequest.GraphicsRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToDesigners = "A Graphics Support Request has been assigned.<br><br>Request Number: " + sRequest.GraphicsRequestID 
                    + "<br>Project Title: " + sRequest.ProjectTitle 
                    + "<br>Assigned to: " + aEmployeeName 
                    + "<br>Request Date: " + sRequest.RequestDate.ToShortDateString() + " " + sRequest.RequestDate.ToShortTimeString() 
                    + "<br>Draft Due Date: " + sRequest.DraftDueDate.ToShortDateString() + " " + sRequest.DraftDueDate.ToShortTimeString() 
                    + "<br>Final Due Date: " + sRequest.FinalDueDate.ToShortDateString() + " " + sRequest.FinalDueDate.ToShortTimeString()
                    + "<br>Instructions: " + RequestDescription.Trim() 
                    + "<br><br><a href='" + URLBase + "GraphicsRequests/Details/" + sRequest.GraphicsRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Graphics Request", EmployeeEmail, TestEmail, GraphicsEmail, MailPriority.Normal);
                    SendEmail(MessageToDesigners, "Assigned SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                        + " - " + sRequest.GraphicsRequestID, TestEmail, "", EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Graphics Request has been Assigned", EmployeeEmail, AssigneeEmail, GraphicsEmail,MailPriority.Normal);
                    SendEmail(MessageToDesigners, "Assigned SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                        + " - " + sRequest.GraphicsRequestID, GraphicsEmail, GraphicsSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "GraphicsRequestSubmitted");
            }
        }

        internal static void GraphicsRequestEdited(int? GraphicsRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                GraphicsRequestsViewModel sRequest = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spGetGraphicsRequest] {0}", GraphicsRequestID).First();

                string MessageToDesigners = sEmployeeName + " has updated an Graphics Support Request.<br><br>Project Title: " + sRequest.ProjectTitle 
                    + "<br>Draft Due Date: " + sRequest.DraftDueDate.ToShortDateString() + " " + sRequest.DraftDueDate.ToShortTimeString() 
                    + "<br>Date: " + sRequest.RequestDate.ToShortDateString() 
                    + "<br>Instructions: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "GraphicsRequests/ManageRequest/" + sRequest.GraphicsRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToDesigners, "Updated SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                        + " - " + sRequest.GraphicsRequestID, EmployeeEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToDesigners, "Updated SCG Graphics Request for " + sEmployeeName + " - " + sRequest.RequestDate.ToShortDateString()
                        + " - " + sRequest.GraphicsRequestID, GraphicsEmail, GraphicsSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "GraphicsRequestEdited");
            }
        }

        internal static void GraphicsRequestDeleted(string MessageToTechs, string Subject, string EmployeeEmail)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, TestEmail, GraphicsEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, Subject, GraphicsEmail, GraphicsSupervisor + "; " + EmployeeEmail, GraphicsEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "GraphicsRequestDeleted");
            }
        }

        internal static void GraphicsRequestCompleted(int? GraphicsRequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                string GraphicsEmail = "Graphics@scgcorp.com";

                GraphicsRequestsViewModel sRequest = db.Database.SqlQuery<GraphicsRequestsViewModel>("EXEC [DBO].[spGetGraphicsRequest] {0}", GraphicsRequestID).First();

                string Message = sEmployeeName + ", your Graphics Support Request has been completed.<br><br>Project Title: " + sRequest.ProjectTitle 
                    + "<br>Date Requested: " + sRequest.RequestDate.ToShortDateString() 
                    + "<br>Draft Due Date: " + sRequest.DraftDueDate.ToShortDateString() + " " + sRequest.DraftDueDate.ToShortTimeString() 
                    + "<br>Final Due Date: " + sRequest.FinalDueDate.ToShortDateString() + " " + sRequest.FinalDueDate.ToShortTimeString() 
                    + "<br>Instructions: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "GraphicsRequests/Edit/" + sRequest.GraphicsRequestID 
                    + "'>Please click this link to approve your request</a>.";

                if (IsDev)
                {
                    SendEmail(Message, "Your SCG Graphics Request is Complete", EmployeeEmail, TestEmail, GraphicsEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(Message, "Your SCG Graphics Request is Complete", EmployeeEmail, GraphicsSupervisor, GraphicsEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "GraphicsRequestCompleted");
            }
        }

        #endregion

        #region Editing Requests

        internal static void EditingRequestSubmitted(int? RequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", RequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                string MessageToUser = sEmployeeName + ", your Editing Request has been submitted.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + " has submitted an Editing Request. Please assign an editor.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "EditingRequests/ManageRequest/" + sRequest.EditingRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Editing Request", EmployeeEmail, TestEmail, EditEmail, Priority);
                    SendEmail(MessageToTechs, "SCG Editing Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, TestEmail, TestEmail, EmployeeEmail, Priority);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Editing Request", EmployeeEmail, "", EditEmail, Priority);
                    SendEmail(MessageToTechs, "SCG Web Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, EditEmail, EditingSupervisor, EmployeeEmail, Priority);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EditingRequestSubmitted");
            }
        }

        internal static void EditingRequestAssigned(int? RequestID, int? EditorID, int? EditingRequestID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            try
            {
                SolutionDBContext db = new SolutionDBContext();

                //Using drafts, get the EditingRequest to get the emp_id so we can get the person who requested this and notify them
                EditingRequests mRequest = db.EditingRequest.Where(x => x.EditingRequestID == EditingRequestID).FirstOrDefault();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == mRequest.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                //Assignee details
                Employees aEmployee = db.Employee.Where(x => x.emp_id == EditorID).FirstOrDefault();
                string AssigneeEmail = aEmployee.network_id + "@scgcorp.com";
                string AssigneeName = aEmployee.emp_fname + " " + aEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", RequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                string MessageToUser = sEmployeeName + ", your Editing Request has been assigned to an editor.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Assigned to: " + AssigneeName 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToEditors = "Editing Request has been assigned.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Assigned to: " + AssigneeName 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + sRequest.ServiceRequested 
                    + "<br><br><a href='" + URLBase + "EditingRequests/ManageRequest/" + sRequest.EditingRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Editing Request has been Assigned", EmployeeEmail, TestEmail, EditEmail, Priority);
                    SendEmail(MessageToEditors, "Assigned SCG Editing Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, TestEmail, TestEmail, EmployeeEmail, Priority);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Editing Request has been Assigned", EmployeeEmail, "", EditEmail, Priority);
                    SendEmail(MessageToEditors, "Assigned SCG Editing Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, EditEmail, EditingSupervisor, EmployeeEmail, Priority);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EditingRequestAssigned");
            }
        }

        internal static void EditingRequestCompleted(int? RequestID, string RequestDescription, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", RequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                //Might have to change the link to EditingRequests - 6/23/2017 @ 3:40 pm
                string MessageToUser = sEmployeeName + ", your Editing Request has been completed.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = "Editing Request has been completed.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + RequestDescription 
                    + "<br><br><a href='" + URLBase + "EditingRequests/ManageRequest/" + sRequest.EditingRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Editing Request is Complete", EmployeeEmail, TestEmail, EditEmail, Priority);
                    SendEmail(MessageToTechs, "Completed SCG Editing Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, TestEmail, TestEmail, EmployeeEmail, Priority);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Editing Request is Complete", EmployeeEmail, "", EditEmail, Priority);
                    SendEmail(MessageToTechs, "Completed SCG Editing Request for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, EditEmail, EditingSupervisor, EmployeeEmail, Priority);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EditingRequestCompleted");
            }
        }

        internal static void EditingRequestDeleted(string MessageToTechs, string Subject, string EmployeeEmail)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, TestEmail, EditEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, EditEmail, EditingSupervisor, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EditingRequestDeleted");
            }
        }

        internal static void DraftEdited(int? EditingRequestID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            //email only goes to editorial reviewer (and Maria/Alicia?)

            try
            {
                SolutionDBContext db = new SolutionDBContext();

                //Using drafts, get the EditingRequest to get the emp_id so we can get the person who requested this and notify them
                EditingRequests mRequest = db.EditingRequest.Where(x => x.EditingRequestID == EditingRequestID).FirstOrDefault();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == mRequest.emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                //Editorial Reviewer details
                //there's a better way to do this using joins but no time right now. Fix later
                Drafts mDraft = db.Draft.Where(x => x.EditingRequestID == mRequest.EditingRequestID).FirstOrDefault();
                Employees aEmployee = db.Employee.Where(x => x.emp_id == mDraft.EditorialReviewerID).FirstOrDefault();
                string AssigneeEmail = aEmployee.network_id + "@scgcorp.com";
                string AssigneeName = aEmployee.emp_fname + " " + aEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", EditingRequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                string MessageToUser = sEmployeeName + ", a draft of your Editing Request has been marked complete by the editor and is now under Editorial Review.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Editorial Reviewer: " + AssigneeName 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString()  
                    + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToEditors = "The draft of an Editing Request has been marked complete. You have been assigned as Editorial Reviewer.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Editorial Reviewer: " + AssigneeName 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Description: " + sRequest.ServiceRequested 
                    + "<br><br><a href='" + URLBase + "EditingRequests/ManageRequest/" + sRequest.EditingRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Editing Request Draft has been Edited", EmployeeEmail, TestEmail, EditEmail, Priority);
                    SendEmail(MessageToEditors, "Review an SCG Editing Request draft for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, TestEmail, TestEmail, EmployeeEmail, Priority);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Editing Request Draft has been Edited", EmployeeEmail, "", EditEmail, Priority);
                    SendEmail(MessageToEditors, "Review an SCG Editing Request draft for " + sEmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, EditEmail, EditingSupervisor, EmployeeEmail, Priority);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "DraftCompleted");
            }
        }

        internal static void SubmitterReviewCompleted(int? EditingRequestID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            //email only goes to employee, QAReviewer

            try
            {
                SolutionDBContext db = new SolutionDBContext();

                EditingRequests mRequest = db.EditingRequest.Where(x => x.EditingRequestID == EditingRequestID).FirstOrDefault();
                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", EditingRequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                //Requestor details
                Employees mEmployee = db.Employee.Where(x => x.emp_id == mRequest.emp_id).FirstOrDefault();
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                string EmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                //QA details
                Employees mQAReviewer = db.Employee.Where(x => x.emp_id == mRequest.QAReviewerID).FirstOrDefault();
                string QAReviewerEmail = mQAReviewer.network_id + "@scgcorp.com";
                string QAReviewerName = mQAReviewer.emp_fname + " " + mQAReviewer.emp_lname;

                string AdminEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["EditingSupervisorDefault"];  //defaults to this

                string MessagetoSubmitter = EmployeeName + "You have approved the final draft of your Editing Request. The QA/QC Reviewer will do final approval.<br><br>Project Code: " + sRequest.Project
                        + "<br>QA/QC Reviewer: " + QAReviewerName
                        + "<br>Service Requested: " + sRequest.ServiceRequested
                        + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString()
                        + "<br><br><a href='" + URLBase + "EditingRequests/Edit/" + sRequest.EditingRequestID
                        + "'>Please click this link to view your request or check the status</a>.";

                string MessagetoTarget = "Please QA/QC an Editing Request.<br><br>Project Code: " + sRequest.Project
                    + "<br>QA/QC Reviewer: " + QAReviewerName
                    + "<br>Service Requested: " + sRequest.ServiceRequested
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString()
                    + "<br><br><a href='" + URLBase + "EditingRequests/ManageRequest/" + sRequest.EditingRequestID
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    SendEmail(MessagetoSubmitter, "Your SCG Editing Request is Approved", EmployeeEmail, TestEmail, AdminEmail, Priority);
                    SendEmail(MessagetoTarget, "QA/QC Review an Editing Request for " + EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, TestEmail, TestEmail, EmployeeEmail, Priority);
                }
                else
                {
                    SendEmail(MessagetoSubmitter, "Your SCG Editing Request is Approved", EmployeeEmail, "", AdminEmail, Priority);
                    SendEmail(MessagetoTarget, "QA/QC Review an Editing Request for " + EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, AdminEmail, "", EmployeeEmail, Priority);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "SubmitterReviewCompleted");
            }
        }

        internal static void DraftReviewCompleted(int? EditingRequestID, int? DraftID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            //email only goes to employee, editor (to let them know work is done), and Maria/Alicia?

            try
            {
                SolutionDBContext db = new SolutionDBContext();

                Drafts mDraft = db.Draft.Where(x => x.DraftID == DraftID).FirstOrDefault();
                EditingRequests mRequest = db.EditingRequest.Where(x => x.EditingRequestID == EditingRequestID).FirstOrDefault();
                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", EditingRequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                //Requestor details
                Employees mEmployee = db.Employee.Where(x => x.emp_id == mRequest.emp_id).FirstOrDefault();
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                string EmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                //Editor details
                Employees mEditor = db.Employee.Where(x => x.emp_id == mDraft.EditorID).FirstOrDefault();
                string EditorEmail = mEditor.network_id + "@scgcorp.com";
                string EditorName = mEditor.emp_fname + " " + mEditor.emp_lname;

                //Editorial Reviewer details
                Employees mReviewer = db.Employee.Where(x => x.emp_id == mDraft.EditorialReviewerID).FirstOrDefault();
                string ReviewerEmail = mReviewer.network_id + "@scgcorp.com";
                string ReviewerName = mReviewer.emp_fname + " " + mReviewer.emp_lname;

                string AdminEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["EditingSupervisorDefault"];  //defaults to this
                string MessagetoTarget = "";
                string MessagetoSubmitter = "";

                if (mDraft.FinalDraft)
                {
                    MessagetoSubmitter = EmployeeName + ", your Editing Request is ready for your review.<br><br>Project Code: " + sRequest.Project
                        + "<br>Service Requested: " + sRequest.ServiceRequested
                        + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString()
                        + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID
                        + "'>Please click this link to view your request or check the status</a>.";

                    if (IsDev)
                    {
                        SendEmail(MessagetoSubmitter, "Your SCG Editing Request is ready for your review", TestEmail, "", AdminEmail, Priority);
                    }
                    else
                    {
                        SendEmail(MessagetoSubmitter, "Your SCG Editing Request is ready for your review", EmployeeEmail, "", AdminEmail, Priority);
                    }
                }
                else
                {
                    #region

                    MessagetoSubmitter = EmployeeName + ", the editorial review of a draft of your Editing Request has been marked complete. Other drafts may be required. You will be notified of next steps.<br><br>Project Code: " + sRequest.Project 
                        + "<br>Editorial Reviewer: " + ReviewerEmail 
                        + "<br>Service Requested: " + sRequest.ServiceRequested 
                        + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString()  
                        + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                        + "'>Please click this link to view your request or check the status</a>.";

                    MessagetoTarget = "The editorial review of draft of an Editing Request has been marked complete.<br><br>Project Code: " + sRequest.Project 
                        + "<br>Editorial Reviewer: " + ReviewerEmail 
                        + "<br>Service Requested: " + sRequest.ServiceRequested 
                        + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString()  
                        + "<br><br><a href='" + URLBase + "EditingRequests/ManageRequest/" + sRequest.EditingRequestID 
                        + "'>Please click this link to manage or view the request</a>.";

                    if (IsDev)
                    {
                        SendEmail(MessagetoSubmitter, "Your SCG Editing Request Draft has been Edited", EmployeeEmail, TestEmail, AdminEmail, Priority);
                        SendEmail(MessagetoTarget, "Editorial Review completed for " + EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                            + " - " + sRequest.Project, TestEmail, TestEmail, EmployeeEmail, Priority);
                    }
                    else
                    {
                        SendEmail(MessagetoSubmitter, "Your SCG Editing Request Draft has been Edited", EmployeeEmail, "", AdminEmail, Priority);
                        SendEmail(MessagetoTarget, "Editorial Review completed for " + EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                            + " - " + sRequest.Project, AdminEmail, "", EmployeeEmail, Priority);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ReviewCompleted");
            }
        }

        internal static void QAQCReviewCompleted(int? EditingRequestID)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start
            MailPriority Priority = MailPriority.Normal;

            //email only goes to employee, editor (to let them know work is done), and Maria/Alicia?

            try
            {
                SolutionDBContext db = new SolutionDBContext();

                EditingRequests mRequest = db.EditingRequest.Where(x => x.EditingRequestID == EditingRequestID).FirstOrDefault();
                EditingRequestsViewModel sRequest = db.Database.SqlQuery<EditingRequestsViewModel>("EXEC [DBO].[spGetEditingRequest] {0}", EditingRequestID).First();

                if (sRequest.RushJob)
                {
                    Priority = MailPriority.High;
                }

                //Requestor details
                Employees mEmployee = db.Employee.Where(x => x.emp_id == mRequest.emp_id).FirstOrDefault();
                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";
                string EmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                Employees mFinalReviewer = db.Employee.Where(x => x.emp_id == mRequest.QAReviewerID).FirstOrDefault();
                string FinalReviewerEmail = mFinalReviewer.network_id + "@scgcorp.com";
                string FinalReviewerName = mFinalReviewer.emp_fname + " " + mFinalReviewer.emp_lname;

                string AdminEmail = @System.Web.Configuration.WebConfigurationManager.AppSettings["EditingSupervisorDefault"];

                string MessagetoSubmitter = EmployeeName + " Final QA/QC Review of your Editing Request is complete.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessagetoTarget = FinalReviewerName + "Final QA/QC Review of an Editing Request is complete.<br><br>Project Code: " + sRequest.Project 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br>Date: " + sRequest.CreatedDate.ToShortDateString() + " " + sRequest.CreatedDate.ToShortTimeString() 
                    + "<br>Service Requested: " + sRequest.ServiceRequested 
                    + "<br><br><a href='" + URLBase + "EditingRequests/Details/" + sRequest.EditingRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                if (IsDev)
                {
                    SendEmail(MessagetoSubmitter, "Your SCG Editing Request is complete", EmployeeEmail, TestEmail, AdminEmail, Priority);
                    SendEmail(MessagetoTarget, "SCG Editing Request is complete for " + EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, TestEmail, "", EmployeeEmail, Priority);
                }
                else
                {
                    SendEmail(MessagetoSubmitter, "Your SCG Editing Request is complete", EmployeeEmail, "", AdminEmail, Priority);
                    SendEmail(MessagetoTarget, "SCG Editing Request is complete for " + EmployeeName + " - " + sRequest.CreatedDate.ToShortDateString()
                        + " - " + sRequest.Project, AdminEmail, "", EmployeeEmail, Priority);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ReviewCompleted");
            }
        }

        #endregion

        #region Equipment Requests

        internal static void EquipmentRequestSubmitted(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Request has been submitted.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + " has submitted an Equipment Request.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Request", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Request", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString(),
                        TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentRequestSubmitted");
            }
        }

        internal static void EquipmentExtensionSubmitted(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Extension has been submitted.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br>Extension Date: " + sRequest.ExtensionDate.Value.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + " has submitted an Equipment Extension.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br>Extension Date: " + sRequest.ExtensionDate.Value.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Extension", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Extension for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Extension", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Extension for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentExtensionSubmitted");
            }
        }

        internal static void EquipmentExtensionRejected(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Extension has been rejected. Please return the equipment on the expected date.<br><br>Date of Request: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + "'s Equipment Extension has been rejected.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Extension is Rejected", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Rejected Equipment Extension for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Extension is Rejected", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Rejected Equipment Extension for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentExtensionRejected");
            }
        }

        internal static void EquipmentExtensionApproved(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Extension has been approved.<br><br>Date of Request: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>New Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + "'s Equipment Extension has been approved.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br>New Request End: " + sRequest.RequestEndDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Extension is Approved", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Approved Equipment Extension for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Extension is Approved", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Approved Equipment Extension for " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentExtensionApproved");
            }
        }

        internal static void EquipmentReturned(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment has been marked returned.<br><br>Date of Request: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + "'s Equipment has been marked returned.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Extension is Marked Returned", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Equipment Returned by " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Extension is Marked Returned", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "Equipment Returned by " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString()
                        + " - #" + sRequest.EquipmentRequestID, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentReturned");
            }
        }

        internal static void EquipmentRejected(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Request has been rejected.<br><br>Date of Request: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + "'s Equipment Request has been rejected.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Request has been rejected", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request has been rejected " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString() + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Request has been rejected", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request has been rejected " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString() + " - #" + sRequest.EquipmentRequestID, WebEmail, EditingSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentRejected");
            }
        }

        internal static void EquipmentApproved(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Request has been approved.<br><br>Date of Request: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + "'s Equipment Request has been approved.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Request has been approved", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request has been approved " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString() + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Request has been approved", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request has been approved " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString() + " - #" + sRequest.EquipmentRequestID, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentApproved");
            }
        }

        internal static void EquipmentUpdated(int? RequestID, int? emp_id)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                SolutionDBContext db = new SolutionDBContext();
                Employees mEmployee = db.Employee.Where(x => x.emp_id == emp_id).FirstOrDefault();
                string sEmployeeName = mEmployee.emp_fname + " " + mEmployee.emp_lname;

                string EmployeeEmail = mEmployee.network_id + "@scgcorp.com";

                EquipmentRequestsViewModel sRequest = db.Database.SqlQuery<EquipmentRequestsViewModel>("EXEC [DBO].[spGetEquipmentRequest] {0}", RequestID).First();

                string MessageToUser = sEmployeeName + ", your Equipment Request has been updated.<br><br>Date of Request: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/Details/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to view your request or check the status</a>.";

                string MessageToTechs = sEmployeeName + "'s Equipment Request has been updated.<br><br>Date: " 
                    + sRequest.RequestSubmittedDate.ToShortDateString() + " " + sRequest.RequestSubmittedDate.ToShortTimeString() 
                    + "<br>Request #" + sRequest.EquipmentRequestID 
                    + "<br>Request Start: " + sRequest.RequestStartDate.ToShortDateString() 
                    + "<br><br><a href='" + URLBase + "EquipmentRequests/ManageRequest/" + sRequest.EquipmentRequestID 
                    + "'>Please click this link to manage or view the request</a>.";

                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToUser, "Your SCG Equipment Request has been updated", EmployeeEmail, TestEmail, TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request has been updated " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString() + " - #" + sRequest.EquipmentRequestID, TestEmail, TestEmail, EmployeeEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToUser, "Your SCG Equipment Request has been updated", EmployeeEmail, "", TechEmail, MailPriority.Normal);
                    SendEmail(MessageToTechs, "SCG Equipment Request has been updated " + sEmployeeName + " - " + sRequest.RequestSubmittedDate.ToShortDateString() + " - #" + sRequest.EquipmentRequestID, TechEmail, ITSupervisor, EmployeeEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "EquipmentUpdated");
            }
        }

        internal static void EquipmentRequestDeleted(string MessageToTechs, string Subject, string EmployeeEmail)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name.Substring(8); //get username and remove SCGCORP\\ from start

            try
            {
                if (IsDev)
                {
                    //Test email addresses
                    SendEmail(MessageToTechs, Subject, EmployeeEmail, TestEmail, TestEmail, MailPriority.Normal);
                }
                else
                {
                    SendEmail(MessageToTechs, Subject, TechEmail, ITSupervisor + "; " + EmployeeEmail, TechEmail, MailPriority.Normal);
                }
            }
            catch (Exception ex)
            {
                Helpers.LogError(ex.Message, ex.StackTrace, userName, "Email Sending", "ITRequestDeleted");
            }
        }

        #endregion
    }
}