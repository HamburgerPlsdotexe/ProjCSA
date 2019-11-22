using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class ScheduleModel
    {
        public string LessonCode { get; set; }
        public string Day { get; set; }
        public string Classroom { get; set; }
        public List<int> Hours { get; set; }
        public string Class { get; set; }
        
    }
}