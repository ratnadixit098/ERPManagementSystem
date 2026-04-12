using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        // GET: Login Page
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public JsonResult Login(string Username, string Password)
        {

            Login user = null;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("ERP_GetLOgin", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Password", Password);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    user = new Login()
                    {
                        LoginId = Convert.ToInt32(dr["UserId"]),
                        Username = dr["Username"].ToString(),
                        Role = dr["Role"].ToString(),
                       StudentId = Convert.ToInt32(dr["StudentId"] == DBNull.Value ? 0 : dr["StudentId"]),
                        TeacherId = Convert.ToInt32(dr["TeacherId"] == DBNull.Value ? 0 : dr["TeacherId"]),

                        Post_Id = Convert.ToInt32(dr["Post_Id"] == DBNull.Value ? 0 : dr["Post_Id"]),
                        Department_Id = Convert.ToInt32(dr["DepartmentId"] == DBNull.Value ? 0 : dr["DepartmentId"]),
                        Class_Id = Convert.ToInt32(dr["ClassId"] == DBNull.Value ? 0 : dr["ClassId"]),
                        Course_Id = Convert.ToInt32(dr["CourseId"] == DBNull.Value ? 0 : dr["CourseId"]),
                        Section = Convert.ToString(dr["Section"] == DBNull.Value ? "" : dr["Section"]),
                        AcademicYear = Convert.ToString(dr["AcademicYear"] == DBNull.Value ? 0 : dr["AcademicYear"]),
                        Semester = Convert.ToString(dr["Semester"] == DBNull.Value ? 0 : dr["Semester"]),
                        DepartmentName = Convert.ToString(dr["DepartmentName"] == DBNull.Value ? 0 : dr["DepartmentName"]),
                        Teacher_Name = Convert.ToString(dr["Teacher_Name"] == DBNull.Value ? 0 : dr["Teacher_Name"]),
                        Stundent_Name = Convert.ToString(dr["Stundent_Name"] == DBNull.Value ? 0 : dr["Stundent_Name"]),
                        CollegeId = Convert.ToInt32(dr["CollegeId"] == DBNull.Value ? 0 : dr["CollegeId"]),
                        CourseName = Convert.ToString(dr["CourseName"] == DBNull.Value ? 0 : dr["CourseName"])

                    };
                }
            }

            // 🔥 Session handling
            if (user != null)
            {
                Session["LoginId"] = user.LoginId;
                Session["Username"] = user.Username;
                Session["Role"] = user.Role;
                Session["StudentId"] = user.StudentId;
                Session["TeacherId"] = user.TeacherId;

                Session["Post_Id"] = user.Post_Id;
                Session["DepartmentId"] = user.Department_Id;
                Session["DepartmentName"] = user.DepartmentName;
                Session["ClassId"] = user.Class_Id;
                Session["CourseId"] = user.Course_Id;
                Session["Section"] = user.Section;
                Session["AcademicYear"] = user.AcademicYear;
                Session["Semester"] = user.Semester;
                Session["Teacher_Name"] = user.Teacher_Name;
                Session["Stundent_Name"] = user.Stundent_Name;
                Session["CollegeId"] = user.CollegeId;
                Session["CourseName"] = user.CourseName;









                if (user.Role=="Admin")
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("create", "Teacher")
                    });
                }
                else if (user.Role=="Teacher")
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("TimeTable", "Attendance")
                    });
                }
                else if (user.Role == "Principal")
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("create", "Student")
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action("StudentDashboard", "StudentDashboard")
                    });
                }
               
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Username or Password"
                });
            }

        }
        public ActionResult Logout()
        {
            // 1. Clear all session variables
            Session.Clear();
            Session.Abandon();

            // 2. Remove the Authentication Cookie
            FormsAuthentication.SignOut();

            // 3. Optional: Clear specific cookies if you have them
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            // 4. Redirect to the Login page (or Home)
            return RedirectToAction("Index", "Home");
        }
    }

}