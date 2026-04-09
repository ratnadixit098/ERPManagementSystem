using System;
using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Timetable
    {
        public int TimetableId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public string DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string Day { get; set; }
        public string Time { get; set; }
        public string SubjectName { get; set; }
        public string Section { get; set; }
        public string Semester { get; set; }
        public string DepartmentName { get; set; }
        public string Teacher { get; set; }
        public bool IsAttendanceDone { get; set; }
        public string CourseName { get; set; }

        // FIXED
        public int? IsPresent { get; set; }
        public string Slot { get; set; }
    }
}