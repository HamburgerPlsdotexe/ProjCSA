using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class StudentModel
    {
        [Display(Name = "Student Number")]
        public string Snum { get; set; }
        [Display(Name = "First Name")]

        public string Fname { get; set; }
        [Display(Name = "Last Name")]

        public string Lname { get; set; }
        [Display(Name = "Class Number")]

        public string cnum { get; set; }

        public string Password { get; set; }

        public string confirmPassword { get; set; }


    }
}