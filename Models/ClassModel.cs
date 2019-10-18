using System.ComponentModel.DataAnnotations;

namespace ProjectCSA.Models
{
    public class ClassModel
    {
        [Display(Name = "Class Number")]
        public string Cnum { get; set; }

    }
}