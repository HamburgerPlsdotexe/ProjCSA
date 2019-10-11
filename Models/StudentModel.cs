using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCSA.Models
{
    public class StudentModel
    {
        public int Snum { get; set; }
        public string Fname { get; set; }
        public string Infix { get; set; }
        public string Lname { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}