using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class Holidays
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("holiday_id")]
        public int HolidayID { get; set; }
        
        [Column("holiday")]
        [Required(ErrorMessage = "Holiday is required")]
        public string Holiday { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [Column("holiday_dt")]
        [Required(ErrorMessage = "Holiday is required")]
        public DateTime HolidayDate { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
    }
}

