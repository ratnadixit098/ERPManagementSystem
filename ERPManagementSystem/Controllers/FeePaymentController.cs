using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using ERPManagementSystem.Models;
using Razorpay.Api;
using System.Collections.Generic;

namespace ERPManagementSystem.Controllers
{
    [CheckSession]
    public class FeePaymentController : Controller
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

        // ------------------ CASCADING FILTERS ------------------

        public JsonResult GetDepartment(int collegeId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "select DepartmentId,DepartmentName from Department", conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(dt.AsEnumerable().Select(r => new
            {
                DepartmentId = r["DepartmentId"],
                DepartmentName = r["DepartmentName"]
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourse(int departmentId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "select CourseId,CourseName from Course where DepartmentId=" + departmentId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(dt.AsEnumerable().Select(r => new
            {
                CourseId = r["CourseId"],
                CourseName = r["CourseName"]
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClass(int courseId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "select ClassId,Section from Class where CourseId=" + courseId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(dt.AsEnumerable().Select(r => new
            {
                ClassId = r["ClassId"],
                Section = r["Section"]
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudent(int classId)
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "select StudentId,StudentName=FirstName+' '+LastName from Student where ClassId=" + classId, conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(dt.AsEnumerable().Select(r => new
            {
                StudentId = r["StudentId"],
                StudentName = r["StudentName"]
            }), JsonRequestBehavior.AllowGet);
        }

        // ------------------ FEE DETAILS ------------------

        public JsonResult GetFeeDetails(int studentId)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"
            select 
            fc.FeeType,
            fc.TotalFees,
            PaidAmount = isnull(sum(fp.PayAmount),0),
            Remaining = fc.TotalFees - isnull(sum(fp.PayAmount),0)
            from FeeCommitment fc
            left join FeePayment fp on fc.StudentId = fp.StudentId
            where fc.StudentId=" + studentId + @"
            group by fc.FeeType,fc.TotalFees", conStr);

            DataTable dt = new DataTable();
            da.Fill(dt);

            var data = dt.AsEnumerable().Select(row => new
            {
                FeeType = row["FeeType"],
                TotalFee = row["TotalFees"],
                PaidAmount = row["PaidAmount"],
                Remaining = row["Remaining"]
            }).FirstOrDefault();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ------------------ RAZORPAY ORDER ------------------

        public JsonResult CreateOrder(decimal amount)
        {
            RazorpayClient client = new RazorpayClient(
                "rzp_test_SZrS5COEYmgiPS",
                "SzCNEqxtCJ28CoIfjNaaEwRs");

            Dictionary<string, object> options = new Dictionary<string, object>();

            options.Add("amount", Convert.ToInt32(amount * 100));
            options.Add("currency", "INR");
            options.Add("receipt", "receipt_" + DateTime.Now.Ticks);
            options.Add("payment_capture", 1);

            Order order = client.Order.Create(options);

            return Json(order["id"].ToString(), JsonRequestBehavior.AllowGet);
        }

        // ------------------ SAVE PAYMENT ------------------

        [HttpPost]
        public ActionResult SavePayment(int studentId, decimal payAmount, string paymentId)
        {
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand(
            "insert into FeePayment(StudentId,PayAmount,PaymentDate,GatewayPaymentId) values(@StudentId,@PayAmount,GETDATE(),@paymentId)", con);

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@PayAmount", payAmount);
            cmd.Parameters.AddWithValue("@paymentId", paymentId);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return Json("success");
        }
    }
}