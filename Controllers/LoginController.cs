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
            string sql = "SELECT Tcode, Hashedpw, Salt FROM dbo.Teacher WHERE Tcode = @Tcode";
          
            List<string> Teacher = SqlDataAccess.LoadPasswordsWithTcode(sql, Tcode);
            


            return Teacher;
        }
            
        public bool IsValid(string Tcode, string Password)
        {
            
            List<string> teacher = GetTeacher(Tcode);
            string converted = teacher[2];
            byte[] array = Convert.FromBase64String(converted);    
            string hashedpw = enc.GetHashPw(Password, array);
            //enc.verifyhash(Password, array, hashedpw);
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