using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class TeacherModel
    {   
        [Display(Name = "Teacher Code")]
        [Required(ErrorMessage = "Please enter a valid teacher code")]
        public int Tcode { get; set; }
        [Display(Name = "First name")]
        [Required(ErrorMessage = "Please enter a valid first name")]
        public string Fname { get; set; }
        [Display(Name = "Infix")]
        public string Infix { get; set; }
        [Display(Name = "Student's last name")]
        [Required(ErrorMessage = "Please enter a valid last name")]
        public string Lname { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Provide a password of minimal 8 characters")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}