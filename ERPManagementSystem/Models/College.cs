using System;
using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class College
    {
        public int CollegeId { get; set; }
        public string CollegeCode { get; set; }
        public string CollegeName { get; set; }
        public string Address { get; set; }

        public int StateId { get; set; }
        public int DistrictId { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}