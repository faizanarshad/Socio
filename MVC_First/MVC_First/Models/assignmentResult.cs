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
    
    public partial class assignmentResult
    {
        public int aid { get; set; }
        public string sid { get; set; }
        public string cid { get; set; }
        public int totalMarks { get; set; }
        public int marksObtained { get; set; }
        public string outPutFilePath { get; set; }
        public string codeFilePath { get; set; }
        public string comment { get; set; }
        public Nullable<int> aNumber { get; set; }
    
        public virtual assignment assignment { get; set; }
        public virtual student student { get; set; }
        public virtual cours cours { get; set; }
    }
}
