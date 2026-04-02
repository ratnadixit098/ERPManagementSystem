using ERPManagementSystem.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class StudentController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindCourse();
        BindClass();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Student model)
    {
        BindCourse();
        BindClass();

        // Barcode Generate
        model.Barcode = "STD" + DateTime.Now.Ticks.ToString().Substring(10);

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand(
 "insert into Student(EnrollmentNo,FirstName,LastName,ClassId,CourseId,Mobile,Email,FatherName,Address,Barcode,AdmissionDate) " +
 "values(@Enroll,@First,@Last,@Class,@Course,@Mobile,@Email,@Father,@Address,@Barcode,@Date)", con);

            cmd.Parameters.AddWithValue("@Enroll", model.EnrollmentNo);
            cmd.Parameters.AddWithValue("@First", model.FirstName);
            cmd.Parameters.AddWithValue("@Last", model.LastName);
            cmd.Parameters.AddWithValue("@Class", model.ClassId);
            cmd.Parameters.AddWithValue("@Course", model.CourseId);
            cmd.Parameters.AddWithValue("@Mobile", model.Mobile);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Father", model.FatherName);
            cmd.Parameters.AddWithValue("@Address", model.Address);
            cmd.Parameters.AddWithValue("@Barcode", model.Barcode);
            cmd.Parameters.AddWithValue("@Date", model.AdmissionDate);
            con.Open();
            cmd.ExecuteNonQuery();
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
}