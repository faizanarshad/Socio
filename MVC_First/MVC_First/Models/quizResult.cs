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
    
    public partial class quizResult
    {
        public int qid { get; set; }
        public string sid { get; set; }
        public string cid { get; set; }
        public string tid { get; set; }
        public string totalmarks { get; set; }
        public string obtainMarks { get; set; }
        public Nullable<int> attempts { get; set; }
        public Nullable<int> quizNum { get; set; }
    
        public virtual cours cours { get; set; }
        public virtual quiz quiz { get; set; }
        public virtual student student { get; set; }
        public virtual teacher teacher { get; set; }
    }
}
