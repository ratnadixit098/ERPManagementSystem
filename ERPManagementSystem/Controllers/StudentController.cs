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

    public ActionResult Create()
    {
        BindCourse();
        BindClass();
        BindPost();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Student model)
    //   {
    //       BindCourse();
    //       BindClass();

    //       // Barcode Generate
    //       model.Barcode = "STD" + DateTime.Now.Ticks.ToString().Substring(10);

    //       using (SqlConnection con = new SqlConnection(conStr))
    //       {
    //           SqlCommand cmd = new SqlCommand(
    //"insert into Student(EnrollmentNo,FirstName,LastName,ClassId,CourseId,Mobile,Email,FatherName,Address,Barcode,AdmissionDate) " +
    //"values(@Enroll,@First,@Last,@Class,@Course,@Mobile,@Email,@Father,@Address,@Barcode,@Date)", con);

    //           cmd.Parameters.AddWithValue("@Enroll", model.EnrollmentNo);
    //           cmd.Parameters.AddWithValue("@First", model.FirstName);
    //           cmd.Parameters.AddWithValue("@Last", model.LastName);
    //           cmd.Parameters.AddWithValue("@Class", model.ClassId);
    //           cmd.Parameters.AddWithValue("@Course", model.CourseId);
    //           cmd.Parameters.AddWithValue("@Mobile", model.Mobile);
    //           cmd.Parameters.AddWithValue("@Email", model.Email);
    //           cmd.Parameters.AddWithValue("@Father", model.FatherName);
    //           cmd.Parameters.AddWithValue("@Address", model.Address);
    //           cmd.Parameters.AddWithValue("@Barcode", model.Barcode);
    //           cmd.Parameters.AddWithValue("@Date", model.AdmissionDate);
    //           con.Open();
    //           cmd.ExecuteNonQuery();
    //       }

    //       TempData["Success"] = "Student Saved Successfully";
    //       return RedirectToAction("Create");
    //   }
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
                // Barcode Generate
                model.Barcode = "STD" + DateTime.Now.Ticks.ToString().Substring(10);
                SqlCommand cmd = new SqlCommand("ERP_SaveStudent", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;

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
                //cmd.Parameters.AddWithValue("@AddedBy", model.Post_Id);

                cmd.ExecuteNonQuery();

                // agar aur bhi queries ho to yahin likho

                tran.Commit(); // ✅ success
            }
            catch (Exception ex)
            {
                tran.Rollback(); // ❌ error
                throw;
            }
        }

        TempData["Success"] = "Student Saved Successfully";
        return RedirectToAction("Create");
    }

    public void BindCourse()
    {
        SqlConnection con = new SqlConnection(conStr);
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

    public void BindClass()
    {
        SqlConnection con = new SqlConnection(conStr);
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

    public void BindPost()
    {
        SqlConnection con = new SqlConnection(conStr);
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