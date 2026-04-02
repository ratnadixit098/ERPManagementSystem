using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class CourseController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindDepartment();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Course model)
    {
        BindDepartment();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into Course(DepartmentId,CourseCode,CourseName,DurationYears,TotalSemester) values(@Dept,@Code,@Name,@Duration,@Semester)", con);

            cmd.Parameters.AddWithValue("@Dept", model.DepartmentId);
            cmd.Parameters.AddWithValue("@Code", model.CourseCode);
            cmd.Parameters.AddWithValue("@Name", model.CourseName);
            cmd.Parameters.AddWithValue("@Duration", model.DurationYears);
            cmd.Parameters.AddWithValue("@Semester", model.TotalSemester);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Course Saved Successfully";
        return RedirectToAction("Create");
    }

    public void BindDepartment()
    {
        SqlConnection con = new SqlConnection(conStr);
        SqlDataAdapter da = new SqlDataAdapter("select DepartmentId,DepartmentName from Department", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["DepartmentId"].ToString(),
                Text = row["DepartmentName"].ToString()
            });
        }

        ViewBag.DepartmentList = list;
    }
}