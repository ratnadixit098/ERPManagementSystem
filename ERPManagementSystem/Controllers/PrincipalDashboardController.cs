using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    public class PrincipalDashboardController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public ActionResult PrincipalDashboard()
        {
            PrincipalDashboard model = new PrincipalDashboard();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                model.TotalDepartments = (int)new SqlCommand("select count(*) from Department", con).ExecuteScalar();
                model.TotalCourses = (int)new SqlCommand("select count(*) from Course", con).ExecuteScalar();
                model.TotalTeachers = (int)new SqlCommand("select count(*) from Teacher", con).ExecuteScalar();
                model.TotalStudents = (int)new SqlCommand("select count(*) from Student", con).ExecuteScalar();

                List<string> deptNames = new List<string>();
                List<int> deptStudents = new List<int>();

                SqlCommand cmd = new SqlCommand(@"
                select d.DepartmentName, count(s.StudentId) as TotalStudents
                from Department d
                left join Course c on c.DepartmentId=d.DepartmentId
                left join Student s on s.CourseId=c.CourseId
                group by d.DepartmentName", con);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int total = Convert.ToInt32(dr["TotalStudents"]);
                    if (total > 0)
                    {
                        deptNames.Add(dr["DepartmentName"].ToString());
                        deptStudents.Add(total);
                    }
                }
                dr.Close();

                ViewBag.DeptNames = deptNames;
                ViewBag.DeptStudents = deptStudents;

                // Reports
                ViewBag.DepartmentList = GetTable("select * from Department", con);
                ViewBag.CourseList = GetTable("select * from Course", con);
                ViewBag.TeacherList = GetTable("select * from Teacher", con);
                ViewBag.StudentList = GetTable("select * from Student", con);
            }

            return View(model);
        }

        private DataTable GetTable(string query, SqlConnection con)
        {
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}