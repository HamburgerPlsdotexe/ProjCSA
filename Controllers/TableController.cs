using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace ProjectCSA.Models
{
    public class TableController : Controller
    {
        // GET: StudentModel
        public async Task<ActionResult> Index()
            {
                var firebase = new FirebaseClient("https://studentpre-a7d96.firebaseio.com");
                var students = await firebase
                    .Child("Students")
                    .OrderByKey()
                    .OnceAsync<StudentModel>();

                var Studentlist = new List<StudentModel>();

                foreach (var stud in students)
                {
                    Studentlist.Add(stud.Object);

                }
/*
            Studentlist.Add(new StudentModel
            {
                Fname = "First",
                Lname = "Last",
                Snum = "0110",
                Cnum = "INF2C",
                Password = "Pass",
                ConfirmPassword = "Pass"
            });
            */
                ViewBag.students = Studentlist.OrderByDescending(x => x);

                return View(Studentlist);

            }
        }
    
}