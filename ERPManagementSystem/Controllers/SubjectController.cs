using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class SubjectController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
    [CheckSession]
    public ActionResult Create()
    {
        BindCourse();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Subject model)
    {
        BindCourse();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into Subject(CourseId,Semester,SubjectCode,SubjectName,Credits) values(@Course,@Sem,@Code,@Name,@Credits)", con);

            cmd.Parameters.AddWithValue("@Course", model.CourseId);
            cmd.Parameters.AddWithValue("@Sem", model.Semester);
            cmd.Parameters.AddWithValue("@Code", model.SubjectCode);
            cmd.Parameters.AddWithValue("@Name", model.SubjectName);
            cmd.Parameters.AddWithValue("@Credits", model.Credits);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Subject Saved Successfully";
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
}