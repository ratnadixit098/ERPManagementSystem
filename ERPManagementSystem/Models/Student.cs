using System;
using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        public string EnrollmentNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int ClassId { get; set; }
        public int CourseId { get; set; }

        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FatherName { get; set; }
        public string Address { get; set; }

        public string Barcode { get; set; }
        public int PostId { get; set; }
        public DateTime AdmissionDate { get; set; }
    }
}