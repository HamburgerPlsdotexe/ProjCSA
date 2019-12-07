﻿using System.Collections.Generic;
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
using Newtonsoft.Json;
using System.Linq;

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

        public ActionResult QR_Button_Click(string LessonCode, object sender, EventArgs e)
        {
            //This variable is the input for the qr-code, which should be pulled from the database instead of being an on-click event
            string Code = LessonCode;

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
        public ActionResult SignUp(TeacherModel model)                                              // When signup is done, redirect to index with correct cookie, doesn't have json yet. 
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

                    PopulateJson(model.Tcode);
                    return RedirectToAction("Login", "Login");                                      //unique account, continue creation.      
                }
                else
                {
                    ViewData["ErrorMessage"] = "That teacher code already exists!";
                    return View("SignUp");                                                          //Not a unique Tcode, redirect to signup
                }

            }
            ViewData["ErrorMessage"] = "Teachercode is too long!";
            return View("SignUp");                                                                  //other error, redirect to signup
        }

        public Random rand = new Random();

        public int ReturnWeeks()
        {
            var exclude = new HashSet<int>() { 1, 9, 18, 28, 29, 30, 31, 32, 33, 42, 52 };
            IEnumerable<int> range = Enumerable.Range(1, 52).Where(i => !exclude.Contains(i));
            int maxvalue = 52 - exclude.Count;
            int index = rand.Next(0, maxvalue);
            int weekN = range.ElementAt(index);
            
            return weekN;

        }

        public void PopulateJson(string tcode)
        {
            string[] Days = new string[7] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };     //days of the week.
            int ZeroToFiveOuter = rand.Next(0, 6);

            string[] Classes = new string[5] { "INF2A", "INF2B", "INF2C", "INF2D", "INF2E" };

            string[] ClassRooms = new string[] { $"H.{ZeroToFiveOuter}.114", $"WD.{ZeroToFiveOuter}.002", $"WD.{ZeroToFiveOuter}.016", $"H.{ZeroToFiveOuter}.403", $"H.{ZeroToFiveOuter}.315", $"H.{ZeroToFiveOuter}.308", $"H.{ZeroToFiveOuter}.306", $"H.{ZeroToFiveOuter}.206", $"H.{ZeroToFiveOuter}.204", $"H.{ZeroToFiveOuter}.110", $"H.{ZeroToFiveOuter}.405" };
            List<ScheduleModel> _data = new List<ScheduleModel>();

            for (int i = 0; i < 250; i++)
            {
                string[] integers = new string[250];
                integers[i] = ReturnWeeks().ToString();
                int OneToFive = rand.Next(1, 6);                                                                                            //pick random day of the week.
                int maxvalue = ClassRooms.Length;                                                                                           //random value from array.
                int ClassRoomInt = rand.Next(0, maxvalue);                                                                                  //random int to pick from array.
                int Hours = rand.Next(1, 13);                                                                                               //pick random hour of the day between 1 and 13, and add the next to upcoming hours into the array.
                int[] hours = new int[] { Hours, Hours + 1, Hours + 2 };
                string ClassCode = Classes[OneToFive - 1];
                
                ScheduleModel tempmodel = new ScheduleModel()
                {
                    Week = integers[i],
                    Day = Days[OneToFive],
                    Classroom = ClassRooms[ClassRoomInt],
                    Hours = hours,
                    Class = ClassCode,
                    LessonCode = ClassCode + "-" + integers[i] + "-" + DateTime.Now.Date.ToString("d")                  //INF1I-49-Date

                };
                if (DoesExist(_data, tempmodel))
                {
                    _data.Add(tempmodel);
                }
                else
                {

                }
            };

            string json = JsonConvert.SerializeObject(_data.ToArray());
            string Tcode = tcode;
            System.IO.File.WriteAllText(HostingEnvironment.MapPath($@"~/Content/{Tcode}.json"), json);
        }
        public static bool DoesExist(List<ScheduleModel> _data, ScheduleModel tempmodel)
        {
            bool isEmpty = !_data.Any();
            if (isEmpty)
            {
                return true;
            }
            bool alreadyExists = _data.Any(x => x.Day == tempmodel.Day && x.Week == tempmodel.Week && x.Hours.Intersect(tempmodel.Hours).Any()
);
            if (alreadyExists)
            {
                return false;
            }
            else
            {
                return true;
            }
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