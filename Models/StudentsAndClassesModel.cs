using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class StudentsAndClassesModel
    {
        public List<StudentModel> Students { get; set; }
        public List<ClassModel> Classes { get; set; }
    }
}