using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class StudentModel
    {
        public string userStudentNum { get; set; }
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string userEmail { get; set; }
        public string userClass { get; set; }


    }
}