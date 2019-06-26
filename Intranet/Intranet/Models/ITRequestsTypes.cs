using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class ITRequestTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ITRequestTypeID { get; set; }

        [Display(Name = "Request Type")]
        public string ITRequestType { get; set; }

        public virtual ITRequests ITRequest { get; set; }
    }
}

