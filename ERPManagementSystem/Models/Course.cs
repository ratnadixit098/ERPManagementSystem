using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string CourseCode { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public int DurationYears { get; set; }

        [Required]
        public int TotalSemester { get; set; }
    }
}