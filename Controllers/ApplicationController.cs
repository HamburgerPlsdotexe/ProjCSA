using System.Collections.Generic;
using System.Web.Mvc;
using static ProjectCSA.Utility.FireBaseOperations;
using static DataLibrary.Logic.TeacherProcessor;
using static DataLibrary.Logic.ClassProcessor;
using static ProjectCSA.DateOperations;
using static ProjectCSA.JsonOperations;
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
        readonly EncOperations pwenc = new EncOperations();

        public ActionResult ScheduleNextWeek()      // Increments the week with one so that index displays the next week of a teacher's schedule
        {
            int n = 1;
            Index(n);
            return RedirectToAction("Index");
        }
        public ActionResult SchedulePreviousWeek()  // Same as the previous method but inverted
        {
            int n = 2;
            Index(n);
            return RedirectToAction("Index");
        }

        public string ReturnDateOfWeek(int year, int weekOfYear, int dayofweek)
        {
            DateTime[] array = ReturnDatesOfWeekArray(year, weekOfYear);
            string Output = array[dayofweek].ToString("dd/MM/yyyy");
            return Output;
        }

        public ActionResult QR_Button_Click(string LessonCode)
        {
            QRCodeGenerator qrgenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrgenerator.CreateQrCode(LessonCode, QRCodeGenerator.ECCLevel.Q);
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
                imgQRcode.Dispose();
                qrCodeData.Dispose();
                qrgenerator.Dispose();
                qrCode.Dispose();
                qrCodeData.Dispose();
                return View("TestQR");
            }
        }
        public string GetUserTcode()
        {
            var username = User.Identity.Name;              //space after name 'knyee ' messes up system.  FIXED
            return username;
        }

        public bool DoesJsonExists(string Tcode)
        {
            string TCode = Tcode.ToUpper();
            string relativepath = $@"~/Content/{TCode}.json";
            var absolutepath = HttpContext.Server.MapPath(relativepath);
            if (System.IO.File.Exists(absolutepath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Index(int direction = 3)
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
                if (!DoesJsonExists(Tcode))
                {
                    PopulateJson(Tcode);
                }
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
        public ActionResult SignUp(TeacherModel model)                                              // When signup is done, redirect to index with correct cookie, doesn't have json yet. 
        {
            if (ModelState.IsValid)
            {

                if (DoesTcodeExist(model.Tcode))
                {
                    string[][] Encrypted = new string[][] { pwenc.Run(model.Password) };

                    CreateTeacher(
                    model.Tcode,
                    model.Fname,
                    model.Lname,
                    model.Password = Encrypted[0][0],
                    model.Salt = Encrypted[0][1],
                    model.Flag = "usr");

                    PopulateJson(model.Tcode);
                    return RedirectToAction("Login", "Login");                                      //unique account, continue creation.      
                }
                else
                {
                    ViewData["ErrorMessage"] = "That teacher code already exists!";
                    return View("SignUp");                                                          //Not a unique Tcode, redirect to signup
                }

            }
            if (model.Tcode == null ||  model.Tcode.Length != 5  )
            {
                ViewData["ErrorMessage"] = "Your teacher code has to be 5 characters long!";
            }
            return View("SignUp");                                                                  //other error, redirect to signup
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

        public ActionResult ReturnStudentListViewWithCnum(string day, string week, string userClass, string CCode)
        {
            string Tcode = GetUserTcode();
            DateTime dateAndTime = DateTime.Now;
            string date = dateAndTime.ToString("dd-MM-yyyy");
            string Code = CCode + "-" + day + "-" + week + "-" + Tcode + "-" + date; //lessoncode 

            StudentsClassesLessonCode model = new StudentsClassesLessonCode();

            var StudentData = RetrieveStudents();
            var ClassesData = LoadClasses();
             

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in StudentData)
            {
                if (row.userClass == userClass)
                {
                    student.Add(new StudentModel
                    {
                        userStudentNum = row.userStudentNum,
                        userFirstName = row.userFirstName,
                        userLastName = row.userLastName,
                        userClass = row.userClass
                               
                    });
                }
            }


            List<ClassModel> classes = new List<ClassModel>();
            foreach (var row in ClassesData)
            {
                classes.Add(new ClassModel
                {
                    Cnum = row.Cnum
                });
            }
           

            model.Classes = classes;
            model.Students = student;
            model.ClassCode = Code;
            try
            {
                model.Attendance = RetrieveAbsence(Code);
            }
            catch
            {
            }
            if (model.Students.Count == 0)
            {
                TempData["Temporary"] = "No students where found!";
                return View("ViewStudentsTemp", model);
            }
            else
            {
                TempData["ClassCode"] = model.Classes[0];
                return View("ViewStudentsTemp", model);
            }

        }
    }
}