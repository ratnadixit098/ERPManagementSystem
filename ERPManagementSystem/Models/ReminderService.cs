using ERPManagementSystem.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ERPManagementSystem
{
    public class ReminderService
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        public void SendFeeReminder()
        {
            SqlConnection con = new SqlConnection(conStr);

            string query = @"
            SELECT 
            s.StudentId,
            s.StudentName,
            s.Email,
            fc.TotalFees,
            fc.DueDate,
            isnull(sum(fp.PayAmount),0) Paid,
            fc.TotalFees - isnull(sum(fp.PayAmount),0) Remaining,
            fc.LastReminderDate
            FROM FeeCommitment fc
            JOIN Student s ON s.StudentId = fc.StudentId
            LEFT JOIN FeePayment fp ON fp.StudentId = fc.StudentId
            WHERE fc.DueDate < GETDATE()
            GROUP BY s.StudentId,s.StudentName,s.Email,fc.TotalFees,fc.DueDate,fc.LastReminderDate";

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
                    Due Date: {Convert.ToDateTime(row["DueDate"]).ToShortDateString()}<br/>
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