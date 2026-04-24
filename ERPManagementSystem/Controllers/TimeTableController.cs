using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;
using System;

public class TimetableController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindClass();
        BindSubject();
        BindTeacher();
        BindDay();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Timetable model)
    {
        BindClass();
        BindSubject();
        BindTeacher();
        BindDay();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            var start = DateTime.Today.Add(model.StartTime).ToString("hh:mm tt");
            var end = DateTime.Today.Add(model.EndTime).ToString("hh:mm tt");

            
            SqlCommand cmd = new SqlCommand("insert into Timetable(ClassId,SubjectId,TeacherId,DayOfWeek,StartTime,EndTime) values(@Class,@Subject,@Teacher,@Day,@Start,@End)", con);

            cmd.Parameters.AddWithValue("@Class", model.ClassId);
            cmd.Parameters.AddWithValue("@Subject", model.SubjectId);
            cmd.Parameters.AddWithValue("@Teacher", model.TeacherId);
            cmd.Parameters.AddWithValue("@Day", model.DayOfWeek);
            //cmd.Parameters.AddWithValue("@Start", model.StartTime.ToString("hh:mm tt"));
            //cmd.Parameters.AddWithValue("@End", model.EndTime.ToString("hh:mm tt"));
            cmd.Parameters.AddWithValue("@Start", start);
            cmd.Parameters.AddWithValue("@End", end);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Timetable Saved Successfully";
        return RedirectToAction("Create");
    }

    public void BindClass()
    {
        SqlDataAdapter da = new SqlDataAdapter("select ClassId,class_Name= CourseName+' '+DepartmentName+' Sem- ' +convert(varchar,Semester)+' '+Section from Class\r\nleft join Course on Course.CourseId=Class.CourseId\r\nleft join Department on Department.DepartmentId=Course.DepartmentId", conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);

        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["ClassId"].ToString(),
                Text = row["class_Name"].ToString()
            });
        }

        ViewBag.ClassList = list;
    
    }

    public void BindSubject()
    {
        SqlDataAdapter da = new SqlDataAdapter("select SubjectId,SubjectName from Subject", conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);
        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["SubjectId"].ToString(),
                Text = row["SubjectName"].ToString()
            });
        }

        ViewBag.SubjectList = list;
       
    }

    public void BindTeacher()
    {
        SqlDataAdapter da = new SqlDataAdapter("select TeacherId,FirstName from Teacher", conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);
        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["TeacherId"].ToString(),
                Text = row["FirstName"].ToString()
            });
        }

        ViewBag.TeacherList = list;
       
    }

    public void BindDay()
    {
        List<SelectListItem> dayList = new List<SelectListItem>()
        {
            new SelectListItem{ Text="Monday", Value="Monday"},
            new SelectListItem{ Text="Tuesday", Value="Tuesday"},
            new SelectListItem{ Text="Wednesday", Value="Wednesday"},
            new SelectListItem{ Text="Thursday", Value="Thursday"},
            new SelectListItem{ Text="Friday", Value="Friday"},
            new SelectListItem{ Text="Saturday", Value="Saturday"}
        };

        ViewBag.DayList = dayList;
    }

    [HttpPost]
    public JsonResult CheckTeacherConflict(int TeacherId, string DayOfWeek, string StartTime, string EndTime)
    {
        bool isConflict = false;
        string message = "";
        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("CheckTeacherConflict", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TeacherId", TeacherId);
            cmd.Parameters.AddWithValue("@DayOfWeek", DayOfWeek);
            cmd.Parameters.AddWithValue("@StartTime", StartTime);
            cmd.Parameters.AddWithValue("@EndTime", EndTime);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                //message = $"{dr["FirstName"]} is already assigned to teach {dr["SubjectName"]} on {dr["DayOfWeek"]} from {dr["StartTime"]} to {dr["EndTime"]}.";

                string type = dr["ConflictType"].ToString();

                if (type == "TEACHER")
                {
                    message = $"{dr["Name"]} is already assigned to {dr["SubjectName"]} on {dr["DayOfWeek"]} from {dr["StartTime"]} to {dr["EndTime"]}.";
                }
                else if (type == "CLASS")
                {
                    message = $"This class already has {dr["Name"]} assigned for {dr["SubjectName"]} on {dr["DayOfWeek"]} from {dr["StartTime"]} to {dr["EndTime"]}.";
                }
            }
        }

        return Json(message);
    }
}