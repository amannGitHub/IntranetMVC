using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Clients
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Client #")]
        public int ClientID { get; set; }

        [Required(ErrorMessage = "Client is required")]
        public string Client { get; set; }

        [Display(Name = "Acronym")]
        [Required(ErrorMessage = "Acronym is required")]
        public string Acronym { get; set; }

        public bool Deleted { get; set; }
    }
}

