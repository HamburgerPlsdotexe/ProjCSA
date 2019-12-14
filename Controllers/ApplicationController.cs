using System.Collections.Generic;
using System.Web.Mvc;
using static DataLibrary.Logic.TeacherProcessor;
using static DataLibrary.Logic.StudentProcessor;
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
using System.Linq;
using System.Threading.Tasks;
//using Firebase.Database;
//using Firebase.Database.Query;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FireSharp;

namespace ProjectCSA.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        public ActionResult ScheduleNextWeek() // Increments the week with one so that mvc displays the next week of a teacher's schedule
        {
            int n = 1;
            Index(n);
            return RedirectToAction("Index");
        }
        public ActionResult SchedulePreviousWeek() // Same as the previous method but inverted
        {
            int n = 2;
            Index(n);
            return RedirectToAction("Index");
        }

        public ActionResult QR_Button_Click(string LessonCode)
        {
            //This variable is the input for the qr-code, which should be pulled from the database instead of being an on-click event
            string Code = LessonCode;

            QRCodeGenerator qrgenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrgenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            System.Web.UI.WebControls.Image imgQRcode = new System.Web.UI.WebControls.Image
            {
                Width = 500,
                Height = 500
            };

            using Bitmap qrCodeImage = qrCode.GetGraphic(20);
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
            qrCodeData.Dispose();
            return View("TestQR");
        }
        public string GetUserTcode()
        {
            var username = User.Identity.Name;              //space after name 'knyee ' messes up system.
            return username;
        }

        public List<StudentModel> studentList;
        public async Task<ActionResult> LoadStudentList()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "S1VfQJeuwZzyPI6KVQgJATyvqnRL995HnR4xkqj2",
                BasePath = "https://studentpre-a7d96.firebaseio.com/"
            };

            IFirebaseClient client = new FirebaseClient(config);

            FirebaseResponse response = await client.GetAsync("Students");
            List<StudentModel> StudentList = response.ResultAs<List<StudentModel>>();
            studentList = StudentList;
            return View(StudentList);
            /*
            var auth = "S1VfQJeuwZzyPI6KVQgJATyvqnRL995HnR4xkqj2";
            var firebase = new FirebaseClient(
                "https://studentpre-a7d96.firebaseio.com",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth)
                });
            var firebase2 = new FirebaseClient("https://studentpre-a7d96.firebaseio.com");
            var students = await firebase2
                .Child("Students")
                .OrderByKey()
                .OnceAsync<StudentModel>();

            var Studentlist = new List<StudentModel>();
            foreach (var stud in students)
            {
                Studentlist.Add(stud.Object);
            }
            
            Studentlist.Add(new StudentModel
            {
                Fname = "First",
                Lname = "Last",
                Snum = "0110",
                Cnum = "INF2C",
                Password = "Pass",
                ConfirmPassword = "Pass"
            });
            
            ViewBag.students = Studentlist.OrderByDescending(x => x);
            studentList = Studentlist;
            return View(Studentlist);
            */
        }

        readonly EncOperations pwenc = new EncOperations();
        public ActionResult ViewStudentsTemp()
        {
            StudentsAndClassesModel model = new StudentsAndClassesModel();
            LoadStudentList();
            var data = studentList;
            var data2 = LoadClasses();

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in data)
            {
                student.Add(new StudentModel
                {
                    userStudentNum = row.userStudentNum,
                    userFirstName = row.userFirstName,
                    userLastName = row.userLastName,
                    userClass = row.userClass,
                    userEmail = row.userEmail
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
        public ActionResult SignUp(TeacherModel model)                                              // When signup is done, redirect to index with correct cookie. 
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
            ViewData["ErrorMessage"] = "Something went wrong!";
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
        public ActionResult ReturnStudentListViewWithCnum(string Cnum, string CCode)
        {
            string ccode = CCode;
            StudentsClassesLessonCode model = new StudentsClassesLessonCode();
            var studload = LoadStudentList();
            studload.Wait();
            var data = studentList;
            var data2 = LoadClasses();

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in data)
            {
                if (row.userClass == Cnum)
                {
                    student.Add(new StudentModel
                    {
                        userStudentNum = row.userStudentNum,
                        userFirstName = row.userFirstName,
                        userLastName = row.userLastName,
                        userClass = row.userClass,
                        userEmail = row.userEmail
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
            model.ClassCode = ccode;

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