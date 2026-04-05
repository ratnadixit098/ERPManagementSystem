using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class FeesReportModel
    {
        public string EnrollmentNo { get; set; }
        public string StudentName { get; set; }
        public string CollegeName { get; set; }
        public string DepartmentName { get; set; }
        public string CourseName { get; set; }
        public int Semester { get; set; }
        public string Section { get; set; }
        public string AcademicYear { get; set; }
        public decimal TotalFees { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string FeeStatus { get; set; }
    }
}