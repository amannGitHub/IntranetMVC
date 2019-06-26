using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class QAStatuses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QAStatusID { get; set; }

        [Display(Name = "Status")]
        public string QAStatus { get; set; }
    }
}

