using MVC_First.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlTypes;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;

namespace MVC_First.Controllers
{
    public class AdminController : Controller
    {
        private PucitDBEntities db = new PucitDBEntities();

        public ActionResult AdminHome()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult ErrorPage()
        {
            return View();
        }
        public ActionResult UserSetup()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult CourseConfiguration()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult UsersInfo()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                var model = from r in db.users
                            select r;
                return View(model);
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult CourseDetails()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                var model = from r in db.courses
                            select r;
                return View(model);
            }
            return RedirectToAction("signIn", "Home");
        }
        public JsonResult GetCourses()
        {
            var query = (from x in db.courses
                         select x);

            List<cours> data = new List<cours>();

            foreach (var q in query)
            {
                cours c = new cours();
                c.cid = q.cid;

                data.Add(c);

            }
            return this.Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTeachers()
        {
            var query = (from x in db.teachers
                         select x);

            List<teacher> data = new List<teacher>();

            foreach (var q in query)
            {
                teacher c = new teacher();
                c.tid = q.tid;

                data.Add(c);

            }
            return this.Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetStudents()
        {
            var query = (from x in db.students
                         select x);

            List<student> data = new List<student>();

            foreach (var q in query)
            {
                student c = new student();
                c.sid = q.sid;

                data.Add(c);

            }
            return this.Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AddCourse()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult EditCourseInfo()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult AssignCourseToTeacher()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult AssignCourseToStudent()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult AddUser()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult DeleteUser()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult EditUser()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult AddSingleUser()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult AddMultipleUsers()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                return View();
            }
            return RedirectToAction("signIn", "Home");
        }

        public ActionResult AssignCourse()
        {
            string tid = Request ["tid"];
            string cid = Request ["cid"];

            String fall = Request["fall"];
            String degree = Request["degree"];
            String section = Request["section"];
            
            string className = degree + fall + section;
           
            teacher t = db.teachers.SingleOrDefault(x => x.tid == tid);
           
            cours c = db.courses.SingleOrDefault(x=>x.cid == cid);

            teacherClass tc = new teacherClass();
            tc.className = className;
            tc.cid = cid;
            tc.tid = tid;

            db.teacherClasses.Add(tc);
            
            t.courses.Add(c);
            c.teachers.Add(t);

            db.SaveChanges();

            var query = (from x in db.courses.Where(f => f.teachers.Any())
                         from p in db.teachers.Where(g => g.courses.Contains(x))
                                select new { x.cid, x.name, p.tid });
            string str = "";
            foreach (var a in query)
            {
                str = str + a.tid + " " + a.name + " " + a.cid;
            }
            return View("CourseConfiguration");
        }
        public ActionResult AssignCourseStudent()
        {
            string sid = Request["sid"];
            string cid = Request["cid"];
           
            student s = db.students.SingleOrDefault(x => x.sid == sid);


            cours c = db.courses.SingleOrDefault(x => x.cid == cid);

            s.courses.Add(c);
            c.students.Add(s);
            db.SaveChanges();

            var query = (from x in db.students.Where(f => f.courses.Any())
                         from p in db.courses.Where(g => g.students.Contains(x))
                         select new { p.cid, p.name, x.sid });
            string str = "";
            foreach (var a in query)
            {
                str = str + a.sid + " " + a.name + " " + a.cid;
            }

            return View("CourseConfiguration");
            
        }

        public ActionResult SaveUser( user u )
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                var query = db.users.Where(x => x.uid.Equals(u.uid));
                string s = null;
                foreach (var a in query)
                {
                    s = s + a.uid + " " + a.name;
                }
                if (s == null)
                {
                    u.password = "123";
                    if (u.type.Equals("Teacher"))
                    {
                        teacher t = new teacher();
                        t.name = u.name;
                        t.password = u.password;
                        t.tid = u.uid;
                        t.email = u.email;
                        db.teachers.Add(t);
                    }
                    else
                    {
                        student st = new student();
                        st.name = u.name;
                        st.password = u.password;
                        st.sid = u.uid;
                        st.batch = u.uid.Substring(0, 3);
                        st.degree = "BCS";
                        //st.section = u.uid[6] == 'M' ? "Morning" : "Afternoon";
                        st.section = "Morning";
                        st.session = u.uid.Substring(0, 6);
                        st.email = u.email;
                        db.students.Add(st);
                    }
                    db.users.Add(u);
                    db.SaveChanges();
                    ViewBag.message = 1;
                    return View("AddUser");
                }
                else
                {
                    ViewBag.message = 0;
                    return View("AddSingleUser");
                }
            }
            return RedirectToAction("signIn", "Home");
            
        }
       
        public ActionResult Delete(user u)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                user u1 = db.users.Find(u.uid);
                if (u1 == null)
                {
                    return HttpNotFound();
                }
                return View(u1);
            }
            return RedirectToAction("signIn", "Home");
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(user u)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                user u1 = db.users.Find(u.uid);
                if (u1.type.Equals("Teacher"))
                {
                    teacher t = db.teachers.Find(u.uid);
                    db.teachers.Remove(t);
                }
                else if (u1.type.Equals("Student"))
                {
                    student s = db.students.Find(u.uid);
                    db.students.Remove(s);
                }
                db.users.Remove(u1);
                db.SaveChanges();
                return RedirectToAction("UserSetup");
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult Edit(user u)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                user u1 = db.users.Find(u.uid);
                if (u1 == null)
                {
                    return HttpNotFound();
                }
                return View(u1);
            }
            return RedirectToAction("signIn", "Home");
        }
        
        public ActionResult EditConfirmed(user u)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserSetup");
            }
            return RedirectToAction("signIn", "Home");
        }
        [HttpPost]
        public ActionResult SaveFile()
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                HttpPostedFileBase file = Request.Files[0];
                file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));

                var result = "";

                char[] delimiterChar = { ',' };
                string[] data = null;

                var dataFile = Server.MapPath(@"~\Files\" + file.FileName);

                FileStream fin = new FileStream(dataFile, FileMode.Open);
                StreamReader sr = new StreamReader(fin);
                result = sr.ReadLine();

                try
                {

                    while (result != null)
                    {
                        data = result.Split(delimiterChar);
                        user u = new user();
                        u.uid = data[0];
                        u.name = data[1];
                        u.type = data[2];
                        u.password = "123";
                        u.email = data[3];

                        var query = db.users.Where(x => x.uid.Equals(u.uid));
                        string s = null;
                        foreach (var a in query)
                        {
                            s = s + a.uid + " " + a.name;
                        }
                        if (s == null)
                        {
                            db.users.Add(u);
                            db.SaveChanges();

                            if (u.type.Equals("Student"))
                            {
                                student st = new student();
                                st.sid = u.uid;
                                st.name = u.name;
                                st.batch = u.uid.Substring(3, 3);
                                st.degree = u.uid.Substring(0, 3);
                                st.section = u.uid[6] == 'M' ? "Morning" : "Afternoon";
                                st.session = u.uid.Substring(0, 7);
                                st.password = u.password;
                                st.email = u.email;
                                db.students.Add(st);
                                db.SaveChanges();
                            }
                            else if (u.type.Equals("Teacher"))
                            {
                                teacher t = new teacher();
                                t.tid = u.uid;
                                t.name = u.name;
                                t.password = u.password;
                                t.email = u.email;
                                db.teachers.Add(t);
                                db.SaveChanges();
                            }
                        }

                        result = sr.ReadLine();
                        data = null;
                    }
                    sr.Close();
                    fin.Close();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorType = "UserAdd";
                    ViewBag.message = "File format is not correct";
                    sr.Close();
                    fin.Close();
                    return View("ErrorPage");
                }
                return View("AddUser");
            }
            return RedirectToAction("signIn", "Home");
        }
        [HttpPost]
        public ActionResult SaveCourse(cours c)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                var query = db.courses.Where(x => x.cid.Equals(c.cid));
                string s = null;
                foreach (var a in query)
                {
                    s = s + a.cid + " " + a.name;
                }
                if (s == null)
                {
                    db.courses.Add(c);
                    db.SaveChanges();
                }


                return View("CourseConfiguration");
            }
            return RedirectToAction("signIn", "Home");
        }
        [HttpPost]
        public ActionResult EditCourse( cours c)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                cours c1 = db.courses.Find(c.cid);
                if (c1 == null)
                {
                    return HttpNotFound();
                }
                return View(c1);
            }
            return RedirectToAction("signIn", "Home");
        }
        public ActionResult DeleteCourse(cours c)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                cours c1 = db.courses.Find(c.cid);
                db.courses.Remove(c1);
                db.SaveChanges();
                return RedirectToAction("CourseConfiguration");
            }
            return RedirectToAction("signIn", "Home");
        }
        [HttpPost]
        public ActionResult SaveCourseInfo(cours c)
        {
            if (Session["id"] != null && db.users.Find(Session["id"]).type.Equals("admin"))
            {
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CourseConfiguration");
            }
            return RedirectToAction("signIn", "Home");
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
