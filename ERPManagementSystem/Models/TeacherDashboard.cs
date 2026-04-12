using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class TeacherDashboard
    {
        public string DayOfWeek { get; set; }
        public string Slot { get; set; }
        public string SubjectName { get; set; }
        public string ClassName { get; set; }
        public bool IsAttendanceTaken { get; set; }
    }
}