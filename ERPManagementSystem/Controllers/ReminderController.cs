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
    [CheckSession]
    public class ReminderController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

        // GET: Reminder
        public void SendFeeReminder()
        {
            
            SqlConnection con = new SqlConnection(conStr);

            string query = @"
  select *,TotalFees - isnull((Paid),0) Remaining from ( SELECT Amount=sum(Amount), Paid=max(PayAmount), s.StudentId, DueDate=max(FeeInstallment.DueDate) ,TotalFees=max(TotalFees), LastReminderDate=max(LastReminderDate) ,StudentName=  s.FirstName+' '+LastName, s.Email FROM (select* from FeeCommitment )fc JOIN Student s ON s.StudentId = fc.StudentId left join FeeInstallment on FeeInstallment.CommitmentId=fc.CommitmentId LEFT JOIN (select PayAmount=sum(PayAmount),StudentId from FeePayment fp group by StudentId)fp ON fp.StudentId = fc.StudentId WHERE  (FeeInstallment.DueDate < convert(date,GETDATE()) or fc.FeeType='Ontime') group by s.StudentId, s.FirstName+' '+LastName, s.Email )t where convert(decimal,Paid)< convert(decimal,Amount)";

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