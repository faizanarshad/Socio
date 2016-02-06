using MVC_First.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.Entity;
using System.Net.Mail;

namespace MVC_First.Controllers
{
    public class HomeController : Controller
    {
        PucitDBEntities db = new PucitDBEntities();
              
        public ActionResult index()
        {
            return View();
        }
          public ActionResult About()
        {
 
                return View();
        }
        
        public ActionResult signIn()
        {
 
                return View();
        }
        public ActionResult signUp()
        {

            return View();
        }

        public ActionResult Logout()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("signIn", "Home");
            }
            else
            {
                Session.Abandon();
                Session["id"] = null;
                Session["type"] = null;
                return RedirectToAction("signIn", "Home");
            }
        }

        public ActionResult RecoverPassword()
        {
            string uid = Request["uid"];
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                
                mail.From = new MailAddress("faizanarshad124@gmail.com");
                mail.Subject = "Recover Password";
                user u = db.users.Find(uid);
                mail.To.Add(u.email);
                string Body = "Your Information is as follows.    Username: "+u.uid+"     "+"Password: "+u.password;
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
                return RedirectToAction("signIn", "Home");
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }

        }

        public ActionResult changePassword()
        {
            string id = Session["id"].ToString();
            user u = db.users.Find(id);
            return View(u);
        }
        public JsonResult CheckPassword()
        {
            string passwd = Request["pass"].ToString();
            string id = Session["id"].ToString();

            var query = from x in db.users where x.uid == id select x;
            string oldp = "";

            foreach (var q in query)
                oldp = q.password;

            if (oldp.Equals(passwd))
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult setPassword( user u)
        {
            
            string newp = Request["newPassword"].ToString();

            string id = Session["id"].ToString();
            u.password = newp;
            u.uid = id;

            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.passwordChanged = "true";

            if (Session["type"] != null && Session["type"].ToString().Equals("admin"))
            {
                return RedirectToAction("AdminHome","Admin");
            }
            else if (Session["type"] != null && Session["type"].ToString().Equals("Teacher"))
            {
                return RedirectToAction("TeacherHome","Teacher");
            }
            else if (Session["type"] != null && Session["type"].ToString().Equals("Student"))
            {
                return RedirectToAction("StudentHome","Student");
            }
            else if (Session["type"] != null && Session["type"].ToString().Equals("ExamBranch"))
            {
                return RedirectToAction("ExamBranchHome","Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public JsonResult CheckLoginID()
        {

            string id = Request["uid"];
            
            var query = db.users.Find(id);

            if ( query == null )
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }

        
        }
        [HttpPost]
        public JsonResult CheckLoginIDs()
        {

            string id = Request["uid"];
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            var query = db.users.Find(id);

            if (query == null)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(true, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult checkInfo1(string username, string password, string account)
        {

            /*user u = new user();
            u.uid=username;
            u.password=password;
            u.type = account;*/
            string u = Request["username"];
            string p = Request["password"];


            string str = "";
            Response.AppendHeader("Access-Control-Allow-Origin", "null");
            var query = from x in db.users.Where(x => x.uid.Equals(u) && x.password.Equals(p)) select x;

            if (query != null)
            {
                foreach (var s in query)
                {
                    str = s.type;
                }

                if (str.Equals("admin"))
                {

                    Session["id"] = username;
                    Session["type"] = str;
                    return this.Json(true, JsonRequestBehavior.AllowGet);
                }
                else if (str.Equals("Student"))
                {

                    Session["id"] = username;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return this.Json(true, JsonRequestBehavior.AllowGet);

                }
                else if (str.Equals("Teacher"))
                {
                    Session["id"] = username;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return this.Json(true, JsonRequestBehavior.AllowGet);

                }
                else if (str.Equals("CustomUser"))
                {
                    Session["id"] = username;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return this.Json(true, JsonRequestBehavior.AllowGet);

                }


                else if (str.Equals("ExamBranch"))
                {
                    Session["id"] = username;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return this.Json(true, JsonRequestBehavior.AllowGet);

                }
            }
            return this.Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkInfo(user u)
        {

            string str = "";
            var query = from x in db.users.Where(x => x.uid.Equals(u.uid) && x.password.Equals(u.password)) select x;

            if (query != null)
            {
                foreach (var s in query)
                {
                    str = s.type;
                }

                if (str.Equals("admin"))
                {

                    Session["id"] = u.uid;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return RedirectToAction("AdminHome", "Admin");
                }
                else if (str.Equals("Student"))
                {

                    Session["id"] = u.uid;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return RedirectToAction("StudentHome", "Student");
                }
                else if (str.Equals("Teacher"))
                {
                    Session["id"] = u.uid;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return RedirectToAction("TeacherHome", "Teacher");
                }
                else if (str.Equals("CustomUser"))
                {
                    Session["id"] = u.uid;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return RedirectToAction("CustomUserHome", "Home");

                }
                else if (str.Equals("ExamBranch"))
                {
                    Session["id"] = u.uid;
                    Session["type"] = str;
                    Session.Timeout = 60;
                    return RedirectToAction("ExamBranchHome", "Home");

                }

            }

            return View("signIn");

        }
        public ActionResult CustomUserHome()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");


        }
        public JsonResult GetUsers()
        {
            var query = (from x in db.users where x.type != "Admin"
                         select x);

            List<user> data = new List<user>();

            foreach (var q in query)
            {
                user c = new user();
                c.uid = q.uid;
                
                data.Add(c);

            }
            return this.Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Courses()
        {
            if (Session["id"] != null && (db.users.Find(Session["id"]).type.Equals("Teacher") || db.users.Find(Session["id"]).type.Equals("Student")))
            {
                List<cours> data = new List<cours>();

                if (db.users.Find(Session["id"]).type.Equals("Teacher"))
                {
                    string tid = Session["id"].ToString();

                    var query = (from x in db.teachers.Where(f => f.tid.Equals(tid))
                                 from p in db.courses.Where(g => g.teachers.Contains(x))
                                 select p);


                    foreach (var q in query)
                    {
                        cours c = new cours();
                        c.cid = q.cid;
                        c.name = q.name;
                        c.creditHours = q.creditHours;
                        data.Add(c);

                    }

                }
                else
                {
                    string sid = Session["id"].ToString();

                    var query = (from x in db.students.Where(f => f.sid.Equals(sid))
                                 from p in db.courses.Where(g => g.students.Contains(x))
                                 select p);

                    foreach (var q in query)
                    {
                        cours c = new cours();
                        c.cid = q.cid;
                        c.name = q.name;
                        c.creditHours = q.creditHours;
                        data.Add(c);
                    }

                }

                return View(data);
            }
            else
            {
                ViewBag.message = "Please Login First";
                return View("SignIn");
            }
        }
        public ActionResult Faculty()
        {
            List<teacher> data = new List<teacher>();

            var query = from r in db.teachers

                                select r;

                    foreach (var q in query)
                    {
                        teacher c = new teacher();
                        c.tid = q.tid;
                        c.name = q.name;
                        c.email = q.email;
                        c.designation = q.designation;
                        data.Add(c);

                    }
            return View(data);
 
        }
         //=========================================


        public ActionResult Announcement()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementToFall()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementToDegFall()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementToDegFallSection()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementToStudent()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
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
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                ViewBag.ttid = Session["id"];
                /*  if (Request.Files[0].ToString().Equals(""))
                  {
                      HttpPostedFileBase file = Request.Files[0];


                      file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                      var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                  }*/

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
                return View("CustomUserHome");
            }

            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementStudent()
        {
            {
                if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
                {
                    ViewBag.ttid = Session["id"];

                    /*   if (Request.Files[0].ToString().Equals(""))
                       {

                           HttpPostedFileBase file = Request.Files[0];
                           file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                           var dataFile = Server.MapPath(@"~\Files\" + file.FileName);
                       }*/




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
                        //   id = Convert.ToInt32(a_id);
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
                    return View("CustomUserHome");
                }

                return RedirectToAction("signIn", "Home");
            }
        }




        public ActionResult AnnouncementDegFall()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                ViewBag.ttid = Session["id"];

                /*  if (Request.Files[0].ToString().Equals(""))
                  {
                      HttpPostedFileBase file = Request.Files[0];
                      file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                      var dataFile = Server.MapPath(@"~\Files\" + file.FileName);
                  }*/




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


                    //   id.ToString();
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
                return View("CustomUserHome");
            }

            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AnnouncementDegFallSection()
        {
            {
                if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("CustomUser") || Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
                {
                    ViewBag.ttid = Session["id"];

                    /*    if (Request.Files[0].ToString().Equals(""))
                        {
                            HttpPostedFileBase file = Request.Files[0];
                            file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                            var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                        }
                        */


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
                    return View("CustomUserHome");
                }

                return RedirectToAction("signIn", "Home");
            }
        }


        public ActionResult ExamBranchHome()
        {

            return View();
        }


        public ActionResult uploadSittingPlan()
        {

            return View();
        }

        public ActionResult saveSittingPlan()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {

                string result;
                HttpPostedFileBase file = Request.Files[0];
                file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                Announcement a = new Announcement();
                a.attachment = dataFile;
                a.Sender_u_id = "ExamBranch";

                //   db.Announcements.Add(a);

                char[] delimiterChar = { ',' };
                string[] data = null;

                System.IO.FileStream fin = new FileStream(dataFile, FileMode.Open);
                StreamReader sr = new StreamReader(fin);
                result = sr.ReadLine();

                try
                {

                    while (result != null)
                    {
                        data = result.Split(delimiterChar);
                        Announcement a1 = new Announcement();
                        a1.Sender_u_id = "ExamBranch";

                        a1.destination = data[0].ToString();
                        string[] str = data[1].Split(' ');
                        string[] time = str[1].Split(':');
                        string[] date = str[0].Split('/');
                        string tt = str[2];

                        if (tt.Equals("AM"))
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), 0);
                            a1.dateTime = dt;
                        }
                        else
                        {
                            DateTime dt = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(time[0]) + 12, Convert.ToInt32(time[1]), 0);
                            a1.dateTime = dt;
                        }

                        // string seat = data[4];
                        a1.mesg_text = data[2].ToString() + " " + data[3].ToString() + " " + data[4].ToString();
                        db.Announcements.Add(a1);

                        result = sr.ReadLine();
                        data = null;
                        db.Announcements.Add(a1);


                    }
                    sr.Close();
                    fin.Close();

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "Upload Sitting Plan";
                    ViewBag.message = "File format is not correct";
                    sr.Close();
                    fin.Close();
                    return View("ErrorPage");
                }
                return View("ExamBranch");
            }
            return RedirectToAction("signIn", "Home");

        }
        public ActionResult UploadDatesheet()
        {

            return View();
        }
        public ActionResult saveDatesheet()
        {

            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("ExamBranch"))
            {
                HttpPostedFileBase file = Request.Files[0];

                file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                string fall = Request["fall"];
                string degree = Request["degree"];
                string tid = Session["id"].ToString();

                DateSheet d1 = new DateSheet();
                d1.className = degree + fall;
                DateSheet d2  = db.DateSheets.Find(d1.className);
                
                if (d2 != null)
                {
                    db.DateSheets.Remove(d2);
                    db.SaveChanges();
                }

                d1.filePath = dataFile;

                db.DateSheets.Add(d1);
                db.SaveChanges();

                return RedirectToAction("ExamBranchHome");
            }
            else
            {
                return RedirectToAction("signIn", "Home");
            }
        }

    }
    
}
