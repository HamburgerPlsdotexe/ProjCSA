using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DataLibrary.Logic.TeacherProcessor;



namespace ProjectCSA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult ViewTeachers()
        {
            ViewBag.Message = "Teachers List";

            var data = LoadTeachers();
            List<TeacherModel> teachers = new List<TeacherModel>();
            
            foreach (var row in data)
            {
                teachers.Add(new TeacherModel
                {
                    Tcode = row.Tcode,
                    Fname = row.Fname,
                    Infix = row.Infix,
                    Lname = row.Lname
                });
            }
            return View(teachers);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SignUp()
        {
            ViewBag.Message = "Teacher Sign Up";
       
            return View();
        }   
        
        public ActionResult SignIn()
        {
            ViewBag.Message = "Teacher Sign in";

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(TeacherModel model)
        {
            if (ModelState.IsValid) 
            {

                int RecordsCreated = CreateTeacher(model.Tcode,
                    model.Fname,
                    model.Infix,
                    model.Lname);

                return RedirectToAction("index");
            }
            return View();
        }

        public ActionResult ViewTeachers()
        {
            ViewBag.Message = "Teacher List";

            var data = LoadTeachers();

            List<TeacherModel> teachers = new List<TeacherModel>();
            foreach(var row in data)
            {
                teachers.Add(new TeacherModel
                {
                    Tcode = row.Tcode,
                    Fname = row.Fname,
                    Lname = row.Lname,
                });
            }

            return View(teachers);
        }

    }
}