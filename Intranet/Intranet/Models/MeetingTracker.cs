using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class MeetingTracker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Meeting #")]
        public int? MeetingID { get; set; }

        [Display(Name = "Project/Task #")]
        [Required(ErrorMessage = "Project is required")]
        public int project_id { get; set; }

        [Display(Name = "Created By")]
        public int? emp_id { get; set; }

        [Display(Name = "Meeting Title")]
        [Required(ErrorMessage = "Meeting Title is required")]
        public string MeetingTitle { get; set; }

        [Display(Name = "Meeting Planner")]
        [Required(ErrorMessage = "Meeting Planner is required")]
        public int MeetingPlannerID { get; set; }

        [Required(ErrorMessage = "Meeting Short Name is required")]
        [Display(Name = "Meeting Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Client")]
        [Required(ErrorMessage = "Client is required")]
        public int? ClientID { get; set; }

        [Required(ErrorMessage = "Meeting Start is required")]
        [Display(Name = "Meeting Start")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime MeetingStartDate { get; set; }

        [Required(ErrorMessage = "Meeting End is required")]
        [Display(Name = "Meeting End")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime MeetingEndDate { get; set; }

        [Display(Name = "Final # of Attendees")]
        public int FinalAttendeeCount { get; set; }

        //Before Location was turned into a drop list
        //[Required(ErrorMessage = "Location is required")]
        //[Display(Name = "Location")]
        //public string Location { get; set; }

        //After Location was turned into a drop list
        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public int LocationID { get; set; }

        [Required(ErrorMessage = "Meeting Purpose is required")]
        [Display(Name = "Meeting Purpose")]
        public string MeetingPurpose { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Problems and Solutions")]
        public string ProblemsAndSolutions { get; set; }

        [Display(Name = "Registration End Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime RegistrationEndDate { get; set; }

        [Display(Name = "Max # of Registrants")]
        public int MaxRegistrants { get; set; }

        [Display(Name = "Fee")]
        public Single RegistrationFee { get; set; }

        [Display(Name = "Home")]
        public bool Home { get; set; }
        public bool Registration { get; set; }
        public bool Logistics { get; set; }
        public bool Agenda { get; set; }
        public bool Abstracts { get; set; }
        public bool Other { get; set; }

        public bool Honorific { get; set; }

        [Display(Name = "Required")]
        public bool HonorificReq { get; set; }

        [Display(Name = "First Name")]
        public bool FirstName { get; set; }

        [Display(Name = "Required")]
        public bool FirstNameReq { get; set; }

        [Display(Name = "Last Name")]
        public bool LastName { get; set; }

        [Display(Name = "Required")]
        public bool LastNameReq { get; set; }

        public bool Degree { get; set; }

        [Display(Name = "Required")]
        public bool DegreeReq { get; set; }

        public bool Title { get; set; }

        [Display(Name = "Required")]
        public bool TitleReq { get; set; }

        public bool Department { get; set; }

        [Display(Name = "Required")]
        public bool DepartmentReq { get; set; }

        public bool Organization { get; set; }

        [Display(Name = "Required")]
        public bool OrganizationReq { get; set; }

        public bool Phone { get; set; }

        [Display(Name = "Required")]
        public bool PhoneReq { get; set; }

        public bool Email { get; set; }

        [Display(Name = "Required")]
        public bool EmailReq { get; set; }

        [Display(Name = "Dietary Restrictions")]
        public bool DietaryRestrictions { get; set; }

        [Display(Name = "Required")]
        public bool DietaryRestrictionsReq { get; set; }

        [Display(Name = "Special Needs")]
        public bool SpecialNeeds { get; set; }

        [Display(Name = "Payment Methods")]
        public bool PaymentMethods { get; set; }

        [Display(Name = "Required")]
        public bool PaymentMethodsReq { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Address")]
        public bool Address1 { get; set; }

        [Display(Name = "Required")]
        public bool Address1Req { get; set; }

        [Display(Name = "Address (Line 2)")]
        public bool Address2 { get; set; }

        [Display(Name = "Required")]
        public bool Address2Req { get; set; }

        public bool City { get; set; }

        [Display(Name = "Required")]
        public bool CityReq { get; set; }

        public bool State { get; set; }

        [Display(Name = "Required")]
        public bool StateReq { get; set; }

        public bool Country { get; set; }

        [Display(Name = "Required")]
        public bool CountryReq { get; set; }

        public bool Zipcode { get; set; }

        [Display(Name = "Required")]
        public bool ZipcodeReq { get; set; }

        public bool Bios { get; set; }

        [Display(Name = "Required")]
        public bool BiosReq { get; set; }

        [Display(Name = "Abstracts")]
        public bool Abstracts1 { get; set; }

        [Display(Name = "Required")]
        public bool Abstracts1Req { get; set; }

        public bool Posters { get; set; }

        [Display(Name = "Required")]
        public bool PostersReq { get; set; }

        [Display(Name = "Deleted?")]
        public bool Deleted { get; set; }

        [Display(Name = "")]
        public string OtherText { get; set; }
    }
}

