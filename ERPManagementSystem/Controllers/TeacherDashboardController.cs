using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERPManagementSystem.Models;

namespace ERPManagementSystem.Controllers
{
    public class TeacherDashboardController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public ActionResult TeacherDashboard()
        {
            int teacherId = 1;

            List<TeacherDashboard> list = new List<TeacherDashboard>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                select 
               distinct t.DayOfWeek,
                t.StartTime,
                t.EndTime,
                s.SubjectName,
ClassName=CourseName+' '+DepartmentName+' '+Section,
                ClassName=Section,
                case when a.AttendanceId is null then 0 else 1 end as IsTaken
                from TimeTable t
                left join Subject s on s.SubjectId=t.SubjectId
                left join Class c on c.ClassId=t.ClassId
left join Course co on co.CourseId=s.CourseId
left join Department dep on dep.DepartmentId=co.DepartmentId
                left join Attendance a 
                    on a.SubjectId=t.SubjectId 
                    and a.ClassId=t.ClassId
                    and cast(a.AttendanceDate as date)=cast(getdate() as date)
                where t.TeacherId=@teacherId
                and t.DayOfWeek = DATENAME(WEEKDAY, GETDATE())
                ", con);

                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TimeSpan start = (TimeSpan)dr["StartTime"];
                    TimeSpan end = (TimeSpan)dr["EndTime"];

                    list.Add(new TeacherDashboard
                    {
                        DayOfWeek = dr["DayOfWeek"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        Slot = start.Hours + ":00 - " + end.Hours + ":00",
                        IsAttendanceTaken = Convert.ToInt32(dr["IsTaken"]) == 1
                    });
                }

                dr.Close();

                // total classes today
                SqlCommand cmd2 = new SqlCommand(@"
                select count(*) 
                from TimeTable 
                where TeacherId=@teacherId
                and DayOfWeek = DATENAME(WEEKDAY, GETDATE())
                ", con);

                cmd2.Parameters.AddWithValue("@teacherId", teacherId);
                ViewBag.TotalClasses = cmd2.ExecuteScalar();


                // attendance taken today
                SqlCommand cmd3 = new SqlCommand(@"
                select count(distinct SubjectId) 
                from Attendance
                where TeacherId=@teacherId
                and cast(AttendanceDate as date)=cast(getdate() as date)
                ", con);

                cmd3.Parameters.AddWithValue("@teacherId", teacherId);
                ViewBag.AttendanceTaken = cmd3.ExecuteScalar();


                int total = Convert.ToInt32(ViewBag.TotalClasses);
                int taken = Convert.ToInt32(ViewBag.AttendanceTaken);

                ViewBag.Pending = total - taken;
            }

            return View(list);
        }
    }
}