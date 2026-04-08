using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class Attendance
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public bool IsPresent { get; set; }
    }
}