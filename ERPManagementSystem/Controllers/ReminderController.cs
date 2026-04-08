using ERPManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace ERPManagementSystem.Controllers
{
    public class ReminderController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        // GET: Reminder
        public void SendFeeReminder()
        {
            
            SqlConnection con = new SqlConnection(conStr);

            string query = @"
    SELECT 
s.StudentId,
s.FirstName+' '+LastName as StudentName,
s.Email,
fc.TotalFees,
FeeInstallment.DueDate,
isnull(sum(fp.PayAmount),0) Paid,
fc.TotalFees - isnull(sum(fp.PayAmount),0) Remaining,
fc.LastReminderDate
FROM FeeCommitment fc
JOIN Student s ON s.StudentId = fc.StudentId
left join FeeInstallment on FeeInstallment.CommitmentId=fc.CommitmentId
LEFT JOIN FeePayment fp ON fp.StudentId = fc.StudentId
WHERE  (FeeInstallment.DueDate < GETDATE() or fc.FeeType='Ontime')
GROUP BY s.StudentId,s.FirstName+' '+LastName,s.Email,fc.TotalFees,FeeInstallment.DueDate,fc.LastReminderDate";

            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                decimal remaining = Convert.ToDecimal(row["Remaining"]);
                if (remaining <= 0) continue;

                DateTime? lastSent = row["LastReminderDate"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(row["LastReminderDate"]);

                if (lastSent == null || lastSent.Value.AddDays(7) <= DateTime.Now)
                {
                    string body = $@"
            Dear {row["StudentName"]},<br/><br/>
            Your fee payment is overdue.<br/><br/>
            Due Date: {Convert.ToDateTime(row["DueDate"]== DBNull.Value? DateTime.Now.Date: row["DueDate"]).ToShortDateString()}<br/>
            Total Fee: {row["TotalFees"]}<br/>
            Remaining: {row["Remaining"]}<br/><br/>
            Please pay as soon as possible.<br/><br/>
            Regards,<br/>
            College Admin";

                    EmailHelper.SendEmail(
                        row["Email"].ToString(),
                        "Fee Payment Reminder",
                        body);

                    SqlCommand cmd = new SqlCommand(
                    "update FeeCommitment set LastReminderDate=GETDATE() where StudentId=@id", con);

                    cmd.Parameters.AddWithValue("@id", row["StudentId"]);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
}