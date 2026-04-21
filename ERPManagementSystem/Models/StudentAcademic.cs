using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class StudentAcademic
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DepartmentId { get; set; }
        public int YearOfStudent { get; set; }
        public int Semester { get; set; }
        public string AcademicYear { get; set; }
        public string BatchYear { get; set; }

        public string StudentName { get; set; }
        public string EnrollmentNo { get; set; }
        public string CourseName { get; set; }
        public string DepartmentName { get; set; }

        public int Duration { get; set; }
        public int AdmissionYear { get; set; }
    }
}