using ERPManagementSystem.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;


public class FeesController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindStudent();
        return View();
    }

    [HttpPost]
    public ActionResult Create(Fees model)
    {
        BindStudent();

        model.PendingAmount = model.TotalFees - model.PaidAmount;

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into Fees(StudentId,Semester,TotalFees,PaidAmount,PendingAmount,PaymentDate) values(@Student,@Sem,@Total,@Paid,@Pending,@Date)", con);

            cmd.Parameters.AddWithValue("@Student", model.StudentId);
            cmd.Parameters.AddWithValue("@Sem", model.Semester);
            cmd.Parameters.AddWithValue("@Total", model.TotalFees);
            cmd.Parameters.AddWithValue("@Paid", model.PaidAmount);
            cmd.Parameters.AddWithValue("@Pending", model.PendingAmount);
            cmd.Parameters.AddWithValue("@Date", model.PaymentDate);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "Fees Saved Successfully";
        return RedirectToAction("Create");
    }

    public void BindStudent()
    {
        SqlDataAdapter da = new SqlDataAdapter("select StudentId,StudentName=FirstName+' '+LastName from Student", conStr);
        DataTable dt = new DataTable();
        da.Fill(dt);

        List<SelectListItem> list = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SelectListItem
            {
                Value = row["StudentId"].ToString(),
                Text = row["StudentName"].ToString()
            });
        }

        ViewBag.StudentList = list;
    }
}