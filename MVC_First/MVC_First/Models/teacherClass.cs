//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_First.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class teacherClass
    {
        public string tid { get; set; }
        public string cid { get; set; }
        public string className { get; set; }
    
        public virtual cours cours { get; set; }
        public virtual teacher teacher { get; set; }
    }
}
