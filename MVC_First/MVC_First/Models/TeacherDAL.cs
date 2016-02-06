using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_First.Models;
using MVC_First.Controllers;
using MVC_First.App_Start;
using System.Web.Mvc;
using System.Data.SqlTypes;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace MVC_First.Models
{
    public interface ITeacherDAL
    {
        void SaveQuiz(quiz z, string id);
        void SaveQuestions(string id, string topic, string fileName);
        int verifyCourse(string id);
        List<cours> GetCourses( string tid );
        List<question> GetTopic( string id );
        List<quiz> GetQuizes( string id , string cid);
        quiz GetQuizeForUpdation( int id );
        void SaveUpdatedQuiz( quiz q );
        List<quiz> getQuize(string id , string cid);
        int verifyTotalQ( string totalq , string id);
        List<quizResult> getResult(string id, string cid, int qNo);
        void SaveAssignment( assignment ass, string id );
        List<assignment> getAssignments( string id , string cid);
        int verifyAssignmentNumber(string cid , int aNo,string tid);
        List<assignmentResult> getAssignmentsResult( string tid , string cid , int aNo );
        List<MVC_First.Models.attendance1> getAttendanceView(string n, string c, string class1);
        int markAttendance(string list, string tName, string cName, string date,string classN);
        assignment GetAssignmentForUpdation(int aid);
        
    }
    public class TeacherDAL : Controller,ITeacherDAL
    {
        private PucitDBEntities db = new PucitDBEntities();
        //private puEntities db1 = new puEntities();

        public void SaveQuiz(quiz z, string id)
        {

            teacher t = db.teachers.SingleOrDefault(x => x.tid == id);
            cours c = db.courses.SingleOrDefault(x => x.cid.Equals(z.cid));
            z.tid = id;
            t.quizs.Add(z);
            c.quizs.Add(z);
            db.quizs.Add(z);
            db.SaveChanges();
        }
        public int verifyCourse(string id)
        {
            var c = db.courses.Find(id);
            if (c == null)
                return -1;
            else
                return 1;
        }
        public List<cours> GetCourses( string tid )
        {
            var query = (from x in db.teachers.Where(f => f.tid.Equals(tid))
                         from p in db.courses.Where(g => g.teachers.Contains(x))
                         select p);

            List<cours> data = new List<cours>();

            foreach (var q in query)
            {
                cours c = new cours();
                c.cid = q.cid;
                c.name = q.name;
                data.Add(c);

            }
            return data;

        }
        public List<question> GetTopic( string id )
        {
            var query = from x in db.questions where x.cid == id
                         select x;

            List<question> data = new List<question>();
            List<string> temp = new List<string>();

            foreach (var q in query)
            {
                question c = new question();
                c.topic = q.topic;
                if ( !(temp.Contains(q.topic)) )
                    data.Add(c);
                temp.Add(q.topic);
            }
            return data;

        }
        public quiz GetQuizeForUpdation( int id )
        {
            var query = from x in db.quizs.Where(f => f.qid.Equals(id))
                        select x;
            quiz q = new quiz();
            foreach (var a in query)
            {
                q.qid = a.qid;
                q.tid = a.tid;
                q.cid = a.cid;
                q.topic = a.topic;
                q.TotalMarks = a.TotalMarks;
                q.totalQuestion = a.totalQuestion;
                q.TotalTime = a.TotalTime;
                q.startDate = a.startDate;
                q.endDate = a.endDate;
            }
            return q;
        }
        public assignment GetAssignmentForUpdation( int aid )
        {
            var query = from x in db.assignments.Where(f => f.aid.Equals(aid))
                        select x;
            assignment q = new assignment();
            foreach (var a in query)
            {
                q.aid = a.aid;
                q.tid = a.tid;
                q.cid = a.cid;
                q.aNumber = a.aNumber;
                q.totalMarks = a.totalMarks;
                q.startDate = a.startDate;
                q.endDate = a.endDate;
                q.questionFilePath = a.questionFilePath;
                q.solutionFilePath = a.solutionFilePath;
            }
            return q;
        }
        public void SaveUpdatedQuiz( quiz q)
        {
            db.Entry(q).State = EntityState.Modified;
            
            db.SaveChanges();
        }
        public List<quiz> GetQuizes( string id , string cid)
        {
            var query = (from x in db.quizs where x.tid.Equals(id) && x.cid.Equals(cid)
                         select x);

            DateTime d = DateTime.Now;
            List<quiz> data = new List<quiz> ();

            foreach (var q in query)
            {
                quiz z = new quiz();
                z.cid = q.cid;
                z.qid = q.qid;
                z.totalQuestion = q.totalQuestion;
                z.TotalMarks = q.TotalMarks;
                z.TotalTime = q.TotalTime;
                z.topic = q.topic;
                z.startDate = q.startDate;
                z.endDate = q.endDate;
                z.quizNum = q.quizNum;

                if (z.endDate >= d)
                {
                    data.Add(z);
                }
               
            }
            return data;
        }
        public void SaveQuestions(string id, string topic, string fileName)
        {
            FileStream fin = new FileStream(fileName, FileMode.Open);
            StreamReader sr = new StreamReader(fin);
            question q = new question();
            cours c = db.courses.SingleOrDefault(x => x.cid.Equals(id));

            q.statement = sr.ReadLine();
            q.option_A = sr.ReadLine();
            q.option_B = sr.ReadLine();
            q.Option_C = sr.ReadLine();
            q.option_D = sr.ReadLine();
            q.correctOption = sr.ReadLine();
            q.marks = Convert.ToInt32(sr.ReadLine());
            q.solution = sr.ReadLine();
            q.topic = topic;
            q.cid = id;
            c.questions.Add(q);
            db.questions.Add(q);
            db.SaveChanges();

            string str = sr.ReadLine();

            while (str != null)
            {
                q.statement = str;
                q.option_A = sr.ReadLine();
                q.option_B = sr.ReadLine();
                q.Option_C = sr.ReadLine();
                q.option_D = sr.ReadLine();
                q.correctOption = sr.ReadLine();
                q.marks = Convert.ToInt32(sr.ReadLine());
                q.solution = sr.ReadLine();
                q.topic = topic;
                q.cid = id;
                c.questions.Add(q);
                db.questions.Add(q);
                db.SaveChanges();

                str = sr.ReadLine();
            }
            sr.Close();
            fin.Close();
        }
        public List<quiz> getQuize(string id , string cid )
        {
            var query = from x in db.quizs
                        where (x.tid == id && x.cid == cid )
                         select x;

            DateTime d = DateTime.Now;
            List<quiz> data = new List<quiz>();

            foreach (var q in query)
            {
                quiz z = new quiz();
                z.cid = q.cid;
                z.qid = q.qid;
                z.totalQuestion = q.totalQuestion;
                z.TotalMarks = q.TotalMarks;
                z.TotalTime = q.TotalTime;
                z.topic = q.topic;
                z.startDate = q.startDate;
                z.endDate = q.endDate;
                z.quizNum = q.quizNum;
                data.Add(z);
            }
            return data;
        }

        public int verifyTotalQ(string totalq , string id )
        {
            var query = (from x in db.questions.Where(f => f.cid.Equals(id))
                         select x).Distinct();
            int i = 0;
            foreach (var q in query)
            {
                i = i + 1;
            }
            if ( i >= Convert.ToInt32( totalq ))
                return 1;

            return -1;
        }
        public List<quizResult> getResult( string id , string cid , int qNo)
        {
            var query = from x in db.quizResults
                        where x.tid == id && x.cid == cid && (int)x.quizNum == qNo
                        select x;

            List<quizResult> data = new List<quizResult>();

            foreach (var q in query)
            {
                quizResult a = new quizResult();
                a.cid = q.cid;
                a.qid = q.qid;
                a.sid = q.sid;
                a.tid = q.tid;
                a.obtainMarks = q.obtainMarks;
                a.totalmarks = q.totalmarks;
                a.quizNum = q.quizNum;
                data.Add(a);
            }

            return data;
        }
        public int verifyAssignmentNumber(string cid, int aNo,string tid)
        {
            var q = from x in db.assignments
                    where (x.cid == cid && x.aNumber == aNo && x.tid == tid)
                    select x;
            if (q != null)
                return -1;
            return 1;
        }
        public List<assignmentResult> getAssignmentsResult(string tid, string cid , int aNo)
        {
            var query = from x in db.assignmentResults
                        where x.cid.Equals(cid) && x.aNumber == aNo
                         select x;

            /*var q = from x in db.assignmentResults.Where(x => x.cid.Equals(cid))
                    select x;*/

            List<assignmentResult> data = new List<assignmentResult>();

            foreach (var a in query)
            {
                //foreach (var s in query)
                //{
                  //  if (s.aid.Equals(a.aid))
                    //{
                        assignmentResult ass = new assignmentResult();
                        ass.aid = a.aid;
                        ass.cid = a.cid;
                        ass.marksObtained = a.marksObtained;
                        ass.totalMarks = a.totalMarks;
                        ass.sid = a.sid;
                        ass.outPutFilePath = a.outPutFilePath;
                        ass.codeFilePath = a.codeFilePath;
                        ass.aNumber = a.aNumber;
                        data.Add(ass);
                    //}
                //}
            }
            return data;
        }
        public void SaveAssignment(assignment ass, string id)
        {
            teacher t = db.teachers.SingleOrDefault(x => x.tid == id);
            cours c = db.courses.SingleOrDefault(x => x.cid.Equals(ass.cid));
            ass.tid = id;
            t.assignments.Add(ass);
            c.assignments.Add(ass);
            db.assignments.Add(ass);
            db.SaveChanges();
        }
        public List<assignment> getAssignments(string id , string cid)
        {
            var query = from x in db.assignments
                        where x.tid == id && x.cid == cid
                        select x;
            List<assignment> data = new List<assignment>();

            DateTime d = DateTime.Now;

            foreach (var q in query)
            {
                assignment a = new assignment();
                a.aid = q.aid;
                a.cid = q.cid;
                a.tid = q.tid;
                a.aNumber = q.aNumber;
                a.totalMarks = q.totalMarks;
                a.startDate = q.startDate;
                a.endDate = q.endDate;
                a.solutionFilePath = q.solutionFilePath;
                a.questionFilePath = q.questionFilePath;
                if ( q.endDate >= d )
                    data.Add(a);
            }
            return data;
        }

        public List<MVC_First.Models.attendance1> getAttendanceView(string n, string c, string class1)
        {
            var query = (from x in db.courses.Where(f => f.cid.Equals(c))
                         from p in db.students.Where(g => g.courses.Contains(x) && g.session.Equals(class1))
                         select p);
            
           
            List<MVC_First.Models.attendance1> t = new List<attendance1>();
            foreach(var ab in query)
            {
                attendance1 at = new attendance1();
                at.courseId = c;
                at.tName = n;
                at.studentId = ab.sid;
                at.student = ab;
                if (!t.Contains(at))
                {
                    t.Add(at);
                }
            }
            return t;
        }

        public int markAttendance(string list, string tName, string cName,string date,string classN)
        {
            string[] roll = {"a"};
            if( list!=null)
                roll = list.Split(',');

            var query = (from x in db.courses.Where(f => f.cid.Equals(cName))
                         from p in db.students.Where(g => g.courses.Contains(x) && g.session.Equals(classN))
                         select p);

            List<MVC_First.Models.attendance1> t = new List<attendance1>();
            foreach (var ab in query)
            {
                attendance1 at = new attendance1();
                at.courseId = cName;
                at.tName = tName;
                at.@class = classN;
                at.studentId = ab.sid;

                at.date = new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[0]), Convert.ToInt32(date.Split('/')[1]));
                
                if (roll.Contains(at.studentId))
                {
                    at.atnd = "P";
                }
                else
                    at.atnd = "A";

                var z = db.attendance1.ToList();
                attendance1 s = z.Find(x => x.studentId.Equals(at.studentId) && x.tName.Equals(tName) && x.courseId.Equals(cName) && x.date.Equals(at.date));
                if (s == null)
                {
                    db.attendance1.Add(at);
                    
                }
                else
                {
                    if (roll.Contains(at.studentId))
                    {
                        s.atnd = "P";
                    }
                    else
                        s.atnd = "A";
                    
                    db.Entry(s).State = System.Data.Entity.EntityState.Modified;
                }
            }
            db.SaveChanges();
            return 1;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}