using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    [CheckSession]
    public class StudentDashboardController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
        public ActionResult StudentDashboard()
        {
            int studentId = Convert.ToInt32(Session["StudentId"]);

            List<Timetable> list = new List<Timetable>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // timetable + today attendance
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
                    //TimeSpan start = (TimeSpan)dr["StartTime"];
                    //TimeSpan end = (TimeSpan)dr["EndTime"];

                    DateTime start = DateTime.ParseExact(dr["StartTime"].ToString(), "hh:mm tt", null);
                    DateTime end = DateTime.ParseExact(dr["EndTime"].ToString(), "hh:mm tt", null);

                    //list.Add(new Timetable
                    //{
                    //    DayOfWeek = dr["DayOfWeek"].ToString(),
                    //    SubjectName = dr["SubjectName"].ToString(),
                    //    Teacher = dr["TeacherName"].ToString(),
                    //    Slot = start.Hours + "-" + end.Hours,
                    //    IsPresent = dr["IsPresent"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IsPresent"])
                    //});
                    list.Add(new Timetable
                    {
                        DayOfWeek = dr["DayOfWeek"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        Teacher = dr["TeacherName"].ToString(),

                        Slot = Convert.ToDateTime(dr["StartTime"]).ToString("hh:mm tt")
         + " - " +
           Convert.ToDateTime(dr["EndTime"]).ToString("hh:mm tt"),

                        IsPresent = dr["IsPresent"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IsPresent"])
                    });
                }

                dr.Close();

                // total subjects
                SqlCommand cmd2 = new SqlCommand(
                    "select count(*) from TimeTable  where ClassId=@ClassId", con);
                cmd2.Parameters.AddWithValue("@ClassId", Session["ClassId"]);

                ViewBag.TotalSubjects = cmd2.ExecuteScalar();

                // total lectures
                SqlCommand cmd3 = new SqlCommand(
                    "select count(*) from TimeTable  where ClassId=@ClassId", con);
                cmd3.Parameters.AddWithValue("@ClassId", Session["ClassId"]);

                ViewBag.TotalLectures = cmd3.ExecuteScalar();

                // pending fees
                SqlCommand cmd4 = new SqlCommand(
                    "select Remaining = fc.TotalFees - isnull(sum(fp.PayAmount),0) from FeeCommitment fc left join FeePayment fp on fc.StudentId = fp.StudentId where fc.StudentId=@studentId group by fc.FeeType,fc.TotalFees", con);
                cmd4.Parameters.AddWithValue("@studentId", studentId);
                ViewBag.PendingFees = cmd4.ExecuteScalar();


                // ✅ SEMESTER ATTENDANCE FOR PIE CHART
                SqlCommand cmd5 = new SqlCommand(@"
               
               select 
                SUM(case when IsPresent = 1 then 1 else 0 end) as PresentCount,
                SUM(case when IsPresent = 0 then 1 else 0 end) as AbsentCount
                from Attendance
                where StudentId = @studentId
                ", con);

                cmd5.Parameters.AddWithValue("@studentId", studentId);

                SqlDataReader dr2 = cmd5.ExecuteReader();

                int present = 0;
                int absent = 0;

                if (dr2.Read())
                {
                    present = dr2["PresentCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr2["PresentCount"]);
                    absent = dr2["AbsentCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr2["AbsentCount"]);
                }

                dr2.Close();

                ViewBag.Present = present;
                ViewBag.Absent = absent;
            }

            return View(list);
        }
    }
}