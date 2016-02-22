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

        PucitDBEntities db = new PucitDBEntities();
        puEntities db1 = new puEntities();

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


        public ActionResult SendMail()
        {
            return View();
        }
        [HttpPost]
        public ViewResult SendMail(MailModel _objModelMail)
        {
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress("faizanarshad124@gmail.com");
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("nufeit@gmail.com", "skyfighter");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return View("TeacherHome");
            }
            else
            {
                return View();
            }
        }

        public JsonResult AddComment()
        {
            string body = Request["id"];
            string pid = Request["pid"];
            string sid = Session["id"].ToString();

            comment c = new comment();
            c.body = body;
            c.c_creator = sid;
            c.likes = 0;
            c.time = DateTime.Now;
            c.pid = Convert.ToInt32(pid);

            db.comments.Add(c);
            db.SaveChanges();

            
            return this.Json(c, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPost()
        {
            string body = Request["id"];
            string gid = Request["gid"];
            string sid = Session["id"].ToString();

            post p = new post();
            p.body = body;
            p.p_creator = sid;
            p.likes = 0;
            p.time = DateTime.Now;
            p.grpID = Convert.ToInt32(gid);

            db.posts.Add(p);
            db.SaveChanges();

            return this.Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowGroupData(string gid)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                int grp = Convert.ToInt32(gid);
                var data = from x in db.posts
                           where x.grpID == grp
                           select x;

                ViewBag.grpID = grp;
                ViewBag.grpNam = db.groups.Find(grp).name;
                return View(data.ToList());
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

        public ActionResult ShowComments(string pid)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                int post = Convert.ToInt32(pid);
                var data = from x in db.comments
                           where x.pid == post
                           select x;

                var posts = db.posts.Find(post);

                ViewBag.grpName = db.groups.Find(posts.grpID).name;
                ViewBag.pCreator = posts.p_creator;
                ViewBag.time = posts.time;
                ViewBag.body = posts.body;
                ViewBag.likes = posts.likes;
                ViewBag.postID = post;

                return View(data.ToList());
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

        public ActionResult DiscussionForum()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

        public ActionResult ViewGroups()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                var s1 = db.students.Find(Session["id"]);
                if (s1 != null)
                {
                    string classname = s1.session;
                    var data = db.groups.Where(x => x.@class.Equals(classname));
                    return View(data.ToList());
                }
                else
                {
                    ViewBag.message = "No Group Exist For user";
                    return View("MessageToUser");
                }
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
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
                    return RedirectToAction("notEnrolled","Student");
                }

                return View(ar);
            }
            else
                return RedirectToAction("signIn", "Home");

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

                    FileStream fin = new FileStream(dataFile1, FileMode.Open);
                    StreamReader sr = new StreamReader(fin);
                    string result = sr.ReadLine();

                    List<string> OutFile = new List<string>();

                    char[] delimiterChar = { ' ' };
                    string[] data = null;
                    int i = 0;
                    while (result != null)
                    {
                        data = result.Split(delimiterChar);
                        i = 0;
                        while (i < data.Length)
                        {
                            OutFile.Add(data[i]);
                            i++;
                        }
                        result = sr.ReadLine();
                    }
                    sr.Close();
                    fin.Close();

                    var dataFile2 = ass.solutionFilePath;

                    FileStream fin1 = new FileStream(dataFile2, FileMode.Open);
                    StreamReader sr1 = new StreamReader(fin1);
                    result = sr1.ReadLine();

                    List<string> SolFile = new List<string>();

                    while (result != null)
                    {
                        data = result.Split(delimiterChar);
                        int j = 0;
                        while (j < data.Length)
                        {
                            SolFile.Add(data[j]);
                            j++;
                        }
                        result = sr1.ReadLine();
                    }
                    sr1.Close();
                    fin1.Close();

                    i = 0;
                    double count = 0;
                    while (i < OutFile.Count && i < SolFile.Count)
                    {
                        if (OutFile[i].Equals(SolFile[i]))
                            count = count + 1;
                        i = i + 1;
                    }
                    int marks = 0;
                    if (count == SolFile.Count)
                        marks = ass.totalMarks;

                    assignmentResult a = new assignmentResult();

                    a.aid = ass.aid;
                    a.cid = cid;
                    a.sid = Session["id"].ToString();
                    a.marksObtained = marks;
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


            // var a = cx.Announcements.Where(x => x.audience.Equals(name) ORDER BY (date) DESC );
            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (!(x.title.Equals("message")) && !(x.Sender_u_id.Equals("ExamBranch"))) //&& x.status.Equals("0")
                    select x;




            return View(a.ToList());
        }

        public ActionResult inbox()
        {

            string name = Session["id"].ToString();


            // var a = cx.Announcements.Where(x => x.audience.Equals(name) ORDER BY (date) DESC );
            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (x.title.Equals("message") && (!(x.Sender_u_id.Equals("ExamBranch"))))//&& x.status.Equals("0")
                    select x;




            return View(a.ToList());
        }
        public ActionResult showDatesheet()
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
        public FileResult DownLoadDatesheet()
        {
            string fall = Request["fall"];
            string degree = Request["degree"];
            string cname = degree + fall;

            var query = from f in db.DateSheets
                               where f.className.Equals(cname)
                               select f;

            string filename = "result";

            foreach (var q in query)
                filename = q.filePath;

            string contentType = "application/pdf";
            return File(filename, contentType);
        }
        /// <summary>
        /// JSon functions for all buttons at student side starts here 
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult Notification1(string uid)
        {
            // string name = Session["id"].ToString();
           
           // string name = Request["uid"];r
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
            
            //var data = student.GetCourses(name);

            //Response.AppendHeader("Access-Control-Allow-Origin", "null");

            return this.Json(anno, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ViewCourseJson(string uid)
        {
            string sid = Request["uid"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            //.string sid = Session["id"].ToString();

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


        public ActionResult viewDatesheet()
        {

            string name = Session["id"].ToString();
            var a = from x in db.Announcements
                    where x.destination.Equals(name) && (!(x.title.Equals("message")) && (x.Sender_u_id.Equals("ExamBranch"))) //&& x.status.Equals("0")
                    select x;



            return View(a.ToList());
        }
        public ActionResult personalMesgS2S()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();

            }
            return RedirectToAction("signIn", "Home");

        }
        public ActionResult personalMesgS2T()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                return View();

            }
            return RedirectToAction("signIn", "Home");

        }

        public ActionResult personalMesg2S()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                ViewBag.ttid = Session["id"];




                String subject = Request["sub"];
                String mesg = Request["mesg_text"];



                var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");

                String stid = Request["sid"];
                var q = from x in db.students
                        where x.sid.Equals(stid)
                        select x;

                foreach (var i in q)
                {

                    var a = new Announcement();
                    a.destination = i.sid;

                    a.audience = i.batch;
                    a.title = "message";
                    a.mesg_text = mesg;

                    a.Sender_u_id = Session["id"].ToString();
                    a.dateTime = DateTime.Now;
                    a.status = "0";

                    db.Announcements.Add(a);



                }

                try
                {

                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }

                    throw new DbEntityValidationException(
                        "Entity Validation Failed - errors follow:\n" +
                        sb.ToString(), ex
                    ); // Add the original exception as the innerException
                }
                return View("StudentHome");
            }

            return RedirectToAction("signIn", "Home");

        }

        public ActionResult personalMesg2T()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Student"))
            {
                ViewBag.ttid = Session["id"];

                String subject = Request["sub"];
                String mesg = Request["mesg_text"];

                var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");

                String ttid = Request["tid"];
                var q = from x in db.teachers
                        where x.tid.Equals(ttid)
                        select x;

                foreach (var i in q)
                {


                    var a = new Announcement();
                    a.destination = i.tid;

                    a.audience = i.name;
                    a.title = "message";
                    a.mesg_text = mesg;

                    a.Sender_u_id = Session["id"].ToString();
                    a.dateTime = DateTime.Now;
                    a.status = "0";

                    db.Announcements.Add(a);


                }

                try
                {

                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }

                    throw new DbEntityValidationException(
                        "Entity Validation Failed - errors follow:\n" +
                        sb.ToString(), ex
                    ); // Add the original exception as the innerException
                }
                return View("StudentHome");
            }

            return RedirectToAction("signIn", "Home");

        }
    }
}
