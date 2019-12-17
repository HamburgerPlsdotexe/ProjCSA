using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class JsonAttendance
    {
        public string LessonCode { get; set; }
        public string userClass { get; set; }
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
    }
}