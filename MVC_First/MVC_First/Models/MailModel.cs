using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_First.Models
{
    public class MailModel
    {
        
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        [Required]
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}