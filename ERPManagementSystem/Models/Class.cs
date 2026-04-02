using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Class
    {
        public int ClassId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        public string Section { get; set; }

        [Required]
        public string AcademicYear { get; set; }
    }
}