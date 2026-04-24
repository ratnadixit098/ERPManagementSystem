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
    public class CommitmentController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public ActionResult Create()
        {
            BindCollege();
            return View();
        }

        void BindCollege()
        {
            SqlDataAdapter da = new SqlDataAdapter("select CollegeId,CollegeName from College", conStr);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ViewBag.CollegeList = new SelectList(dt.AsDataView(), "CollegeId", "CollegeName");
        }

        [HttpPost]
        public ActionResult Create(FeesCommitment model)
        {
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand(
            "insert into FeeCommitment(StudentId,Semester,TotalFees,FeeType) " +
            "output inserted.CommitmentId " +
            "values(@StudentId,@Semester,@TotalFees,@FeeType)", con);

            cmd.Parameters.AddWithValue("@StudentId", model.StudentId);
            cmd.Parameters.AddWithValue("@Semester", model.Semester == null ? "" : model.Semester);
            cmd.Parameters.AddWithValue("@TotalFees", model.TotalFees);
            cmd.Parameters.AddWithValue("@FeeType", model.FeeType);

            con.Open();
            int commitmentId = (int)cmd.ExecuteScalar();
            con.Close();

            // installment save
            if (model.FeeType == "Installment" && model.InstallmentDate != null)
            {
                for (int i = 0; i < model.InstallmentDate.Count; i++)
                {
                    SqlCommand instCmd = new SqlCommand(
                    "insert into FeeInstallment(CommitmentId,DueDate,Amount) " +
                    "values(@CommitmentId,@DueDate,@Amount)", con);

                    instCmd.Parameters.AddWithValue("@CommitmentId", commitmentId);
                    instCmd.Parameters.AddWithValue("@DueDate", model.InstallmentDate[i]);
                    instCmd.Parameters.AddWithValue("@Amount", model.InstallmentAmount[i]);

                    con.Open();
                    instCmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            TempData["Success"] = "Save Successfully";   // 👈 added
            return RedirectToAction("Create");
        }

        // ---------- Cascading Dropdown ----------

        public JsonResult GetDepartment(int collegeId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "select DepartmentId,DepartmentName from Department where CollegeId=" + collegeId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = dt.AsEnumerable().Select(row => new
            {
                DepartmentId = row["DepartmentId"],
                DepartmentName = row["DepartmentName"]
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourse(int departmentId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "select CourseId,CourseName from Course where DepartmentId=" + departmentId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = dt.AsEnumerable().Select(row => new
            {
                CourseId = row["CourseId"],
                CourseName = row["CourseName"]
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClass(int courseId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "select ClassId,Section from Class where CourseId=" + courseId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = dt.AsEnumerable().Select(row => new
            {
                ClassId = row["ClassId"],
                Section = row["Section"]
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudent(int classId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "select StudentId,StudentName=FirstName+' '+LastName from Student where ClassId=" + classId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = dt.AsEnumerable().Select(row => new
            {
                StudentId = row["StudentId"],
                StudentName = row["StudentName"]
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}