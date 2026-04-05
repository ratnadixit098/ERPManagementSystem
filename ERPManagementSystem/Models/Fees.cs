using System;
using System.Collections.Generic;

namespace ERPManagementSystem.Models
{
    public class Fees
    {
        public int FeeId { get; set; }
        public int StudentId { get; set; }
        public decimal TotalFees { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public string FeeType { get; set; }
        public string Semester { get; set; }
        public DateTime? PaymentDate { get; set; }

        public List<FeeInstallment> Installments { get; set; }
    }
    public class FeeInstallment
    {
        public int InstallmentId { get; set; }
        public int FeeId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}