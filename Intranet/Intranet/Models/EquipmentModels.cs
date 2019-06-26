using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models
{
    public class EquipmentModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentModelID { get; set; }

        [Display(Name = "Model")]
        public string EquipmentModel { get; set; }

        [Display(Name = "Make")]
        public int ManufacturerID { get; set; }
    }
}

