using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_First.Models
{
    public class StudentLog
    {
        
        public int ID { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int Obtained_marks { get; set; }
        public int Total_marks { get; set; }
    }
}