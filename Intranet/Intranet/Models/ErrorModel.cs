using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Intranet.Models
{
    public class ErrorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorID { get; set; }
        public string Page { get; set; }
        public string Controller { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ErrorTime { get; set; }
        public string UserName { get; set; }
        public string StackTrace { get; set; }
    }
}