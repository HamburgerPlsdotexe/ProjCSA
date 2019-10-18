using DataLibrary.Models;
using DataLibrary.DataAccess;
using System;
using System.Collections.Generic;
using static DataLibrary.Logic.TeacherProcessor;
using System.Web.Mvc;
using System.Text;

namespace ProjectCSA.Controllers
{
    public class LoginController : Controller
    {
        readonly Pwenc enc = new Pwenc();

        public ActionResult Login()
        {
            ViewBag.Message = "Login";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Tcode, string Password)
        {
            if (IsValid(Tcode, Password))
            {
                return Redirect("~/Application/index");
            }

            else
            {
                return Redirect("Login");
            }
        }

        public List<string> GetTeacher(string Tcode)
        {
            string sql = "SELECT [Tcode], [Hashedpw], [Salt] FROM dbo.[Teacher] WHERE Tcode = @Tcode AND Hashedpw = Hashedpw  AND Salt = Salt";
            
            List<string> Danyel = SQLDataAccess.LoadTeachersWithTcode<string>(sql, Tcode);
            return Danyel;
        }

        public bool IsValid(string Tcode, string Password)
        {
            List<string> teacher = GetTeacher(Tcode);
            byte[] array = Encoding.ASCII.GetBytes(teacher[2]);
            string hashedpw = enc.GetHashPw(Password, array);

            if (hashedpw == teacher[1])
            {
                return true;
            }
            else
            {
                return false;
            }
         

        }
    }
}