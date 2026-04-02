using System.ComponentModel.DataAnnotations;

namespace ERPManagementSystem.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentCode { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        [Required]
        public int CollegeId { get; set; }

        [Required]
        public string Description { get; set; }
    }
}