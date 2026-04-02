using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class FeesStructureController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindCourse();
        return View();
    }

    [HttpPost]
    public ActionResult Create(FeesStructure model)
    {
        BindCourse();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into FeesStructure(CourseId,Semester,TotalFees) values(@Course,@Semester,@Fees)", con);

            cmd.Parameters.AddWithValue("@Course", model.CourseId);
            cmd.Parameters.AddWithValue("@Semester", model.Semester);
            cmd.Parameters.AddWithValue("@Fees", model.TotalFees);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Fees Structure Saved Successfully";
        return RedirectToAction("Create");
    }

    public void BindCourse()
    {
        SqlDataAdapter da = new SqlDataAdapter("select CourseId,CourseName from Course", conStr);
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