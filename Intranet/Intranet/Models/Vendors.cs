using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Vendors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? VendorID { get; set; }

        [Display(Name = "Vendor")]
        [Required(ErrorMessage = "Vendor name is required")]
        public string VendorName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name = "State")]
        public int StateID { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Website { get; set; }

        public string Services { get; set; }

        public string Contact1Name { get; set; }

        public string Contact2Name { get; set; }

        public string Contact1Email { get; set; }

        public string Contact2Email { get; set; }

        public string Contact1Phone { get; set; }

        public string Contact2Phone { get; set; }
    }
}

