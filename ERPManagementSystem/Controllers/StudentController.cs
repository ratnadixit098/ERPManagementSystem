using ERPManagementSystem.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

[CheckSession]
public class StudentController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
    [CheckSession]
    public ActionResult Create()
    {
        BindCourse();
        BindClass();
        BindPost();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Student model)
    {
        BindCourse();
        BindClass();
        BindPost();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();

            SqlTransaction tran = con.BeginTransaction();

            try
            {
                // 🔥 Barcode Generate
                model.Barcode = "STD" + DateTime.Now.Ticks.ToString().Substring(10);

                SqlCommand cmd = new SqlCommand("ERP_SaveStudent", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;

                // ✅ Student Fields
                cmd.Parameters.AddWithValue("@EnrollmentNo", model.EnrollmentNo);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@ClassId", model.ClassId);
                cmd.Parameters.AddWithValue("@CourseId", model.CourseId);
                cmd.Parameters.AddWithValue("@Barcode", model.Barcode);
                cmd.Parameters.AddWithValue("@AdmissionDate", model.AdmissionDate);
                cmd.Parameters.AddWithValue("@Mobile", model.Mobile);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@FatherName", model.FatherName);
                cmd.Parameters.AddWithValue("@Address", model.Address);
                cmd.Parameters.AddWithValue("@PostId", model.PostId);

                // 🔥 NEW FIELDS (IMPORTANT)
                cmd.Parameters.AddWithValue("@YearOfStudent", model.YearOfStudent);
                cmd.Parameters.AddWithValue("@Semester", model.Semester);

                cmd.ExecuteNonQuery();

                tran.Commit(); // ✅ success
            }
            catch (Exception ex)
            {
                tran.Rollback(); // ❌ error
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        TempData["Success"] = "Student Saved Successfully";
        return RedirectToAction("Create");
    }

    // ------------------ DROPDOWN BIND METHODS ------------------

    public void BindCourse()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlDataAdapter da = new SqlDataAdapter("select CourseId,CourseName from Course", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["CourseId"].ToString(),
                    Text = row["CourseName"].ToString()
                });
            }

            ViewBag.CourseList = list;
        }
    }

    public void BindClass()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlDataAdapter da = new SqlDataAdapter("select ClassId,Section from Class", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["ClassId"].ToString(),
                    Text = "Section " + row["Section"].ToString()
                });
            }

            ViewBag.ClassList = list;
        }
    }

    public void BindPost()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from Post where isActive=1 and Post_Id=3", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Post_Id"].ToString(),
                    Text = row["Post_Name"].ToString()
                });
            }

            ViewBag.PostList = list;
        }
    }
}