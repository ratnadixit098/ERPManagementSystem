using ERPManagementSystem.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.Collections.Generic;

public class CollegeController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;

    public ActionResult Create()
    {
        BindState();
        return View();
    }

    [HttpPost]
    public ActionResult Create(College model)
    {
        BindState();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("insert into College(CollegeCode,CollegeName,Address,StateId,DistrictId,PhoneNumber,Email) values(@CollegeCode,@CollegeName,@Address,@StateId,@DistrictId,@Phone,@Email)", con);

            cmd.Parameters.AddWithValue("@CollegeCode", model.CollegeCode);
            cmd.Parameters.AddWithValue("@CollegeName", model.CollegeName);
            cmd.Parameters.AddWithValue("@Address", model.Address);
            cmd.Parameters.AddWithValue("@StateId", model.StateId);
            cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
            cmd.Parameters.AddWithValue("@Phone", model.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", model.Email);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["Success"] = "College Saved Successfully";
        return RedirectToAction("Create");
    }
    public void BindState()
    {
        SqlConnection con = new SqlConnection(conStr);
        SqlDataAdapter da = new SqlDataAdapter("select StateId,StateName from State", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        List<SelectListItem> stateList = new List<SelectListItem>();

        foreach (DataRow dr in dt.Rows)
        {
            stateList.Add(new SelectListItem
            {
                Value = dr["StateId"].ToString(),
                Text = dr["StateName"].ToString()
            });
        }

        ViewBag.StateList = stateList;
    }

    public JsonResult GetDistrict(int StateId)
    {
        SqlConnection con = new SqlConnection(conStr);
        SqlDataAdapter da = new SqlDataAdapter(
            "select DistrictId, DistrictName from District where StateId=" + StateId, con);

        DataTable dt = new DataTable();
        da.Fill(dt);

        var list = new List<object>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new
            {
                DistrictId = row["DistrictId"],
                DistrictName = row["DistrictName"]
            });
        }

        return Json(list, JsonRequestBehavior.AllowGet);
    }
}