using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class StudentModel
    {
        [Display(Name = "Class Number")]
        public string userClass { get; set; }
        public string userEmail { get; set; }
        [Display(Name = "First Name")]
        public string userFirstName { get; set; }
        [Display(Name = "Last Name")]
        public string userLastName { get; set; }
        [Display(Name = "Student Number")]
        public string userStudentNum { get; set; }


    }
}

