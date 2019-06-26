using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class WebsiteNames
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int website_id { get; set; }

        [Display(Name = "Website")]
        public string websitename { get; set; }
    }
}

