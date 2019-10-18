﻿using System.Collections.Generic;
using System.Web.Mvc;
using static DataLibrary.Logic.TeacherProcessor;
using static DataLibrary.Logic.StudentProcessor;
using static DataLibrary.Logic.ClassProcessor;
using ProjectCSA.Models;

namespace ProjectCSA.Controllers
{
    public class ApplicationController : Controller
    {
        readonly Pwenc penc = new Pwenc();
        public ActionResult Index()
        {
            ViewBag.Message = "Home page";

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
                    cnum = row.Cnum
                });
            }
            model.Classes = classes;
            model.Students = student;


            return View(model);
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
                CreateTeacher(
                    model.Tcode,
                    model.Fname,
                    model.Lname,
                    model.Password = penc.Run(model.Password, true),
                    model.Salt = penc.Run(model.Password, false));

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
        public ActionResult OnClick(string cnum)
        {
            ViewBag.Message = "Home page";

            StudentsAndClassesModel model = new StudentsAndClassesModel();
            var data = LoadStudents();
            var data2 = LoadClasses();

            List<StudentModel> student = new List<StudentModel>();
            foreach (var row in data)
            {
                if (row.Cnum == cnum)
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
                    cnum = row.Cnum
                });
            }
            model.Classes = classes;
            model.Students = student;

            return View("Index", model);

        }

    }
}