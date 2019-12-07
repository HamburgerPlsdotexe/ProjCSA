using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class StudentsClassesLessonCode
    {
        public List<StudentModel> Students { get; set; }
        public List<ClassModel> Classes { get; set; }
        public string ClassCode { get; set; }
    }
}