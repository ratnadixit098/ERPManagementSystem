using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class DepartmentController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
    [CheckSession]
    public ActionResult Create()
    {
        BindCollege();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Department model)
    {
        BindCollege();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into Department(DepartmentCode,DepartmentName,CollegeId,Description) values(@Code,@Name,@College,@Desc)", con);

            cmd.Parameters.AddWithValue("@Code", model.DepartmentCode);
            cmd.Parameters.AddWithValue("@Name", model.DepartmentName);
            cmd.Parameters.AddWithValue("@College", model.CollegeId);
            cmd.Parameters.AddWithValue("@Desc", model.Description);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Department Saved Successfully";
        return RedirectToAction("Create");
    }

    public void BindCollege()
    {
        SqlConnection con = new SqlConnection(conStr);
        SqlDataAdapter da = new SqlDataAdapter("select CollegeId,CollegeName from College", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["CollegeId"].ToString(),
                Text = row["CollegeName"].ToString()
            });
        }

        ViewBag.CollegeList = list;
    }
}