using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        public string SubjectCode { get; set; }

        [Required]
        public string SubjectName { get; set; }

        [Required]
        public int Credits { get; set; }
    }
}