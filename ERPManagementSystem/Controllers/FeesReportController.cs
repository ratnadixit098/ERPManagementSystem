using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERPManagementSystem.Models;
 

public class FeesReportController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create(int? CollegeId, int? DepartmentId, int? CourseId, int? Semester, string AcademicYear)
    {
        BindCollege();
        BindDepartment();
        BindCourse();
        BindSemester();
        BindAcademicYear();

        List<FeesReportModel> list = new List<FeesReportModel>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("sp_FeesReport_Filter", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CollegeId", (object)CollegeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartmentId", (object)DepartmentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CourseId", (object)CourseId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Semester", (object)Semester ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AcademicYear", (object)AcademicYear ?? DBNull.Value);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new FeesReportModel
                {
                    EnrollmentNo = dr["EnrollmentNo"].ToString(),
                    StudentName = dr["StudentName"].ToString(),
                    CollegeName = dr["CollegeName"].ToString(),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    CourseName = dr["CourseName"].ToString(),
                    Semester = Convert.ToInt32(dr["Semester"]),
                    Section = dr["Section"].ToString(),
                    AcademicYear = dr["AcademicYear"].ToString(),
                    TotalFees = Convert.ToDecimal(dr["TotalFees"]),
                    PaidAmount = Convert.ToDecimal(dr["PaidAmount"]),
                    PendingAmount = Convert.ToDecimal(dr["PendingAmount"]),
                    DueDate = Convert.ToDateTime(dr["DueDate"]),
                    FeeStatus = dr["FeeStatus"].ToString()
                });
            }
        }

        return View(list);
    }
    void BindCollege()
    {
        SqlDataAdapter da = new SqlDataAdapter("select CollegeId,CollegeName from College", conStr);
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

    void BindDepartment()
    {
        SqlDataAdapter da = new SqlDataAdapter("select DepartmentId,DepartmentName from Department", conStr);
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

    void BindCourse()
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

    void BindSemester()
    {
        ViewBag.SemesterList = new SelectList(new List<SelectListItem>
        {
            new SelectListItem{Text="1st Year",Value="1"},
            new SelectListItem{Text="2nd Year",Value="3"},
            new SelectListItem{Text="3rd Year",Value="5"},
            new SelectListItem{Text="4th Year",Value="7"}
        }, "Value", "Text");
    }

    void BindAcademicYear()
    {
        SqlDataAdapter da = new SqlDataAdapter("select distinct AcademicYear from Class", conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);
      
        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["AcademicYear"].ToString(),
                Text = row["AcademicYear"].ToString()
            });
        }

        ViewBag.YearList = list;
    }
}