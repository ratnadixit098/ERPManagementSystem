using System;
using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string TeacherCode { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid 10 digit mobile number")]
        public string Phone { get; set; }

        [Required]
        public DateTime JoiningDate { get; set; }
        public int Post_Id { get; set; }
        public string Post_Name { get; set; }
    }
}