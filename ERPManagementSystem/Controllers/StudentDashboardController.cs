using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public ActionResult StudentDashboard()
        {
            int studentId = 1;

            List<Timetable> list = new List<Timetable>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // timetable + attendance
                SqlCommand cmd = new SqlCommand(@"
                select 
                t.DayOfWeek,
                t.StartTime,
                t.EndTime,
                s.SubjectName,
                te.FirstName+' '+te.LastName as TeacherName,
                a.IsPresent
                from TimeTable t
                left join Subject s on s.SubjectId=t.SubjectId
                left join Teacher te on te.TeacherId=t.TeacherId
                left join Attendance a 
                    on a.SubjectId=t.SubjectId 
                    and a.ClassId=t.ClassId
                    and a.StudentId=@studentId
                    and cast(a.AttendanceDate as date)=cast(getdate() as date)
                ", con);

                cmd.Parameters.AddWithValue("@studentId", studentId);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TimeSpan start = (TimeSpan)dr["StartTime"];
                    TimeSpan end = (TimeSpan)dr["EndTime"];

                    list.Add(new Timetable
                    {
                        DayOfWeek = dr["DayOfWeek"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        Teacher = dr["TeacherName"].ToString(),
                        Slot = start.Hours + "-" + end.Hours,
                        IsPresent = dr["IsPresent"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IsPresent"])
                    });
                }

                dr.Close();

                // total subjects
                SqlCommand cmd2 = new SqlCommand(
                    "select count(distinct SubjectId) from TimeTable", con);
                ViewBag.TotalSubjects = cmd2.ExecuteScalar();

                // total lectures
                SqlCommand cmd3 = new SqlCommand(
                    "select count(*) from TimeTable", con);
                ViewBag.TotalLectures = cmd3.ExecuteScalar();

                // pending fees
                SqlCommand cmd4 = new SqlCommand(
                    "select isnull(sum(PendingAmount),0) from Fees where StudentId=@studentId", con);
                cmd4.Parameters.AddWithValue("@studentId", studentId);
                ViewBag.PendingFees = cmd4.ExecuteScalar();
            }

            return View(list);
        }
    }
}