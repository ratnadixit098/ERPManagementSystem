using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class ClassController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindCourse();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Class model)
    {
        BindCourse();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into Class(CourseId,Semester,Section,AcademicYear) values(@Course,@Sem,@Sec,@Year)", con);

            cmd.Parameters.AddWithValue("@Course", model.CourseId);
            cmd.Parameters.AddWithValue("@Sem", model.Semester);
            cmd.Parameters.AddWithValue("@Sec", model.Section);
            cmd.Parameters.AddWithValue("@Year", model.AcademicYear);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Class Saved Successfully";
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