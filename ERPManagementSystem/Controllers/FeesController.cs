using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    public class FeesController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        // GET
        public ActionResult Create()
        {
            BindCollege();
            return View();
        }

        void BindCollege()
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlDataAdapter da = new SqlDataAdapter("select CollegeId,CollegeName from College", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ViewBag.CollegeList = new SelectList(dt.AsDataView(), "CollegeId", "CollegeName");
        }

        [HttpPost]
        public ActionResult Create(Fees model)
        {
            SqlConnection con = new SqlConnection(conStr);

            decimal previousPaid = 0;
            decimal totalFees = model.TotalFees;

            // check existing fee
            SqlCommand checkCmd = new SqlCommand("select isnull(sum(PaidAmount),0) from Fees where StudentId=@StudentId", con);
            checkCmd.Parameters.AddWithValue("@StudentId", model.StudentId);

            con.Open();
            previousPaid = Convert.ToDecimal(checkCmd.ExecuteScalar());
            con.Close();

            decimal newPaidTotal = previousPaid + model.PaidAmount;
            decimal remaining = totalFees - newPaidTotal;

            SqlCommand cmd = new SqlCommand("insert into Fees(StudentId,Semester,TotalFees,PaidAmount,PendingAmount,FeeType,PaymentDate) values(@StudentId,@Semester,@TotalFees,@PaidAmount,@PendingAmount,@FeeType,@PaymentDate)", con);

            cmd.Parameters.AddWithValue("@StudentId", model.StudentId);
            cmd.Parameters.AddWithValue("@Semester", model.Semester);
            cmd.Parameters.AddWithValue("@TotalFees", model.TotalFees);
            cmd.Parameters.AddWithValue("@PaidAmount", model.PaidAmount);
            cmd.Parameters.AddWithValue("@PendingAmount", remaining);
            cmd.Parameters.AddWithValue("@FeeType", model.FeeType);
            cmd.Parameters.AddWithValue("@PaymentDate", model.PaymentDate);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Create");
        }

        // load previous fee
        public JsonResult GetStudentFee(int studentId)
        {
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand("select top 1 TotalFees,Semester,FeeType,(TotalFees - isnull((select sum(PaidAmount) from Fees where StudentId=@StudentId),0)) as Pending from Fees where StudentId=@StudentId", con);

            cmd.Parameters.AddWithValue("@StudentId", studentId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            object data = null;

            if (dr.Read())
            {
                data = new
                {
                    TotalFees = dr["TotalFees"],
                    Semester = dr["Semester"],
                    FeeType = dr["FeeType"],
                    PendingAmount = dr["Pending"]
                };
            }

            con.Close();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartment(int collegeId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select DepartmentId,DepartmentName from Department", conStr);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourse(int departmentId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select CourseId,CourseName from Course where DepartmentId=" + departmentId, conStr);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClass(int courseId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select ClassId,Section from Class where CourseId=" + courseId, conStr);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudent(int classId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select StudentId,StudentName from Student where ClassId=" + classId, conStr);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(dt, JsonRequestBehavior.AllowGet);
        }
    }
}