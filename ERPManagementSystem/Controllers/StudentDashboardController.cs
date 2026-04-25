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
        SELECT 
            t.DayOfWeek,
            t.StartTime,
            t.EndTime,
            s.SubjectName,
            te.FirstName + ' ' + te.LastName AS TeacherName,
            a.IsPresent
        FROM TimeTable t
        LEFT JOIN Subject s ON s.SubjectId = t.SubjectId
        LEFT JOIN Teacher te ON te.TeacherId = t.TeacherId
        LEFT JOIN Attendance a 
            ON a.SubjectId = t.SubjectId 
            AND a.ClassId = t.ClassId
            AND a.StudentId = @studentId
            AND CAST(a.AttendanceDate AS DATE) = CAST(GETDATE() AS DATE)
        ", con);

                cmd.Parameters.AddWithValue("@studentId", studentId);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    DateTime start = DateTime.ParseExact(dr["StartTime"].ToString(), "hh:mm tt", null);
                    DateTime end = DateTime.ParseExact(dr["EndTime"].ToString(), "hh:mm tt", null);

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
                    "SELECT COUNT(*) FROM TimeTable WHERE ClassId = @ClassId", con);

                cmd2.Parameters.AddWithValue("@ClassId", Session["ClassId"]);
                ViewBag.TotalSubjects = cmd2.ExecuteScalar();

                // total lectures
                SqlCommand cmd3 = new SqlCommand(
                    "SELECT COUNT(*) FROM TimeTable WHERE ClassId = @ClassId", con);

                cmd3.Parameters.AddWithValue("@ClassId", Session["ClassId"]);
                ViewBag.TotalLectures = cmd3.ExecuteScalar();

                // pending fees
                SqlCommand cmd4 = new SqlCommand(@"
            SELECT 
                Remaining = fc.TotalFees - ISNULL(SUM(fp.PayAmount), 0)
            FROM FeeCommitment fc
            LEFT JOIN FeePayment fp 
                ON fc.StudentId = fp.StudentId
            WHERE fc.StudentId = @studentId
            GROUP BY fc.FeeType, fc.TotalFees
        ", con);

                cmd4.Parameters.AddWithValue("@studentId", studentId);
                ViewBag.PendingFees = cmd4.ExecuteScalar();

                // semester attendance for pie chart
                SqlCommand cmd5 = new SqlCommand(@"
            SELECT 
                SUM(CASE WHEN IsPresent = 1 THEN 1 ELSE 0 END) AS PresentCount,
                SUM(CASE WHEN IsPresent = 0 THEN 1 ELSE 0 END) AS AbsentCount
            FROM Attendance
            WHERE StudentId = @studentId
        ", con);

                cmd5.Parameters.AddWithValue("@studentId", studentId);

                SqlDataReader dr2 = cmd5.ExecuteReader();

                int present = 0;
                int absent = 0;

                if (dr2.Read())
                {
                    present = dr2["PresentCount"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(dr2["PresentCount"]);

                    absent = dr2["AbsentCount"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(dr2["AbsentCount"]);
                }

                dr2.Close();

                ViewBag.Present = present;
                ViewBag.Absent = absent;
            }

            return View(list);
        }
    }
}