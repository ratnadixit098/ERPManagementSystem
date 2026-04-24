using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    [CheckSession]
    public class PrincipalDashboardController : Controller
    {
        // Connection string ko ek baar define karein
        private readonly string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public ActionResult PrincipalDashboard()
        {
            PrincipalDashboard model = new PrincipalDashboard();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // Dashboard Stats
                model.TotalDepartments = (int)new SqlCommand("SELECT COUNT(*) FROM Department", con).ExecuteScalar();
                model.TotalCourses = (int)new SqlCommand("SELECT COUNT(*) FROM Course", con).ExecuteScalar();
                model.TotalTeachers = (int)new SqlCommand("SELECT COUNT(*) FROM Teacher", con).ExecuteScalar();
                model.TotalStudents = (int)new SqlCommand("SELECT COUNT(*) FROM Student", con).ExecuteScalar();

                // Graph Data
                List<string> deptNames = new List<string>();
                List<int> deptStudents = new List<int>();

                string chartQuery = @"
                    SELECT d.DepartmentName, COUNT(s.StudentId) AS TotalStudents
                    FROM Department d
                    LEFT JOIN Course c ON c.DepartmentId = d.DepartmentId
                    LEFT JOIN Student s ON s.CourseId = c.CourseId
                    GROUP BY d.DepartmentName";

                using (SqlCommand cmd = new SqlCommand(chartQuery, con))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            deptNames.Add(dr["DepartmentName"].ToString());
                            deptStudents.Add(Convert.ToInt32(dr["TotalStudents"]));
                        }
                    }
                }

                ViewBag.DeptNames = deptNames;
                ViewBag.DeptStudents = deptStudents;

                // Reports (Tables)
                ViewBag.DepartmentList = GetTable("SELECT * FROM Department", con);
                ViewBag.CourseList = GetTable("SELECT * FROM Course", con);
                ViewBag.TeacherList = GetTable("SELECT * FROM Teacher", con);
                ViewBag.StudentList = GetTable("SELECT * FROM Student", con);
            }

            return View(model);
        }

        // Helper Method: Connection reuse karne ke liye
        private DataTable GetTable(string query, SqlConnection con)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                da.Fill(dt);
            }
            return dt;
        }

        // Helper Method: Jab sirf query pass karni ho
        private DataTable GetDataTable(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public JsonResult GetDepartments()
        {
            DataTable dt = GetDataTable("SELECT DepartmentId, DepartmentName FROM Department");
            var list = dt.AsEnumerable().Select(r => new { id = r["DepartmentId"], name = r["DepartmentName"] });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourses(int? deptId)
        {
            string query = "SELECT CourseId, CourseName FROM Course";
            List<SqlParameter> prm = new List<SqlParameter>();

            if (deptId.HasValue)
            {
                query += " WHERE DepartmentId = @deptId";
                prm.Add(new SqlParameter("@deptId", deptId));
            }

            DataTable dt = GetDataTable(query, prm.ToArray());
            var list = dt.AsEnumerable().Select(r => new { id = r["CourseId"], name = r["CourseName"] });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTeachers(int? deptId)
        {
            string query = "SELECT TeacherId, FirstName + ' ' + LastName AS Name FROM Teacher WHERE 1=1";
            List<SqlParameter> prm = new List<SqlParameter>();

            if (deptId.HasValue)
            {
                query += " AND DepartmentId = @deptId";
                prm.Add(new SqlParameter("@deptId", deptId));
            }

            DataTable dt = GetDataTable(query, prm.ToArray());
            var list = dt.AsEnumerable().Select((r, index) => new { srNo = index + 1, name = r["Name"] });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudents(int? deptId, int? courseId)
        {
            string query = "SELECT StudentId, FirstName + ' ' + LastName AS Name FROM Student S inner join Course C on S.CourseId=C.CourseId WHERE 1=1";
            List<SqlParameter> prm = new List<SqlParameter>();

            if (deptId.HasValue)
            {
                query += " AND DepartmentId = @deptId";
                prm.Add(new SqlParameter("@deptId", deptId));
            }
            if (courseId.HasValue)
            {
                query += " AND S.CourseId = @courseId";
                prm.Add(new SqlParameter("@courseId", courseId));
            }

            DataTable dt = GetDataTable(query, prm.ToArray());
            var list = dt.AsEnumerable().Select((r, index) => new { srNo = index + 1, name = r["Name"] });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}