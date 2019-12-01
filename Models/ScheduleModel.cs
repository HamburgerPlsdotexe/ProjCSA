using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class ScheduleModel
    {
        [Display(Name = "Lesson Code")]
        public string LessonCode { get; set; }
        [Display(Name = "Day")]
        public string Day { get; set; }
        [Display(Name = "Classroom")]
        public string Classroom { get; set; }
        [Display(Name = "Hours")]
        public int[] Hours { get; set; }
        [Display(Name = "Class")]
        public string Class { get; set; }
    }
}