using System;
using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class FeesStructure
    {
        public int FeesStructureId { get; set; }
        public int CourseId { get; set; }
        public int Semester { get; set; }
        public decimal TotalFees { get; set; }
    }
}