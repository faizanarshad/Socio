using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_First.Models;
using System.IO;
using System.Data.Entity;
using System.Text;
using System.Data.Entity.Validation;
using System.Net.Mail;

namespace MVC_First.Controllers
{
    public class StudentController : Controller
    {
        IStudentDAL student;

        NudbEntities db = new NudbEntities();

        public StudentController ( IStudentDAL IS )
        {
            student = IS;
        }

        public ActionResult StudentHome()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
            
        }
        
        public ActionResult notEnrolled()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult MessageToUser()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public JsonResult LikePost()
        {
            int pid = Convert.ToInt32(Request["pid"]);
            string tid = Session["id"].ToString();

            post p = db.posts.Find(pid);
            p.likes = p.likes + 1;

            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePost()
        {
            int pid = Convert.ToInt32(Request["pid"]);
            string tid = Session["id"].ToString();

            post p = db.posts.Find(pid);
            db.posts.Remove(p);

            db.SaveChanges();

            return this.Json(true, JsonRequestBehavior.AllowGet);
        }



        public ActionResult TakeQuiz()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
            
        }
        public ActionResult StartQuiz()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                string id = Request["cid"];
                List<quiz> qz = new List<quiz>();
                string sid = Session["id"].ToString();
                qz = student.getQuiz(id , sid);

                var q = from x in db.courses where x.cid == id select x;
                string cname = "";
                foreach (var a in q)
                    cname = a.name;

                if (qz.Count > 0)
                {
                    ViewBag.name = cname;
                    return View(qz);
                }
                else
                {
                    ViewBag.ErrorType = "Start Quiz";
                    ViewBag.message = "There is no quiz uploaded";
                    return View("ErrorPage");
                }
            }
            return RedirectToAction("signIn", "Home");
            
        }
        public JsonResult CheckCourse()
        {

            string id = Request["cid"];
            string sid = Session["id"].ToString();

            int ans = student.verifyCourse(id , sid);

            if (ans == -1)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetCourses()
        {
            string sid = Session["id"].ToString();

            var data = student.GetCourses(sid);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Start( string id , int totalQ , int qid , string tid , int totalTime , int totalAttempts , int qNo)
        {
            List<question> q = new List<question>();

            q = student.getQuestions( id , totalQ);

            ViewBag.qid = qid;
            ViewBag.tid = tid;
            ViewBag.cid = id;
            ViewBag.totalTime = totalTime;
            ViewBag.attempts = totalAttempts;
            ViewBag.quizNo = qNo;

            if (q.Count > 0)
                return View(q);
            else
            {
                ViewBag.ErrorType = "No Question";
                ViewBag.message = "There is no question uploaded for this quiz";
                return View("ErrorPage");
            }
        }
        public ActionResult ViewCourse()
        {
            string sid = Session["id"].ToString();

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

            if (data.Count > 0)
                return View(data);
            else
            {
                ViewBag.ErrorType = "View Courses";
                ViewBag.message = "You are not enrolled in any course";
                return View("ErrorPage");
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GenerateResult(FormCollection form)
        {
            string questions = Request["QID"];

            string TeacherID = Request["TID"];
            string QuizID = Request["QuizID"];
            string CourseID = Request["CID"];
            string attempt = Request["TotalAttempts"];

            var qu = from x in db.courses where x.cid == CourseID select x;
            string cname = "";
            foreach (var a in qu)
                cname = a.name;

            string[] token = questions.Split(',');
            int totalQuestion = token.Length - 1;
            int obtainedMarks = 0;

            List<question> qs = new List<question>();

            var q = from x in db.questions
                    where (x.cid.Equals(CourseID))
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

            int attempted = 0;
            int wrong = 0;
            int correct = 0;
            int blank = 0;
            string str = form[token[1]];
            for (int i = 1; i < token.Length; i++)
            {
                if (form[token[i]] != null)
                {
                    attempted = attempted + 1;
                    if (qs.Find(ques => ques.quid == Convert.ToInt32(token[i])).correctOption.Equals(form[token[i]]))
                    {
                        obtainedMarks = obtainedMarks + 1;
                        correct = correct + 1;
                    }
                    else
                    {
                        wrong = wrong + 1;
                    }
                }
                
            }
            blank = totalQuestion - attempted;
            int qid = Convert.ToInt32(QuizID);
            string id = Session["id"].ToString();
            var res = db.quizResults.SingleOrDefault(y =>y.qid == qid && y.cid.Equals(CourseID) && y.tid.Equals(TeacherID) && y.sid.Equals(id));

            quizResult qr = new quizResult();
            qr.cid = CourseID;
            qr.qid = Convert.ToInt32(QuizID);
            qr.tid = TeacherID;
            qr.sid = Session["id"].ToString();
            qr.totalmarks = totalQuestion.ToString();
            qr.obtainMarks = obtainedMarks.ToString();
            
            
            if (res != null)
            {
                if (Convert.ToInt32(attempt) > res.attempts && Convert.ToInt32(res.obtainMarks) < obtainedMarks)
                {
                    res.totalmarks = totalQuestion.ToString();
                    res.obtainMarks = obtainedMarks.ToString();
                    res.attempts = res.attempts + 1;

                    Log log = new Log();
                    log.courseId = qr.cid;
                    log.discription = "Take Quiz "+res.attempts;
                    log.totalMarks = Convert.ToInt32(qr.totalmarks);
                    log.tName = qr.tid;
                    log.type = "Quiz";
                    log.studentId = qr.sid;
                    log.obtainedMarks = Convert.ToInt32(qr.obtainMarks);
                    log.date = DateTime.Now;

                    db.Logs.Add(log);

                    db.SaveChanges();
                }
            }
            else if (res == null)
            {
                qr.attempts = 1;

                Log log = new Log();
                log.courseId = qr.cid;
                log.discription = "Take Quiz Attempt # " + qr.attempts;
                log.totalMarks = Convert.ToInt32(qr.totalmarks);
                log.tName = qr.tid;
                log.type = "Quiz";
                log.studentId = qr.sid;
                log.obtainedMarks = Convert.ToInt32(qr.obtainMarks);
                log.date = DateTime.Now;

                db.Logs.Add(log);

                db.quizResults.Add(qr);
                db.SaveChanges();
            }
            
            ViewBag.correct = correct;
            ViewBag.wrong = wrong;
            ViewBag.blank = blank;
            ViewBag.attempted = attempted;
            ViewBag.name = cname;

            return View(qr);
        }
        public ActionResult ViewAssignments()
        {
            string id = Request ["cid"];

            var q = from x in db.courses where x.cid == id select x;
            string cname = "";
            foreach (var a in q)
                cname = a.name;

            List<assignment> data = new List<assignment>();

            data = student.getAssignments(id);

            if (data.Count > 0)
            {
                ViewBag.name = cname;
                return View(data);
            }
            else
            {
                ViewBag.ErrorType = "View Assignment";
                ViewBag.message = "There is no Assignment uploaded";
                return View("ErrorPage");
            }
        }
        public ActionResult AssignmentResult()
        {
            return View();
        }
        public ActionResult getAssignmentsResult()
        {
            string id = Session["id"].ToString();
            string cid = Request["cid"].ToString();

            var quer = from x in db.courses where x.cid == cid select x;
            string cname = "";
            foreach (var a in quer)
                cname = a.name;

            List<assignmentResult> data = new List<assignmentResult>();

            var query = from x in db.assignmentResults
                        where x.cid == cid && x.sid == id
                        select x;

            foreach (var q in query)
            {
                assignmentResult a = new assignmentResult();
                a.cid = q.cid;
                a.sid = q.sid;
                a.marksObtained = q.marksObtained;
                a.totalMarks = q.totalMarks;
                a.comment = q.comment;

                data.Add(a);
            }
            ViewBag.name = cname;
            return View(data);
        }
        public ActionResult ViewResult()
        {
            string id = Session["id"].ToString();
            string cid = Request["cid"].ToString();
            List<quizResult> data = new List<quizResult>();

            var quer = from x in db.courses where x.cid == cid select x;
            string cname = "";
            foreach (var a in quer)
                cname = a.name;

            data = student.getResult(id , cid);

            if (data.Count > 0)
            {
                ViewBag.name = cname;
                return View(data);
            }
            else
            {
                ViewBag.ErrorType = "View Result";
                ViewBag.message = "Sorry , No record found";
                return View("ErrorPage");
            }
        }
        public ActionResult showResult()
        {
            return View();
        }
        public ActionResult Assignments()
        {
            return View();
        }

        public ActionResult ViewLog()
        {
            List<Log> data = db.Logs.ToList();
            if (data.Count > 0)
                return View(data);
            else
            {
                ViewBag.ErrorType = "View Log";
                ViewBag.message = "Sorry , No recent Log";
                return View("ErrorPage");
            }
        }

        public ActionResult SubmitAssignment()
        {

            return View();
        }
        public ActionResult SaveSubmitAssignment()
        {
            HttpPostedFileBase file = Request.Files[0];
            
            
                file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                HttpPostedFileBase file1 = Request.Files[1];

                if (file.ContentType.Equals("text/plain") && file1.ContentType.Equals("text/plain"))
                {
                    file1.SaveAs(Server.MapPath(@"~\Files\" + file1.FileName));

                    var dataFile1 = Server.MapPath(@"~\Files\" + file1.FileName);

                    string cid = Request["cid"];
                    int aNo = Convert.ToInt32(Request["aNumber"]);

                    assignment ass = student.getAssignmentData(cid, aNo);

                    var dataFile2 = ass.solutionFilePath;

                    assignmentResult a = new assignmentResult();

                    a.aid = ass.aid;
                    a.cid = cid;
                    a.sid = Session["id"].ToString();
                    a.aNumber = ass.aNumber;
                    a.totalMarks = ass.totalMarks;
                    a.codeFilePath = dataFile;
                    a.outPutFilePath = dataFile1;

                    int ans = student.SaveAssignmentResult(a);

                    if (ans == 0)
                    {
                        ViewBag.ErrorType = "Assignment Submission";
                        ViewBag.message = "You have already submit this assignment";
                        return View("ErrorPage");
                    }

                    Log log = new Log();
                    log.courseId = cid;
                    log.discription = "Submit Assignment";
                    log.totalMarks = Convert.ToInt32(a.totalMarks);
                    log.tName = ass.tid;
                    log.type = "Quiz";
                    log.studentId = a.sid;
                    log.obtainedMarks = Convert.ToInt32(a.marksObtained);
                    log.date = DateTime.Now;

                    db.Logs.Add(log);

                    return View("StudentHome");
            }
            else
            {
                ViewBag.ErrorType = "File Uploading";
                ViewBag.message = "Please upload a text code file and text output file";
                return View("ErrorPage");
            }

        }
        public ActionResult ErrorPage()
        {
            return View();
        }

        public ActionResult personalMesg4mStd()
        {
            return View();
        }

        public ActionResult Notification()
        {

            string name = Session["id"].ToString();


            
            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (!(x.title.Equals("message")) && !(x.Sender_u_id.Equals("ExamBranch"))) //&& x.status.Equals("0")
                    select x;




            return View(a.ToList());
        }

      
        public ActionResult showMidResult()
        {
            return View();
        }
        public ActionResult showFinalResult()
        {
            return View();
        }
        public FileResult DownLoad(int id)
        {
            
            string filename = (from f in db.assignments
                               where f.aid == id
                               select f.questionFilePath).First();
            string contentType = "application/pdf";
            return File(filename, contentType);
        }
        public FileResult DownLoadMidResult()
        {
            string fall = Request["fall"];
            string degree = Request["degree"];
            string cname = degree + fall;

            var query = from f in db.FileDatas
                        where f.className.Equals(cname) && f.type.Equals("Mid")
                        select f;

            string filename = "result";

            foreach (var q in query)
                filename = q.filePath;

            string contentType = "application/pdf";
            return File(filename, contentType);
            
        }
        public FileResult DownLoadFinalResult()
        {
            string fall = Request["fall"];
            string degree = Request["degree"];
            string cname = degree + fall;

            var query = from f in db.FileDatas
                        where f.className.Equals(cname) && f.type.Equals("Final")
                        select f;


            string filename = "result";

            foreach (var q in query)
                filename = q.filePath;

            string contentType = "application/pdf";
            return File(filename, contentType);
        }
        

        [HttpPost]
        public JsonResult Notification1(string uid)
        {
           
            string name = Request["uid"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");

            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (!(x.title.Equals("message")) && !(x.Sender_u_id.Equals("ExamBranch"))) //&& x.status.Equals("0")
                    select x;
            List<Announcement> anno = new List<Announcement>();
            foreach (Announcement b in a)
            {
                Announcement an = new Announcement();
                an.mesg_text = b.mesg_text;
                an.Sender_u_id = b.Sender_u_id;
                an.status = b.status;
                an.title = b.title;
                an.dateTime = b.dateTime;
                an.audience = b.audience;
                an.destination = b.destination;
                anno.Add(an);
            }
            
            
            return this.Json(anno, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ViewCourseJson(string uid)
        {
            string sid = Request["uid"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            

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

            if (data.Count > 0)
                return Json(data.ToList().ToString(), JsonRequestBehavior.AllowGet);
            else
            {
                ViewBag.ErrorType = "View Courses";
                ViewBag.message = "You are not enrolled in any course";
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ViewAssignmentsJson(string cid)
        {
            string id = Request["cid"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            List<assignment> data = new List<assignment>();

            data = student.getAssignments(id);

            if (data.Count > 0)
                return Json(data, JsonRequestBehavior.AllowGet);
            else
            {
                ViewBag.ErrorType = "View Assignment";
                ViewBag.message = "There is no Assignment uploaded";
                return Json(ViewBag.message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ViewLogson()
        {
            List<Log> data = db.Logs.ToList();
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            if (data.Count > 0)
                return Json(data, JsonRequestBehavior.AllowGet);
            else
            {
                ViewBag.ErrorType = "View Log";
                ViewBag.message = "Sorry , No recent Log";
                return Json(ViewBag.message, JsonRequestBehavior.AllowGet); // yaha error page hora ta jahan bhi false return hoga waha errorpage ki dive show krwa den gey 
            }
        }
        public JsonResult GetCoursesJson(string username)
        {
            //string sid = Request["username"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            var data = student.GetCourses(username);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ViewResultJson(string uid)
        {
            //string id = Session["id"].ToString();
            string id = Request["uid"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            List<quizResult> data = new List<quizResult>();

            data = student.getResult(id);

            if (data.Count > 0)
                return Json(data.ToList(), JsonRequestBehavior.AllowGet);
            else
            {
                ViewBag.ErrorType = "View Result";
                ViewBag.message = "Sorry , No record found";
                return Json(ViewBag.message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AttendanceJson(string c , string username)
        {
            string n = Request["username"];
            string c1 = Request["c"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            var  ar1 = (from x in db.attendance1.Where(f => f.courseId.Equals(c1) && f.studentId.Equals(n))
                                        select x);
            List<attendance1> ar = new List<attendance1>(); 
            foreach (attendance1 b in ar1)
            {
                attendance1 an = new attendance1();
                an.atnd = b.atnd;
                an.date = b.date;
                an.courseId = b.courseId;
                //an.cours = b.cours;
                ar.Add(an);
            }
           return Json(ar, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Attendance(string c)
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                var a1 = Session["id"];
                string n = a1.ToString();
                List<attendance1> ar = (from x in db.attendance1.Where(f => f.courseId.Equals(c) && f.studentId.Equals(n))
                                        select x).ToList();
                if (ar.Count == 0)
                {
                    return RedirectToAction("notEnrolled", "Student");
                }

                return View(ar);
            }
            else
                return RedirectToAction("signIn", "Home");

        }

        public ActionResult viewMidResult()
        {

            string name = Session["id"].ToString();
            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (!(x.title.Equals("message")) && (x.Sender_u_id.Equals("Teacher"))) //&& x.status.Equals("0")
                    select x;



            return View(a.ToList());
        }
        public ActionResult viewFinalResult()
        {

            string name = Session["id"].ToString();
            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (!(x.title.Equals("message")) && (x.Sender_u_id.Equals("Teacher"))) //&& x.status.Equals("0")
                    select x;



            return View(a.ToList());
        }
       
    }
}
