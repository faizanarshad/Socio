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
    
    public partial class teacher
    {
        public teacher()
        {
            this.assignments = new HashSet<assignment>();
            this.attendances = new HashSet<attendance>();
            this.attendance1 = new HashSet<attendance1>();
            this.Logs = new HashSet<Log>();
            this.LogTeachers = new HashSet<LogTeacher>();
            this.quizs = new HashSet<quiz>();
            this.quizResults = new HashSet<quizResult>();
            this.teacherClasses = new HashSet<teacherClass>();
            this.courses = new HashSet<cours>();
        }
    
        public string tid { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string designation { get; set; }
        public string status { get; set; }
        public string qualification { get; set; }
        public string dept { get; set; }
    
        public virtual ICollection<assignment> assignments { get; set; }
        public virtual ICollection<attendance> attendances { get; set; }
        public virtual ICollection<attendance1> attendance1 { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<LogTeacher> LogTeachers { get; set; }
        public virtual ICollection<quiz> quizs { get; set; }
        public virtual ICollection<quizResult> quizResults { get; set; }
        public virtual ICollection<teacherClass> teacherClasses { get; set; }
        public virtual ICollection<cours> courses { get; set; }
    }
}
