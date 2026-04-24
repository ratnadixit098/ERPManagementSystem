using ERPManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;

namespace ERPManagementSystem.Controllers
{
    [CheckSession]
    public class StudentAcademicController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Constring"].ConnectionString);

        public ActionResult EditStudentAcademic(int? CourseId, int? DepartmentId, int? YearOfStudent, int? Semester)
        {
            List<StudentAcademic> list = new List<StudentAcademic>();

            if (CourseId != null || DepartmentId != null || YearOfStudent != null || Semester != null)
            {
                SqlCommand cmd = new SqlCommand("sp_GetStudentAcademicList", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CourseId", (object)CourseId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object)DepartmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@YearOfStudent", (object)YearOfStudent ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", (object)Semester ?? DBNull.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new StudentAcademic
                    {
                        StudentId = Convert.ToInt32(dr["StudentId"]),
                        StudentName = dr["StudentName"].ToString(),
                        EnrollmentNo = dr["EnrollmentNo"].ToString(),
                        CourseName = dr["CourseName"].ToString(),
                        DepartmentName = dr["DepartmentName"].ToString(),
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        Duration = Convert.ToInt32(dr["Duration"]),
                        AdmissionYear = Convert.ToInt32(dr["AdmissionYear"]),
                        YearOfStudent = Convert.ToInt32(dr["YearOfStudent"]),
                        Semester = Convert.ToInt32(dr["Semester"])
                    });
                }
            }

            SqlDataAdapter daCourse = new SqlDataAdapter("select CourseId,CourseName,DurationYears from Course", con);
            DataTable dtCourse = new DataTable();
            daCourse.Fill(dtCourse);
            ViewBag.Course = dtCourse;

            SqlDataAdapter daDept = new SqlDataAdapter("select DepartmentId,DepartmentName from Department", con);
            DataTable dtDept = new DataTable();
            daDept.Fill(dtDept);
            ViewBag.Department = dtDept;

            return View(list);
        }

        [HttpPost]
        public ActionResult Save(StudentAcademic model)
        {
            SqlCommand cmd = new SqlCommand("sp_InsertStudentAcademic", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StudentId", model.StudentId);
            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
            cmd.Parameters.AddWithValue("@YearOfStudent", model.YearOfStudent);
            cmd.Parameters.AddWithValue("@Semester", model.Semester);
            cmd.Parameters.AddWithValue("@AcademicYear", model.AcademicYear);
            cmd.Parameters.AddWithValue("@BatchYear", model.BatchYear);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            TempData["Success"] = "Saved Successfully";

            return RedirectToAction("EditStudentAcademic");
        }
    }
}