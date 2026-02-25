using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers
{
    public class MeetingsController : Controller
    {
        public ActionResult<List<MeetingsModel>> MeetingsList()
        {
            List <MeetingsModel> list = new List<MeetingsModel>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingsModel m = new MeetingsModel();
                m.MeetingID = Convert.ToInt32(reader["MeetingTypeID"]);

                m.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                m.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                m.MeetingDescription = reader["MeetingDescription"].ToString();
                m.DocumentPath = reader["DocumentPath"].ToString();
                m.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);
                m.CancellationReason = reader["CancellationReason"].ToString();

                list.Add(m);
            }

            reader.Close();

            con.Close();

            return View("MeetingsList",list);
        }

        public IActionResult MeetingsAddEdit()
        {
            ViewBag.DepartmentList = FillDepartmentDropDown();
            ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
            ViewBag.MeetingVenueList = FillMeetingVenueDropDown();
            return View();
        }

        public IActionResult Save(MeetingsModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentList = FillDepartmentDropDown();
                    ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
                    ViewBag.MeetingVenueList = FillMeetingVenueDropDown();
                    return View("MeetingsAddEdit", model);
                }


                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;

                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.MeetingID == 0)
                {
                    cmd.CommandText = "PR_MOM_Meetings_Insert";
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Meetings_UpdateByPk";
                    cmd.Parameters.AddWithValue("@MeetingID", model.MeetingID);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter dattime = new SqlParameter();
                dattime.ParameterName = "@MeetingDate";
                dattime.SqlDbType = SqlDbType.DateTime;
                dattime.Value = model.MeetingDate;

                SqlParameter MVId = new SqlParameter();
                MVId.ParameterName = "@MeetingVenueID";
                MVId.SqlDbType = SqlDbType.Int;
                MVId.Value = model.MeetingVenueID;

                SqlParameter MTId = new SqlParameter();
                MTId.ParameterName = "@MeetingTypeID";
                MTId.SqlDbType = SqlDbType.Int;
                MTId.Value = model.MeetingTypeID;

                SqlParameter deptId = new SqlParameter();
                deptId.ParameterName = "@DepartmentID";
                deptId.SqlDbType = SqlDbType.Int;
                deptId.Value = model.DepartmentID;

                SqlParameter MTDes = new SqlParameter();
                MTDes.ParameterName = "@MeetingDescription";
                MTDes.SqlDbType = SqlDbType.NVarChar;
                MTDes.Value = model.MeetingDescription;

                SqlParameter DocPath = new SqlParameter();
                DocPath.ParameterName = "@DocumentPath";
                DocPath.SqlDbType = SqlDbType.NVarChar;
                DocPath.Value = model.DocumentPath;

                SqlParameter created = new SqlParameter();
                created.ParameterName = "@Created";
                created.SqlDbType = SqlDbType.DateTime;
                created.Value = model.Created;

                SqlParameter modified = new SqlParameter();
                modified.ParameterName = "@Modified";
                modified.SqlDbType = SqlDbType.DateTime;
                modified.Value = model.Modified;

                SqlParameter IsCancel = new SqlParameter();
                IsCancel.ParameterName = "@IsCancelled";
                IsCancel.SqlDbType = SqlDbType.Bit;
                IsCancel.Value = model.IsCancelled;

                SqlParameter CancelTime = new SqlParameter();
                CancelTime.ParameterName = "@CancellationDateTime";
                CancelTime.SqlDbType = SqlDbType.DateTime;
                CancelTime.Value = model.CancellationDateTime;

                SqlParameter CancelReason = new SqlParameter();
                CancelReason.ParameterName = "@CancellationReason";
                CancelReason.SqlDbType = SqlDbType.NVarChar;
                CancelReason.Value = model.CancellationReason;

                cmd.Parameters.Add(dattime);
                cmd.Parameters.Add(MVId);
                cmd.Parameters.Add(MTId);
                cmd.Parameters.Add(deptId);
                cmd.Parameters.Add(MTDes);
                cmd.Parameters.Add(DocPath);
                cmd.Parameters.Add(created);
                cmd.Parameters.Add(modified);
                cmd.Parameters.Add(IsCancel);
                cmd.Parameters.Add(CancelTime);
                cmd.Parameters.Add(CancelReason);

                con.Open();

                int noOfRows = cmd.ExecuteNonQuery();

                if (noOfRows > 0)
                {
                    TempData["Success"] = "Record Inserted";
                }
                con.Close();


                return RedirectToAction("MeetingsList");

            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingsList");

            }

        }

        public List<SelectListItem> FillDepartmentDropDown()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new SelectListItem(reader["DepartmentName"].ToString(), reader["DepartmentID"].ToString()));

            }
            reader.Close();
            con.Close();
            return list;
        }

        public List<SelectListItem> FillMeetingTypeDropDown()
        {

            List<SelectListItem> meetingTypeList = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                meetingTypeList.Add(new SelectListItem(reader["MeetingTypeName"].ToString(), reader["MeetingTypeID"].ToString()));
            }

            reader.Close();
            con.Close();

            return meetingTypeList;
        }

        public List<SelectListItem> FillMeetingVenueDropDown()
        {

            List<SelectListItem> venueList = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                venueList.Add(new SelectListItem(reader["MeetingVenueName"].ToString(), reader["MeetingVenueID"].ToString()));
            }

            reader.Close();
            con.Close();

            return venueList;
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingMember_DeleteByPk";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingsID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MeetingsList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingsList");
            }
        }
    }
}