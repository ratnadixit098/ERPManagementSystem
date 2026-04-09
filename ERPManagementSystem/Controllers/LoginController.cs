using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
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
                        Role = dr["Role"].ToString()
                    };
                }
            }

            // 🔥 Session handling
            if (user != null)
            {
                Session["LoginId"] = user.LoginId;
                Session["Username"] = user.Username;
                Session["Role"] = user.Role;

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

    }
}