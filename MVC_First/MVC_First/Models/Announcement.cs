//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_First.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Announcement
    {
        public int announcement_id { get; set; }
        public string audience { get; set; }
        public string status { get; set; }
        public string Sender_u_id { get; set; }
        public Nullable<System.DateTime> dateTime { get; set; }
        public string attachment { get; set; }
        public string destination { get; set; }
        public string title { get; set; }
        public string mesg_text { get; set; }
    
        public virtual user user { get; set; }
    }
}
