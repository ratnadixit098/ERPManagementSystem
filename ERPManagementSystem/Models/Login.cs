using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPManagementSystem.Models
{
    public class Login
    {
        public int LoginId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }  // Admin / Teacher / Student

        public int? TeacherId { get; set; }

        public int? StudentId { get; set; }

        public bool IsActive { get; set; }
    }
}