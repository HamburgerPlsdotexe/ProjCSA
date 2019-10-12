using static DataLibrary.Logic.TeacherProcessor;
using static DataLibrary.Logic.StudentProcessor;
using static DataLibrary.Models.TeacherModel;
using static DataLibrary.Models.StudentModel;
using static ProjectCSA.Models.TeacherModel;
using ProjectCSA.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ProjectCSA.Controllers

{
    public class HomeController : Controller
    {
        Pwenc penc = new Pwenc();
        public ActionResult Index()
        {
            ViewBag.Message = "Home page";

            var data = LoadStudents();

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in data) 
            {
                student.Add(new StudentModel
                {
                    Snum = row.Snum,
                    Fname = row.Fname,
                    Lname = row.Lname,
                    cnum = row.cnum
                });
            }

            return View(student);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(TeacherModel model)
        {
            if (ModelState.IsValid)
            {
                int recordsCreated = CreateTeacher(
                    model.Tcode,
                    model.Fname,
                    model.Lname,
                    model.Password = penc.Run(model.Password));
                return RedirectToAction("index");
            }
            return View();
        }

        public ActionResult ViewTeachers()
        {
            ViewBag.Message = "Teacher List";

            var data = LoadTeachers();

            List<TeacherModel> teachers = new List<TeacherModel>();
            foreach (var row in data)
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