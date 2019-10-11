
using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class StudentModel
    {

        public string Snum { get; set; }

        public string Fname { get; set; }

        public string Lname { get; set; }

        public string cnum { get; set; }

        public string Password { get; set; }

        public string confirmPassword { get; set; }


    }
}