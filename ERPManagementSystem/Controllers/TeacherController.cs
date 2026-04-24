using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;
using System;

[CheckSession]
public class TeacherController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
    [CheckSession]
    public ActionResult Create()
    {
        BindDepartment();
        BindPost();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Teacher model)
    //{
    //    BindDepartment();
    //    BindPost();
    //    using (SqlConnection con = new SqlConnection(conStr))
    //    {
    //        SqlCommand cmd = new SqlCommand("insert into Teacher(DepartmentId,TeacherCode,FirstName,LastName,Email,Phone,JoiningDate,PostId) values(@Dept,@Code,@First,@Last,@Email,@Phone,@Join,@Post)", con);

    //        cmd.Parameters.AddWithValue("@Dept", model.DepartmentId);
    //        cmd.Parameters.AddWithValue("@Code", model.TeacherCode);
    //        cmd.Parameters.AddWithValue("@First", model.FirstName);
    //        cmd.Parameters.AddWithValue("@Last", model.LastName);
    //        cmd.Parameters.AddWithValue("@Email", model.Email);
    //        cmd.Parameters.AddWithValue("@Phone", model.Phone);
    //        cmd.Parameters.AddWithValue("@Join", model.JoiningDate);
    //        cmd.Parameters.AddWithValue("@Post", model.Post_Id);

    //        con.Open();
    //        cmd.ExecuteNonQuery();
    //    }

    //    TempData["Success"] = "Teacher Saved Successfully";
    //    return RedirectToAction("Create");
    //}
    {
        BindDepartment();
        BindPost();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();

            SqlTransaction tran = con.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("ERP_SaveTeacher", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                cmd.Parameters.AddWithValue("@TeacherCode", model.TeacherCode);
                cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                cmd.Parameters.AddWithValue("@LastName", model.LastName);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Phone", model.Phone);
                cmd.Parameters.AddWithValue("@JoiningDate", model.JoiningDate);
                cmd.Parameters.AddWithValue("@PostId", model.Post_Id);
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
    public void BindPost()
    {
        SqlConnection con = new SqlConnection(conStr);
        SqlDataAdapter da = new SqlDataAdapter("select * from Post where isActive=1", con);
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