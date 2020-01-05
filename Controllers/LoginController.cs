using DataLibrary.Models;
using DataLibrary.DataAccess;
using System;
using System.Collections.Generic;
using static DataLibrary.Logic.TeacherProcessor;
using System.Web.Mvc;
using System.Text;
using System.Web;
using System.Web.Security;

namespace ProjectCSA.Controllers
{

    public class LoginController : Controller
    {
        public static string Tcode;
        readonly EncOperations pwenc = new EncOperations();

        public static string ReturnTcode()
        {
            return Tcode;
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Login";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string tcode, string Password)
        {
            Tuple<bool, string> Answer = IsValid(tcode, Password);
            if (Answer.Item1)
            {
                Tcode = tcode;
                if (IsAdmin(tcode))
                {                                                       //Admin
                    FormsAuthentication.SetAuthCookie("Admin", false);
                    return Redirect("~/Application/ViewTeachers");
                }
                else
                {                                                       //User
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, tcode, DateTime.Now, DateTime.Now.AddMinutes(20), true, tcode); //change false to true later for different sessions.
                    //Creates a 'ticket' with information about the current user.
                    string encticket = FormsAuthentication.Encrypt(ticket); //encrypt ticket
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encticket)); //put ticket in cookie, send cookie to user with content of site
                    Tcode = ticket.Name;
                    return Redirect("~/Application/Index");
                }
            }

            else
            {
                TempData["Temp"] = Answer.Item2;
                return Redirect("Login");
            }
        }

        public List<string> GetTeacher(string Tcode)
        {
            string sql = "SELECT Tcode, Hashedpw, Salt FROM dbo.Teacher WHERE Tcode = @Tcode";
            List<string> Teacher = SqlDataAccess.LoadPasswordsWithTcode(sql, Tcode);
            return Teacher;
        }

        public Tuple<bool,string> IsValid(string Tcode, string Password)
        {
            if (Password == "")
            {
                Tuple<bool, string> Error = new Tuple<bool, string>(false, "Password cannot be empty");
                return Error;
            }
            if (Tcode.Length != 5)
            {
                Tuple<bool, string> Error = new Tuple<bool, string>(false, "The teacher code does not exist");
                return Error;
            }
            else
            {
                List<string> teacher = GetTeacher(Tcode);
                if (teacher[0] == "False")
                {
                    Tuple<bool, string> Error = new Tuple<bool, string>(false, "The teacher code does not exist");
                    return Error;
                }
                else
                {
                    string converted = teacher[2];
                    byte[] array = Convert.FromBase64String(converted);
                    string hashedpw = pwenc.GetHashPw(Password, array);
                    if (hashedpw == teacher[1])
                    {
                        Tuple<bool, string> Success = new Tuple<bool, string>(true, "Logging in...");
                        return Success;
                    }
                    else
                    {
                        Tuple<bool, string> Error = new Tuple<bool, string>(false, "The password is not correct");
                        return Error;
                    }
                }
            }
        }

        public static bool IsAdmin(string Tcode)
        {
            if (Tcode == null)
            {
                return false;
            }
            else
            {
                string sql = "SELECT Tcode, Flag FROM dbo.Teacher WHERE Tcode = @Tcode";
                List<string> teacher = SqlDataAccess.LoadPermissionsWithTCode(sql, Tcode);
                if (teacher[1] == "adm")
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
}