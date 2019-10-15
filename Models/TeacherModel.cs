using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class TeacherModel
    {
        [Display(Name = "Teacher code")]
        [Required(ErrorMessage = "You need to enter a valid teacher code")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "You need to enter a valid teacher code")]
        public string Tcode { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "Enter your first name")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Please enter a valid name")]
        public string Fname { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Enter your last name")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Please enter a valid last name")]
        public string Lname { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter a password")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "You need to enter a password (more than 8 characters)")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords must match")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The password must match")]
        public string ConfirmPassword { get; set; }

        public string Salt { get; set; }

        public string Salt { get; set; }



    }
}