using System.Collections.Generic;
using System.Web.Mvc;
using static DataLibrary.Logic.TeacherProcessor;
using static DataLibrary.Logic.StudentProcessor;
using static DataLibrary.Logic.ClassProcessor;
using static ProjectCSA.DateOperations;
using ProjectCSA.Models;
using DataLibrary.DataAccess;
using System.Web.Security;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Hosting;
using QRCoder;
using System.Drawing;
namespace ProjectCSA.Controllers
{
    
    [Authorize]
    public class ApplicationController : Controller
    {

        public ActionResult ScheduleNextWeek() // Increments the week with one so that mvc displays the next week of a teacher's schedule
        {
            int n = 1; 
            Index(n);
            return View("Index");
        }
        public ActionResult SchedulePreviousWeek() // Same as the previous method but inverted
        {
            int n = 2;
            Index(n);
            return View("Index");
        }
       
        
        public ActionResult qr_button_Click(object sender, EventArgs e)
        {
            //This variable is the input for the qr-code, which should be pulled from the database instead of being an on-click event
            string Code = "SomeCode";
            QRCodeGenerator qrgenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrgenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            System.Web.UI.WebControls.Image imgQRcode = new System.Web.UI.WebControls.Image();
            imgQRcode.Width = 500;
            imgQRcode.Height = 500;

            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgQRcode.ImageUrl = "data:image/png;base64, " + Convert.ToBase64String(byteImage);
                }
                ViewData["QRCodeImage"] = imgQRcode.ImageUrl;
                return View("TestQR"); 
            }
        }
        public string GetUserTcode()
        {
            var username = User.Identity.Name;              //space after name 'knyee ' messes up system.
            return username;
        }


        readonly Pwenc penc = new Pwenc();
        public ActionResult ViewStudentsTemp()
        {
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

        public ActionResult Index(int direction=3)
        {
           
            if (direction == 1)
            {
                SetWeeks(1);
            }
            if (direction == 2)
            {
                SetWeeks(2);
            }

            
            string Tcode = GetUserTcode();
            List<ScheduleModel> model = new List<ScheduleModel>();
            if (User.Identity.Name == "Admin")
            {
                return View();
            }
            else
            {
                List<ScheduleModel> list = RetrieveValuesFromJson(Tcode);
                foreach (var row in list)
                {
                    model.Add(new ScheduleModel
                    {
                        Week = row.Week,
                        LessonCode = row.LessonCode,
                        Day = row.Day,
                        Classroom = row.Classroom,
                        Hours = row.Hours,
                        Class = row.Class
                    });

            }
                ViewData["weeks"] = GetWeeks();
                return View(model);
            }
        }
           

        public List<ScheduleModel> RetrieveValuesFromJson(string Tcode)
        {
            string jsonPath = HostingEnvironment.MapPath($@"~/Content/{Tcode}.json");
            using StreamReader f = new StreamReader(jsonPath);
            string json = f.ReadToEnd();
            JArray js = JArray.Parse(json);
            var array = js.ToObject<List<ScheduleModel>>();
            return array;


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
        public ActionResult SignUp(TeacherModel model) // When signup is done, redirect to index with correct cookie, doesn't have json yet. 
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
        public ActionResult ReturnStudentListViewWithCnum(string Cnum)
        {
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
            if(model.Students.Count==0)
            {
                TempData["Temporary"] = "No students where found!";
                return View("ViewStudentsTemp", model);

            }
            else
            {
                return View("ViewStudentsTemp", model);
            }

        }

    }
}