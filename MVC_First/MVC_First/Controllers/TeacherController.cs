using MVC_First.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Data.Entity;
using System.Net.Mail;

namespace MVC_First.Controllers
{
    public class TeacherController : Controller
    {
        ITeacherDAL teacher;
        private PucitDBEntities db = new PucitDBEntities();
        //private puEntities db1 = new puEntities();

        int id;//= 1;
        String a_id;

        public TeacherController(ITeacherDAL it)
        {
            a_id = ""+db.Announcements.Count();
            teacher = it;
        }
        public ActionResult TeacherHome()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                ViewBag.ttid = Session["id"];

                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult editAttendance()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string tid = Session["id"].ToString();
                    string course = Request["cor"];
                    string date = Request["startDate"].Split(' ')[0];
                    string class1 = Request["class1"];
                    DateTime date1 = new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[0]), Convert.ToInt32(date.Split('/')[1]));


                    ViewBag.date = date;
                    ViewBag.classN = class1;

                    var query = db.attendance1.Where(x => x.courseId.Equals(course) && x.tName.Equals(tid) && x.date.Equals(date1) && x.@class.Equals(class1)).ToList();
                    if (query.Count > 0)
                    {
                        return View(query);
                    }
                    else
                    {
                        ViewBag.message = "No data Exist!!";
                        return View("MessageToUser");
                    }
                }
                catch (Exception e )
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            return RedirectToAction("signIn", "Home");

        }
        public ActionResult selectClassAndDate()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    ViewBag.ttid = Session["id"];
                    ViewBag.course = Request["c"];
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            return RedirectToAction("signIn", "Home");

        }

        public ActionResult MessageToUser()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

		public ActionResult CreateGroup()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }
        [HttpPost]
        public ActionResult CreateGroup(group g)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    String fall = Request["fall"];
                    String degree = Request["degree"];
                    String section = Request["section"];
                    string tid = Session["id"].ToString();

                    group g1 = new group();
                    g1.@class = degree + fall + section;
                    g1.name = g.name;
                    g1.time = DateTime.Now;
                    g1.g_creator = tid;

                    db.groups.Add(g1);
                    db.SaveChanges();

                    return RedirectToAction("TeacherHome", "Teacher");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

        public JsonResult AttendanceJSON()
        {

            string sid = Request["sid"];
            string tid = Session["id"].ToString();

            var data = db.attendance1.Where(x=>x.studentId.Equals(sid) && x.tName.Equals(tid));
            List<attendance1> data1 = new List<attendance1>();
            foreach (attendance1 a in data)
            {
                attendance1 atn = new attendance1();
                atn.atnd = a.atnd;
                //atn.date = a.date;
                atn.courseId = a.date.ToString().Split(' ')[0];
                data1.Add(atn);
            }
            if (data1.Count>0)
            {
                return this.Json(data1, JsonRequestBehavior.AllowGet);
            }
            else
                return this.Json(false, JsonRequestBehavior.AllowGet);
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
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
                return RedirectToAction("signIn" , "Home");
        }
        [HttpPost]
        public ActionResult SendMail(MailModel _objModelMail)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                if (ModelState.IsValid)
                {
                    try
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
                    catch (Exception e)
                    {
                        ViewBag.ErrorType = "Exception";
                        ViewBag.message = e.Message;
                        return View("ErrorPage");
                    }
                }
                else
                {
                    return View();
                }
            }
            else
                return RedirectToAction("signIn" , "Home");
        }

        public JsonResult AddComment()
        {
            string body = Request["id"];
            string pid = Request["pid"];
            string tid = Session["id"].ToString();

            comment c = new comment();
            c.body = body;
            c.c_creator = tid;
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
            string tid = Session["id"].ToString();

            post p = new post();
            p.body = body;
            p.p_creator = tid;
            p.likes = 0;
            p.time = DateTime.Now;
            p.grpID = Convert.ToInt32(gid);

            db.posts.Add(p);
            db.SaveChanges();

            return this.Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowGroupData(string gid)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    int grp = Convert.ToInt32(gid);
                    var data = from x in db.posts
                               where x.grpID == grp
                               select x;

                    ViewBag.grpID = grp;
                    ViewBag.grpNam = db.groups.Find(grp).name;
                    return View(data.ToList());
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

        public ActionResult ShowComments(string pid)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
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
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

        public ActionResult DiscussionForum()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
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
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View(db.groups.ToList());
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }
		
        public ActionResult ConductQuiz()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
          
            return RedirectToAction("signIn", "Home");
        }
        public JsonResult CheckQuizNumber()
        {
            string [] str = Request["quizNum"].Split(',');
            string quiz = str[0];
            string id = str [1];

            int quizNo = Convert.ToInt32(quiz);

            string tid = Session["id"].ToString();

            var query = from x in db.quizs where x.cid == id && x.tid == tid select x;

            int flage = 0;

            foreach (var q in query)
            {
                if (q.quizNum == quizNo)
                    flage = 1;
            }

            if (flage == 0 )
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetCourses()
        {
            string tid = Session["id"].ToString();

            List<cours> data = new List<cours>();
            
            data = teacher.GetCourses(tid);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTopic( string cid )
        {
            List<question> data = new List<question>();

            string tid = Session["id"].ToString();

            data = teacher.GetTopic( cid );

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewCourse()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    var a1 = Session["id"];

                    List<teacher> s = db.teachers.ToList();

                    teacher s1 = s.Find(x => x.tid.Equals(a1));
                    List<cours> a = s1.courses.ToList();
                    if (a.Count() > 0)
                    {
                        return View(a);
                    }
                    else
                        return RedirectToAction("TeacherHome", "Teacher");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");

        }

        [HttpPost]
        public ActionResult Save(quiz z)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string tid = Session["id"].ToString();
                    string id = tid;
                    var startdate = Request["startDate"];
                    var enddate = Request["endDate"];

                    LogTeacher log = new LogTeacher();
                    log.courseId = z.cid;
                    log.discription = "Save Quiz";
                    log.totalMarks = z.TotalMarks;
                    log.tName = tid;
                    log.type = "Quiz";
                    log.date = DateTime.Today;

                    db.LogTeachers.Add(log);
                    db.SaveChanges();

                    string[] str = startdate.Split(' ');
                    string[] time = str[1].Split(':');
                    string[] date = str[0].Split('/');
                    string tt = str[2];

                    if (tt.Equals("AM"))
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                        z.startDate = dt;
                    }
                    else
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]) + 12, Convert.ToInt32(time[1]), 0);
                        z.startDate = dt;
                    }

                    string[] str1 = enddate.Split(' ');
                    string[] time1 = str1[1].Split(':');
                    string[] date1 = str1[0].Split('/');
                    string tt1 = str1[2];

                    if (tt1.Equals("AM"))
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[0]), Convert.ToInt32(date1[1]), Convert.ToInt32(time1[0]), Convert.ToInt32(time1[1]), 0);
                        z.endDate = dt;
                    }
                    else
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[0]), Convert.ToInt32(date1[1]), Convert.ToInt32(time1[0]) + 12, Convert.ToInt32(time1[1]), 0);
                        z.endDate = dt;
                    }

                    teacher.SaveQuiz(z, id);
                    ViewBag.message = 2;
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn" , "Home");

        }
        public ActionResult AddQuestions()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }

            return RedirectToAction("signIn", "Home");

        }
        public JsonResult CheckCourse()
        {

            string id = Request["cid"];

            int ans = teacher.verifyCourse(id);

            if (ans == -1)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult courseForEditQuiz()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public ActionResult EditQuiz()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string tid = Session["id"].ToString();
                    string cid = Request["cid"];

                    List<quiz> data = new List<quiz>();

                    var q = from x in db.courses where x.cid == cid select x;
                    string cname = "";
                    foreach (var a in q)
                        cname = a.name;

                    data = teacher.GetQuizes(tid, cid);

                    if (data.Count <= 0)
                    {
                        ViewBag.ErrorType = "Edit Quiz";
                        ViewBag.message = "There is no quiz uploaded in current date.";
                        return View("ErrorPage");
                    }
                    ViewBag.name = cname;

                    return View(data);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
                
            }

            return RedirectToAction("signIn", "Home");

            
        }
        public ActionResult Edit( int id )
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    quiz q = teacher.GetQuizeForUpdation(id);

                    return View(q);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
                
            }

            return RedirectToAction("signIn", "Home");
       }
        public ActionResult EditAssignment(int aid = 0)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    assignment q = teacher.GetAssignmentForUpdation(aid);

                    return View(q);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }

            }

            return RedirectToAction("signIn", "Home");
        }

        [HttpPost]
        public ActionResult EditAssignment(assignment assign)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string tid = Session["id"].ToString();
                    string id = tid;
                    var startdate = Request["startDate"];
                    var enddate = Request["endDate"];

                    LogTeacher log = new LogTeacher();
                    log.courseId = assign.cid;
                    log.discription = "Edit Assignment";
                    log.totalMarks = assign.totalMarks;
                    log.tName = tid;
                    log.type = "Assignment";
                    log.date = DateTime.Today;

                    db.LogTeachers.Add(log);


                    if (!startdate.Equals(Request["start"]))
                    {
                        string[] str = startdate.Split(' ');
                        string[] time = str[1].Split(':');
                        string[] date = str[0].Split('/');
                        string tt = str[2];

                        if (tt.Equals("AM"))
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                            assign.startDate = dt;
                        }
                        else
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]) + 12, Convert.ToInt32(time[1]), 0);
                            assign.startDate = dt;
                        }
                    }
                    else
                    {
                        string[] str = Request["start"].Split(' ');
                        string[] time = str[1].Split(':');
                        string[] date = str[0].Split('/');
                        string tt = str[2];

                        if (tt.Equals("AM"))
                        {
                            var outputLines = new List<string>();
                            string s = "" + Convert.ToInt32(date[2]) + "/" + Convert.ToInt32(date[1]) + "/" + Convert.ToInt32(date[0]) + " " + Convert.ToInt32(time[0]) + ":" + Convert.ToInt32(time[1]);
                            outputLines.Add(s);
                            System.IO.File.AppendAllLines(@"d:\errors.txt", outputLines);

                            //DateTime dt = new DateTime(2014, 04, 12);
                            DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0]));
                            assign.startDate = dt;
                        }
                        else
                        {
                            var outputLines = new List<string>();
                            string s = "" + Convert.ToInt32(date[2]) + "/" + Convert.ToInt32(date[1]) + "/" + Convert.ToInt32(date[0]) + " " + Convert.ToInt32(time[0]) + ":" + Convert.ToInt32(time[1]);
                            outputLines.Add(s);
                            System.IO.File.AppendAllLines(@"d:\errors.txt", outputLines);

                            //DateTime dt = new DateTime(2014, 04, 12);
                            DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0]));
                            assign.startDate = dt;
                        }

                    }

                    if (!enddate.Equals(Request["end"]))
                    {
                        string[] str1 = enddate.Split(' ');
                        string[] time1 = str1[1].Split(':');
                        string[] date1 = str1[0].Split('/');
                        string tt1 = str1[2];

                        if (tt1.Equals("AM"))
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[0]), Convert.ToInt32(date1[1]), Convert.ToInt32(time1[0]), Convert.ToInt32(time1[1]), 0);
                            assign.endDate = dt;
                        }
                        else
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[0]), Convert.ToInt32(date1[1]), Convert.ToInt32(time1[0]) + 12, Convert.ToInt32(time1[1]), 0);
                            assign.endDate = dt;
                        }
                    }
                    else
                    {
                        string[] str1 = Request["end"].Split(' ');
                        string[] time1 = str1[1].Split(':');
                        string[] date1 = str1[0].Split('/');
                        string tt1 = str1[2];

                        if (tt1.Equals("AM"))
                        {
                            var outputLines = new List<string>();
                            string s = "" + Convert.ToInt32(date1[2]) + "/" + Convert.ToInt32(date1[1]) + "/" + Convert.ToInt32(date1[0]) + " " + Convert.ToInt32(time1[0]) + ":" + Convert.ToInt32(time1[1]);
                            outputLines.Add(s);
                            System.IO.File.AppendAllLines(@"d:\errors.txt", outputLines);

                            //DateTime dt = new DateTime(2014, 04, 23);
                            DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[1]), Convert.ToInt32(date1[0]));
                            assign.endDate = dt;
                        }
                        else
                        {
                            var outputLines = new List<string>();
                            string s = "" + Convert.ToInt32(date1[2]) + "/" + Convert.ToInt32(date1[1]) + "/" + Convert.ToInt32(date1[0]) + " " + Convert.ToInt32(time1[0]) + ":" + Convert.ToInt32(time1[1]);
                            outputLines.Add(s);
                            System.IO.File.AppendAllLines(@"d:\errors.txt", outputLines);

                            //DateTime dt = new DateTime(2014,04,23);
                            DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[1]), Convert.ToInt32(date1[0]));
                            assign.endDate = dt;
                        }

                    }

                    if (Request.Files[0].ContentLength != 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file.ContentType.Equals("application/pdf"))
                        {
                            file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                            var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                            assign.questionFilePath = dataFile;
                        }
                        else
                        {
                            ViewBag.ErrorType = "File Uploading";
                            ViewBag.message = "Please upload a text code file and text output file";
                            return View("ErrorPage");
                        }
                    }
                    else
                    {
                        assign.questionFilePath = Request["Q"];
                    }

                    if (Request.Files[1].ContentLength != 0)
                    {
                        HttpPostedFileBase file1 = Request.Files[1];
                        if (file1.ContentType.Equals("text/plain"))
                        {
                            file1.SaveAs(Server.MapPath(@"~\Files\" + file1.FileName));

                            var dataFile1 = Server.MapPath(@"~\Files\" + file1.FileName);

                            assign.solutionFilePath = dataFile1;

                        }
                        else
                        {
                            ViewBag.ErrorType = "File Uploading";
                            ViewBag.message = "Please upload a text code file and text output file";
                            return View("ErrorPage");
                        }
                    }
                    else
                    {
                        assign.solutionFilePath = Request["S"];
                    }
                    db.Entry(assign).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        var outputLines = new List<string>();
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            outputLines.Add(string.Format(
                                "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                                DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                            foreach (var ve in eve.ValidationErrors)
                            {
                                outputLines.Add(string.Format(
                                    "- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage));
                            }
                        }
                        System.IO.File.AppendAllLines(@"d:\errors.txt", outputLines);
                        throw;
                    }
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn" , "Home");
        }


        public ActionResult SaveUpdatedQuiz( quiz q )
        {
            if (Session["id"] != null )
            {
                try
                {
                    teacher.SaveUpdatedQuiz(q);

                    LogTeacher log = new LogTeacher();
                    log.courseId = q.cid;
                    log.discription = "Update Quiz";
                    log.totalMarks = q.TotalMarks;
                    log.tName = q.tid;
                    log.type = "Quiz";
                    log.date = DateTime.Today;

                    db.LogTeachers.Add(log);
                    db.SaveChanges();

                    return RedirectToAction("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult ViewQuizes()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string cid = Request["cid"];
                    List<quiz> data = new List<quiz>();
                    string id = Session["id"].ToString();
                    data = teacher.getQuize(id, cid);

                    var q = from x in db.courses where x.cid == cid select x;
                    string cname = "";
                    foreach (var a in q)
                        cname = a.name;

                    if (data.Count > 0)
                    {
                        ViewBag.name = cname;
                        return View(data);
                    }
                    else
                    {
                        ViewBag.ErrorType = "View Quizes";
                        ViewBag.message = "There is no quiz uploaded";
                        return View("ErrorPage");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }

            return RedirectToAction("signIn", "Home");
        }
        public ActionResult SaveQuestion()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string id = Request["cid"];
                    string topic = Request["topic"];

                    HttpPostedFileBase file = Request.Files[0];
                    file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                    var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                    teacher.SaveQuestions(id, topic, dataFile);

                    ViewBag.message = 1;
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public JsonResult CheckTotalQuestions()
        {
            string [] str = Request["cid"].Split(',');
            string totalq = str[1];
            string id = str [0];

            int ans = teacher.verifyTotalQ(totalq , id);

            if (ans == -1)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }

            
        }
        public ActionResult ShowQuizes()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public ActionResult ViewQuizesResult()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string id = Session["id"].ToString();
                    string cid = Request["cid"];
                    int qNo = Convert.ToInt32(Request["quizNum"]);

                    List<quizResult> data = new List<quizResult>();

                    var q = from x in db.courses where x.cid == cid select x;
                    string cname = "";
                    foreach (var a in q)
                        cname = a.name;

                    data = teacher.getResult(id, cid, qNo);

                    if (data.Count > 0)
                    {
                        ViewBag.name = cname;
                        return View(data);
                    }

                    else
                    {
                        ViewBag.ErrorType = "View Quizes Result";
                        ViewBag.message = "There is no result available yet";
                        return View("ErrorPage");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }

            }
            else
                return RedirectToAction("signIn" , "Home");

        }
        public ActionResult UploadAssignment()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public JsonResult CheckAssignmentNumber()
        {
            string[] str = Request["aNumber"].Split(',');
            string cid = str[1];
            string aNo = str[0];
            string tid = Session["id"].ToString();

            int ans = teacher.verifyAssignmentNumber(cid , Convert.ToInt32(aNo),tid);

            if (ans == -1)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetAssignmentNumber()
        {
            string cid = Request["cid"];
            string tid = Session["id"].ToString ();

            List<int> data = new List<int>();

            var query = from x in db.assignments where x.cid == cid && x.tid == tid select x;

            foreach (var q in query)
                data.Add(q.aNumber);

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetQuizNumberNumber()
        {
            string cid = Request["cid"];
            string tid = Session["id"].ToString();

            List<int> data = new List<int>();

            var query = from x in db.quizs where x.cid == cid && x.tid == tid select x;

            foreach (var q in query)
                data.Add((int)q.quizNum);

            return this.Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ViewLog()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    List<LogTeacher> data = db.LogTeachers.ToList();
                    if (data.Count > 0)
                        return View(data);
                    else
                    {
                        ViewBag.ErrorType = "View Log";
                        ViewBag.message = "Sorry , No recent Log";
                        return View("ErrorPage");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
      
        }
        public ActionResult showQuizResult()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {

                return View();
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public ActionResult ViewAssignmentsResult()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {

                return View();
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public ActionResult ShowAssignmentsResult()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string tid = Session["id"].ToString();
                    string cid = Request["cid"];
                    int aNo = Convert.ToInt32(Request["aNumber"].ToString());

                    List<assignmentResult> data = new List<assignmentResult>();

                    data = teacher.getAssignmentsResult(tid, cid, aNo);

                    var q = from x in db.courses where x.cid == cid select x;
                    string cname = "";
                    foreach (var a in q)
                        cname = a.name;

                    if (data.Count > 0)
                    {
                        ViewBag.aNumber = aNo;
                        ViewBag.name = cname;
                        return View(data);
                    }
                    else
                    {
                        ViewBag.ErrorType = "View Assignment Result";
                        ViewBag.message = "There is no result available yet";
                        return View("ErrorPage");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public ActionResult SaveAssignment( assignment ass )
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string tid = Session["id"].ToString();
                    string id = tid;
                    var startdate = Request["startDate"];
                    var enddate = Request["endDate"];

                    string[] str = startdate.Split(' ');
                    string[] time = str[1].Split(':');
                    string[] date = str[0].Split('/');
                    string tt = str[2];

                    LogTeacher log = new LogTeacher();
                    log.courseId = ass.cid;
                    log.discription = "Save Assignment";
                    log.totalMarks = ass.totalMarks;
                    log.tName = tid;
                    log.type = "Assignment";
                    log.date = DateTime.Today;

                    db.LogTeachers.Add(log);

                    if (tt.Equals("AM"))
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                        ass.startDate = dt;
                    }
                    else
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]) + 12, Convert.ToInt32(time[1]), 0);
                        ass.startDate = dt;
                    }

                    string[] str1 = enddate.Split(' ');
                    string[] time1 = str1[1].Split(':');
                    string[] date1 = str1[0].Split('/');
                    string tt1 = str1[2];

                    if (tt1.Equals("AM"))
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[0]), Convert.ToInt32(date1[1]), Convert.ToInt32(time1[0]), Convert.ToInt32(time1[1]), 0);
                        ass.endDate = dt;
                    }
                    else
                    {
                        DateTime dt = new DateTime(Convert.ToInt32(date1[2]), Convert.ToInt32(date1[0]), Convert.ToInt32(date1[1]), Convert.ToInt32(time1[0]) + 12, Convert.ToInt32(time1[1]), 0);
                        ass.endDate = dt;
                    }

                    HttpPostedFileBase file = Request.Files[0];

                    file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                    var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                    ass.questionFilePath = dataFile;

                    HttpPostedFileBase file1 = Request.Files[1];
                    if (file.ContentType.Equals("application/pdf") && file1.ContentType.Equals("text/plain"))
                    {
                        file1.SaveAs(Server.MapPath(@"~\Files\" + file1.FileName));

                        var dataFile1 = Server.MapPath(@"~\Files\" + file1.FileName);

                        ass.solutionFilePath = dataFile1;

                        teacher.SaveAssignment(ass, id);

                        return View("TeacherHome");
                    }
                    else
                    {
                        ViewBag.ErrorType = "File Uploading";
                        ViewBag.message = "Please upload a text code file and text output file";
                        return View("ErrorPage");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }

        }
        public ActionResult showAssignment()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
                return RedirectToAction("signIn" , "Home");
        }
        [HttpPost]
        public ActionResult ViewAssignments()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string id = Session["id"].ToString();
                    string cid = Request["cid"];

                    List<assignment> data = new List<assignment>();

                    data = teacher.getAssignments(id, cid);

                    if (data.Count > 0)
                        return View(data);
                    else
                    {
                        ViewBag.ErrorType = "Show Assignment";
                        ViewBag.message = "There is no Assignment uploaded";
                        return View("ErrorPage");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn" , "Home");
        }
        public ActionResult selectClassAndDate1()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    ViewBag.ttid = Session["id"];
                    ViewBag.course = Request["c"];

                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            return RedirectToAction("signIn", "Home");

        }

        public ActionResult showAttendance1()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string c = Request["cid"];
                    string tid = Session["id"].ToString();
                    //   string date = Request["startDate"].Split(' ')[0];
                    String fall = Request["fall"];
                    String degree = Request["degree"];
                    String section = Request["section"];
                    string class1 = fall + "NUN";

                    ViewBag.course = c;
                    ViewBag.classN = class1;

                    List<MVC_First.Models.attendance1> a = db.attendance1.Where(x => x.tName.Equals(tid) && x.courseId.Equals(c) && x.@class.Equals(class1)).ToList();
                    List<MVC_First.Models.attendance1> ab = new List<attendance1>();
                    List<string> username = new List<string>();
                    foreach (attendance1 atnd in a)
                    {
                        if (!username.Contains(atnd.studentId))
                        {
                            username.Add(atnd.studentId);
                            ab.Add(atnd);
                        }
                    }
                    if (ab.Count > 0)
                    {
                        return View(ab);
                    }
                    else
                    {
                        ViewBag.message = "Option Selected is Not Correct.";
                        return View("MessageToUser");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }
        public ActionResult Attendance()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    string c = Request["cid"];
                    string tid = Session["id"].ToString();
                    string date = Request["startDate"].Split(' ')[0];
                    String fall = Request["fall"];
                    String degree = Request["degree"];
                    String section = Request["section"];
                    string class1 =  fall + "NUN";

                    ViewBag.date = date;
                    ViewBag.classN = class1;

                    List<MVC_First.Models.attendance1> a = teacher.getAttendanceView(tid, c, class1);
                    if (a.Count() > 0)
                    {
                        return View(a);
                    }
                    else
                    {
                        ViewBag.message = "Option Selected is Not Correct.";
                        return View("MessageToUser");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }

        [HttpPost]
        public ActionResult Attendance(string[] marked)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    var u = db.users.Find(Session["id"]).type;
                    string date = Request["date"];

                    string list = Request["marked"];
                    string tname = Request["tname"];
                    string course = Request["course"];
                    string classN = Request["class"];

                    int res = teacher.markAttendance(list, tname, course, date, classN);
                    ViewBag.attendance = "Marked";
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
               
            }
            else
                return RedirectToAction("signIn", "Home");
        }

        public ActionResult Announcement()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
                return RedirectToAction("signIn" , "Home");
        }

        public ActionResult AnnouncementToFall()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
                return RedirectToAction("signIn" , "Home");
        }

        public ActionResult AnnouncementToDegFall()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
                return RedirectToAction("signIn" , "Home");
        }

        public ActionResult AnnouncementToDegFallSection()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }

        public ActionResult AnnouncementToStudent()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            return RedirectToAction("signIn" , "Home");
        }


        public JsonResult getFall()
        {
            List<String> data = new List<String>();

            String[] fall = { "05", "06", "07", "08", "09", "10", "11", "12" };

            foreach (var q in fall)
            {

                data.Add(q);

            }

            return this.Json(data, JsonRequestBehavior.AllowGet);



        }



        public ActionResult AnnouncementAll()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    ViewBag.ttid = Session["id"];

                    //  id = db.Announcements.Max(p => p.announcement_id);

                    String subject = Request["sub"];
                    String mesg = Request["mesg_text"];
                    // String attachment = Request["attachment"];
                    //String audience = Request["a"]; // check on this if audience == "fall" then which fall, populate Student list acc.
                    //if audience == "degree" then which degree, populate Student list acc. 




                    var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");



                    String fall = Request["fall"];
                    var q = from x in db.students
                            where x.batch.Equals(fall)
                            select x;
                    foreach (var i in q)
                    {

                        //   id = db.Announcements.Max(p => p.announcement_id);
                        //     id = Convert.ToInt32(a_id);
                        // id = id + 1;
                        var a = new Announcement();
                        a.destination = i.sid;
                        // a.announcement_id = id;
                        a.audience = fall;
                        a.mesg_text = mesg;
                        //   a.attachment = attachment;
                        a.Sender_u_id = Session["id"].ToString();
                        a.dateTime = DateTime.Now;
                        a.status = "0";

                        db.Announcements.Add(a);


                        //id.ToString();
                        // ViewBag.annID = id;
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
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementStudent()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    ViewBag.ttid = Session["id"];

                    String subject = Request["sub"];
                    String mesg = Request["mesg_text"];
                    // String attachment = Request["attachment"];
                    //String audience = Request["a"]; // check on this if audience == "fall" then which fall, populate Student list acc.
                    //if audience == "degree" then which degree, populate Student list acc. 

                    //int id = -1;


                    var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");



                    String stid = Request["sid"];
                    var q = from x in db.students
                            where x.sid.Equals(stid)
                            select x;

                    foreach (var i in q)
                    {

                        //id = Int32.Parse(a_id);
                        //id = Convert.ToInt32(a_id);
                        //   id = id + 1;
                        var a = new Announcement();
                        a.destination = i.sid;
                        // a.announcement_id = id;
                        a.audience = i.batch;
                        a.mesg_text = mesg;
                        //   a.attachment = attachment;
                        a.Sender_u_id = Session["id"].ToString();
                        a.dateTime = DateTime.Now;
                        a.status = "0";

                        db.Announcements.Add(a);


                        // id.ToString();
                        //ViewBag.annID = id;
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
                    return View("TeacherHome");

                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");
        }
        public ActionResult AnnouncementDegFall()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    ViewBag.ttid = Session["id"];

                    String subject = Request["sub"];
                    String mesg = Request["mesg_text"];
                    // String attachment = Request["attachment"];
                    //String audience = Request["a"]; // check on this if audience == "fall" then which fall, populate Student list acc.
                    //if audience == "degree" then which degree, populate Student list acc. 

                    //int id = -1;
                    // String a_id;

                    //a_id = ViewBag.annID;
                    //int ann;

                    //a_id = db.Announcements.Max(p => p.announcement_id);


                    var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");



                    String fall = Request["fall"];
                    String degree = Request["degree"];

                    var q = from x in db.students
                            where x.batch.Equals(fall)
                            where x.degree.Equals(degree)
                            select x;

                    foreach (var i in q)
                    {

                        //id = Int32.Parse(a_id);
                        //id = Convert.ToInt32(a_id);
                        //id = id + 1;
                        var a = new Announcement();
                        a.destination = i.sid;
                        //a.announcement_id = id;
                        a.audience = fall + " " + degree;
                        a.mesg_text = mesg;
                        //   a.attachment = attachment;
                        a.Sender_u_id = Session["id"].ToString();
                        a.dateTime = DateTime.Now;
                        a.status = "0";

                        db.Announcements.Add(a);


                        //id.ToString();
                        //ViewBag.annID = id;
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
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementDegFallSection()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
                {
                    ViewBag.ttid = Session["id"];
                    String subject = Request["sub"];
                    String mesg = Request["mesg_text"];
                    // String attachment = Request["attachment"];
                    //String audience = Request["a"]; // check on this if audience == "fall" then which fall, populate Student list acc.
                    //if audience == "degree" then which degree, populate Student list acc. 

                    //int id = -1;
                    //  String a_id;

                    //a_id = ViewBag.annID;
                    //int ann;

                    //a_id = db.Announcements.Max(p => p.announcement_id);


                    var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");



                    String fall = Request["fall"];
                    String degree = Request["degree"];
                    String section = Request["section"];

                    var q = from x in db.students
                            where x.batch.Equals(fall)
                            where x.degree.Equals(degree)
                            where x.section.Equals(section)
                            select x;

                    foreach (var i in q)
                    {


                        var a = new Announcement();
                        a.destination = i.sid;

                        a.audience = fall + " " + degree;
                        a.mesg_text = mesg;

                        a.Sender_u_id = Session["id"].ToString();
                        a.dateTime = DateTime.Now;
                        a.status = "0";

                        db.Announcements.Add(a);


                        // id.ToString();
                        // ViewBag.annID = id;
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
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");
        }

        public ActionResult personalMesg2S()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");

        }
        public ActionResult personalMesg2T()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");

        }

        public ActionResult personalMesg()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            return RedirectToAction("signIn" , "Home");
        }


        public ActionResult personalMesg2S1()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
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
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");

        }

        public ActionResult personalMesg2T1()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                try
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
                        //   a.attachment = attachment;
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
                    return View("TeacherHome");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Exception";
                    ViewBag.message = e.Message;
                    return View("ErrorPage");
                }
            }
            else
                return RedirectToAction("signIn", "Home");

        }


        public ActionResult inbox()
        {
                if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
                {
                    try
                    {
                        string name = Session["id"].ToString();


                        // var a = cx.Announcements.Where(x => x.audience.Equals(name) ORDER BY (date) DESC );
                        var a = from x in db.Announcements
                                where x.destination.Equals(name) && (x.title.Equals("message") && (!(x.Sender_u_id.Equals("ExamBranch")))) //&& x.status.Equals("0")
                                select x;

                        return View(a.ToList());
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorType = "Exception";
                        ViewBag.message = e.Message;
                        return View("ErrorPage");
                    }
            }
            else
            {
                return RedirectToAction("signIn" , "Home");
            }
        }


        public FileResult ShowFile(String path)
        {
            string content = string.Empty;
            string contentType = "text/plain";
            return File(path, contentType);
            
        }
        public FileResult ShowPDFFile( String path)
        {
            string contentType = "application/pdf";
            return File(path, contentType);

        }
        public ActionResult ErrorPage()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("Teacher"))
            {
                return View();
            }
            else
                return RedirectToAction("signIn" , "Home");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
