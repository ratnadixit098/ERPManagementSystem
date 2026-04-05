using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class FeesCommitment
    {
        public int StudentId { get; set; }
        public string Semester { get; set; }
        public decimal TotalFees { get; set; }
        public string FeeType { get; set; }

        public List<DateTime> InstallmentDate { get; set; }
        public List<decimal> InstallmentAmount { get; set; }
    }
}