using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class PrincipalDashboard
    {
        public int TotalDepartments { get; set; }
        public int TotalCourses { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
    }
}