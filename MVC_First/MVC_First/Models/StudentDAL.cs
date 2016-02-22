using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_First.Models;

namespace MVC_First.Models
{
   
    public interface IStudentDAL
    {
        int verifyCourse(string id , string stid );
        List<quiz> getQuiz( string id , string sid);
        List<question> getQuestions ( string id , int totalQ );
        List<quizResult> getResult(string id , string cid);
        List<quizResult> getResult(string id);
        List<assignment> getAssignments(string id);
        List<cours> GetCourses(string sid);
        assignment getAssignmentData( string cid , int aNo);
        int SaveAssignmentResult(assignmentResult ass);
    }

    public class StudentDAL : IStudentDAL
    {
        PucitDBEntities db = new PucitDBEntities();

        public int verifyCourse(string id , string stid)
        {
            

            int flage = 0;
            var query = (from x in db.students.Where(f => f.courses.Any())
                         from p in db.courses.Where(g => g.students.Contains(x))
                         select new { p.cid, x.sid });

            DateTime d = DateTime.Now;

            var q = from x in db.quizs
                    where (x.cid.Equals(id))
                    select new { x.startDate, x.endDate };

            if (q != null)
            {
                foreach (var y in q)
                {
                    if (y.endDate >= d)
                    {
                        flage = 1;
                    }
                }
            }
            
            if (query != null)
            {
                foreach (var a in query)
                {
                    if (a.cid.Equals(id) && a.sid.Equals(stid))
                    {
                        if (flage == 1)
                            return 1;
                    }
                        
                }
            }

            return -1;
            
        }
        public List<cours> GetCourses( string sid)
        {
            
            var query = (from x in db.students.Where(f => f.sid.Equals(sid))
                         from p in db.courses.Where(g => g.students.Contains(x))
                         select p);

            List<cours> data = new List<cours>();

            foreach (var q in query)
            {
                cours c = new cours();
                c.cid = q.cid;
                c.name = q.name;
                c.creditHours = q.creditHours;
                data.Add(c);

            }
            return data;

        }
        public assignment getAssignmentData(string cid, int aNo)
        {
            var query = from x in db.assignments where ( x.aNumber==aNo && x.cid.Equals(cid))
                            select x;
            assignment ass = new assignment();

            foreach (var q in query)
            {
                ass.aid = q.aid;
                ass.aNumber = q.aNumber;
                ass.cid = q.cid;
                ass.startDate = q.startDate;
                ass.endDate = q.endDate;
                ass.solutionFilePath = q.solutionFilePath;
                ass.totalMarks = q.totalMarks;
                

            }
            return ass;
        }
        public int SaveAssignmentResult(assignmentResult ass)
        {
            var query = from x in db.assignmentResults
                        where x.cid == ass.cid && x.sid == ass.sid && x.aid == ass.aid
                        select x;
            
            if (query.Count() <= 0)
            {
                db.assignmentResults.Add(ass);
                db.SaveChanges();
                return 1;
            }
            else
                return 0;
        }
        public List<quiz> getQuiz(string id , string sid )
        {
            var currentDate = DateTime.Now;

            
            var q = from x in db.quizs
                    where (x.cid.Equals(id) && x.endDate >= currentDate)
                    select x;

            List<quiz> qz = new List<quiz>();

            foreach (var a in q)
            {
                var res = db.quizResults.SingleOrDefault (y=>y.qid == a.qid && y.cid.Equals(a.cid) && y.tid.Equals(a.tid) && y.sid.Equals(sid));

                if (res != null && a.totalAttempts > res.attempts)
                {
                    quiz z = new quiz();
                    z.qid = a.qid;
                    z.cid = a.cid;
                    z.tid = a.tid;
                    z.totalQuestion = a.totalQuestion;
                    z.TotalMarks = a.TotalMarks;
                    z.TotalTime = a.TotalTime;
                    z.startDate = a.startDate;
                    z.endDate = a.endDate;
                    z.totalAttempts = a.totalAttempts;
                    z.quizNum = a.quizNum;

                    qz.Add(z);
                }
                else if (res == null)
                {
                    quiz z = new quiz();
                    z.qid = a.qid;
                    z.cid = a.cid;
                    z.tid = a.tid;
                    z.totalQuestion = a.totalQuestion;
                    z.TotalMarks = a.TotalMarks;
                    z.TotalTime = a.TotalTime;
                    z.startDate = a.startDate;
                    z.endDate = a.endDate;
                    z.totalAttempts = a.totalAttempts;
                    z.quizNum = a.quizNum;

                    qz.Add(z);
                }
            }

            return qz;

        }
        public List<question> getRandomList(List<question> possibleQuestions , int totalQ)
        {

            int biggestNumber = possibleQuestions.Count;

            int amountOfRandomNumbers = totalQ;

            List<question> resultList = new List<question>();

            Random rand = new Random();

            for (int i = 0; i < amountOfRandomNumbers; i++)
            {
                
                int randomNumber = rand.Next(1, possibleQuestions.Count) - 1;
                resultList.Add(possibleQuestions[randomNumber]);
                possibleQuestions.RemoveAt(randomNumber);
            }
            return resultList;
        }
        public List<question> getQuestions(string id , int totalQ )
        {
            List<question> qs = new List<question>();

            List<question> randomQuestions = new List<question>();

            var q = from x in db.questions
                    where (x.cid.Equals(id))
                    select x;

            foreach (var a in q)
            {
                question s = new question();
                s.quid = a.quid;
                s.statement = a.statement;
                s.option_A = a.option_A;
                s.option_B = a.option_B;
                s.Option_C = a.Option_C;
                s.option_D = a.option_D;
                s.correctOption = a.correctOption;
                s.solution = a.solution;
                s.marks = a.marks;

                qs.Add(s);
            }

            randomQuestions = getRandomList( qs , totalQ);

            return randomQuestions;
        }
        public List<quizResult> getResult( string id , string cid)
        {
            var query = from x in db.quizResults
                        where x.sid == id && x.cid == cid
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
        public List<quizResult> getResult(string id)
        {
            var query = from x in db.quizResults
                        where x.sid == id
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

                data.Add(a);
            }

            return data;
        }
        public List<assignment> getAssignments(string id)
        {
            var query = from x in db.assignments
                        where x.cid == id
                        select x;
            List<assignment> data = new List<assignment>();
            DateTime d = DateTime.Now;

            foreach (var q in query)
            {
                assignment a = new assignment();
                a.aid = q.aid;
                a.aNumber = q.aNumber;
                a.cid = q.cid;
                a.tid = q.tid;
                a.totalMarks = q.totalMarks;
                a.startDate = q.startDate;
                a.endDate = q.endDate;

                if (a.endDate >= d)
                    data.Add(a);
            }
            return data;
        }
    }
}