using System;

namespace ERPManagementSystem.Models
{
    public class FeePayment
    {
        public int StudentId { get; set; }
        public string FeeType { get; set; }
        public decimal TotalFee { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal PayAmount { get; set; }
    }
}