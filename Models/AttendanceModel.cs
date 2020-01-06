using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class AttendanceModel
    {
        [Display(Name = "Attendance")]
        public string Present { get; set; }
        [Display(Name = "Student Number")]
        public string StudentNumber { get; set; }
    }
}