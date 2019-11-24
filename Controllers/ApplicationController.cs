using System.Collections.Generic;
using System.Web.Mvc;
using static DataLibrary.Logic.TeacherProcessor;
using static DataLibrary.Logic.StudentProcessor;
using static DataLibrary.Logic.ClassProcessor;
using ProjectCSA.Models;
using DataLibrary.DataAccess;
using System.Web.Security;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Hosting;


namespace ProjectCSA.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {

        public string Foenc()
        {
            var username = User.Identity.Name;              //space after name 'knyee ' messes up system.
            return username;
        }


        readonly Pwenc penc = new Pwenc();
        public ActionResult ViewStudentsTemp()
        {
            string Tcode = Foenc();

            StudentsAndClassesModel model = new StudentsAndClassesModel();
            var data = LoadStudents();
            var data2 = LoadClasses();

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in data)
            {
                student.Add(new StudentModel
                {
                    Snum = row.Snum,
                    Fname = row.Fname,
                    Lname = row.Lname,
                    Cnum = row.Cnum
                });
            }
            List<ClassModel> classes = new List<ClassModel>();
            foreach (var row in data2)
            {
                classes.Add(new ClassModel
                {
                    Cnum = row.Cnum
                });
            }
            model.Classes = classes;
            model.Students = student;


            return View(model);
        }

        public ActionResult Index()
        {
            Dino(Foenc());
            ViewBag.Message = "Schedule";

            return View();
        }

        public void Dino(string Tcode)
        {
            string jsonPath = HostingEnvironment.MapPath($@"~/Content/{Tcode}.json");

            using StreamReader f = new StreamReader(jsonPath);
            string json = f.ReadToEnd();
            JArray js = JArray.Parse(json);

            foreach (var entry in js)
            {
                Console.WriteLine(entry);
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Login", "Login");
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

        [AllowAnonymous]
        public ActionResult SignUp()
        {
            ViewBag.Message = "Teacher Sign Up";

            return View();
        }

        public bool DoesTcodeExist(string Tcode)
        {
            string sql = "SELECT Tcode FROM dbo.Teacher WHERE Tcode = @Tcode";
            var data = SqlDataAccess.LoadTcodes(sql, Tcode);

            if (data == null)     //does not exist
            {
                return true;
            }
            else                  //does exist
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SignUp(TeacherModel model)
        {

            if (ModelState.IsValid)
            {

                if (DoesTcodeExist(model.Tcode))
                {
                    string[][] Encrypted = new string[][] { penc.Run(model.Password) };


                    CreateTeacher(
                    model.Tcode,
                    model.Fname,
                    model.Lname,
                    model.Password = Encrypted[0][0],
                    model.Salt = Encrypted[0][1],
                    model.Flag = "usr");


                    return RedirectToAction("index");                           //unique account, continue creation.      
                }
                else
                {
                   // if (model.Tcode != null && model.Tcode.Length == 5 && DoesTcodeExist(model.Tcode))
                   // {
                        
                    //}
                    ViewData["ErrorMessage"] = "That teacher code already exists!";
                    return View("SignUp");                                      //Not a unique Tcode, redirect to signup
                }
                
            }
            ViewData["ErrorMessage"] = "Something went wrong, try again!";

            return View("SignUp");                                              //other error, redirect to signup


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


            if (LoginController.IsAdmin(LoginController.ReturnTcode()) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View(teachers);
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult OnClick(string Cnum)
        {
            ViewBag.Message = "Home page";

            StudentsAndClassesModel model = new StudentsAndClassesModel();
            var data = LoadStudents();
            var data2 = LoadClasses();

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in data)
            {
                if (row.Cnum == Cnum)
                {
                    student.Add(new StudentModel
                    {
                        Snum = row.Snum,
                        Fname = row.Fname,
                        Lname = row.Lname,
                        Cnum = row.Cnum
                    });
                }
            }
            List<ClassModel> classes = new List<ClassModel>();
            foreach (var row in data2)
            {
                classes.Add(new ClassModel
                {
                    Cnum = row.Cnum
                });
            }
            model.Classes = classes;
            model.Students = student;

            return View("ViewStudentsTemp", model);

        }

    }
}