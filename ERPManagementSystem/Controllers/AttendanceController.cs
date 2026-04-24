using ERPManagementSystem.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System;
using System.Xml;

public class AttendanceController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;


    // Timetable
    [CheckSession]
    public ActionResult TimeTable()
    {
        List<Timetable> list = new List<Timetable>();
        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand(@"
        select t.*, c.Semester, c.Section,
        s.SubjectName, co.CourseName, d.DepartmentName,teacher_Name=FirstName+' '+LastName,
        case when exists(
            select 1 from Attendance a
            where a.ClassId = t.ClassId
            and a.SubjectId = t.SubjectId
            and cast(a.AttendanceDate as date)=cast(getdate() as date)
        ) then 1 else 0 end as IsAttendanceDone
        from TimeTable t
        inner join Class c on c.ClassId=t.ClassId
        inner join Subject s on s.SubjectId=t.SubjectId
        inner join Course co on co.CourseId=c.CourseId
        inner join Department d on d.DepartmentId=co.DepartmentId
		inner join teacher on teacher.TeacherId= t.TeacherId and t.TeacherId="+ Convert.ToInt32(Session["TeacherId"]) + " ", con);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Timetable
                {
                    TimetableId = Convert.ToInt32(dr["TimetableId"]),
                    ClassId = Convert.ToInt32(dr["ClassId"]),
                    SubjectId = Convert.ToInt32(dr["SubjectId"]),
                    SubjectName = dr["SubjectName"].ToString(),
                    Semester = dr["Semester"].ToString(),
                    Section = dr["Section"].ToString(),
                    CourseName = dr["CourseName"].ToString(),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    DayOfWeek = dr["DayOfWeek"].ToString(),
                    StartTime = (TimeSpan)dr["StartTime"],
                    EndTime = (TimeSpan)dr["EndTime"],
                    IsAttendanceDone = Convert.ToBoolean(dr["IsAttendanceDone"]),
                    TeacherName = dr["teacher_Name"].ToString()
                }) ;
            }
        }

        return View(list);
    }
    // Students load
    public JsonResult GetStudents(int classId)
    {
        SqlConnection con = new SqlConnection(conStr);

        SqlDataAdapter da = new SqlDataAdapter(
        "select StudentId, FirstName+' '+LastName as StudentName, EnrollmentNo from Student where ClassId=" + classId, con);

        DataTable dt = new DataTable();
        da.Fill(dt);

        var list = new List<object>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new
            {
                StudentId = row["StudentId"],
                StudentName = row["StudentName"],
                EnrollmentNo = row["EnrollmentNo"]
            });
        }

        return Json(list, JsonRequestBehavior.AllowGet);
    }

    // Save Attendance
    [HttpPost]
    public JsonResult SaveAttendance(List<Attendance> data)
    {
        SqlConnection con = new SqlConnection(conStr);

        con.Open();

        foreach (var item in data)
        {
            SqlCommand cmd = new SqlCommand(
            "insert into Attendance(StudentId,ClassId,SubjectId,AttendanceDate,IsPresent,TeacherId) values(@StudentId,@ClassId,@SubjectId,@Date,@IsPresent,@TeacherId)", con);

            cmd.Parameters.AddWithValue("@StudentId", item.StudentId);
            cmd.Parameters.AddWithValue("@ClassId", item.ClassId);
            cmd.Parameters.AddWithValue("@SubjectId", item.SubjectId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
            cmd.Parameters.AddWithValue("@IsPresent", item.IsPresent);
            cmd.Parameters.AddWithValue("@TeacherId", Convert.ToInt32(Session["TeacherId"]));

            cmd.ExecuteNonQuery();
        }

        con.Close();

        return Json("Saved");
    }
}