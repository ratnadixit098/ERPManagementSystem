using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ERPManagementSystem.Models
{
    public class Fees
    {
        public int FeesId { get; set; }
        public int StudentId { get; set; }
        public int Semester { get; set; }
        public decimal TotalFees { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}