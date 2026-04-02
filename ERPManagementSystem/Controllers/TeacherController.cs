using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;

public class TeacherController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindDepartment();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Teacher model)
    {
        BindDepartment();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into Teacher(DepartmentId,TeacherCode,FirstName,LastName,Email,Phone,JoiningDate) values(@Dept,@Code,@First,@Last,@Email,@Phone,@Join)", con);

            cmd.Parameters.AddWithValue("@Dept", model.DepartmentId);
            cmd.Parameters.AddWithValue("@Code", model.TeacherCode);
            cmd.Parameters.AddWithValue("@First", model.FirstName);
            cmd.Parameters.AddWithValue("@Last", model.LastName);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);
            cmd.Parameters.AddWithValue("@Join", model.JoiningDate);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Teacher Saved Successfully";
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