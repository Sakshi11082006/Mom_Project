using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers
{
    public class MeetingsController : Controller
    {
        #region Meetings List
        public ActionResult<List<MeetingsModel>> MeetingsList()
        {
            List<MeetingsModel> list = new List<MeetingsModel>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingsModel meetings = new MeetingsModel();
                meetings.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                meetings.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                meetings.MeetingDescription = reader["MeetingDescription"].ToString();
                meetings.DocumentPath = reader["DocumentPath"].ToString();
                meetings.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);
                meetings.CancellationDateTime =
        reader["CancellationDateTime"] == DBNull.Value
            ? (DateTime?)null
    : Convert.ToDateTime(reader["CancellationDateTime"]);

                meetings.CancellationReason = reader["CancellationReason"].ToString();


                list.Add(meetings);
            }

            reader.Close();
            con.Close();
            return View(list);
        }
        #endregion

        #region Meetings Add Edit
        [HttpGet]
        public IActionResult MeetingsAddEdit(int? id)
        {
            ViewBag.MeetingTypeList = FillMeetingTypeDropDown();
            ViewBag.DepartmentList = FillDepartmentDropDown();
            ViewBag.MeetingVenueList = FillMeetingVenueDropDown();

            if (id > 0)
            {
                MeetingsModel meetings = GetMeetingsById(id.Value);
                return View(meetings);
            }
            else
            {
                return View(new MeetingsModel());
            }
        }
        #endregion

        #region Get Meetings By Id
        public MeetingsModel GetMeetingsById(int id)
        {
            MeetingsModel meetings = new MeetingsModel();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_Meetings_SelectByPk", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MeetingID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                meetings.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                meetings.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                meetings.MeetingDescription = reader["MeetingDescription"].ToString();
                meetings.DocumentPath = reader["DocumentPath"].ToString();
                meetings.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);
                meetings.CancellationDateTime = reader["CancellationDateTime"] == DBNull.Value
                ? (DateTime?)null : Convert.ToDateTime(reader["CancellationDateTime"]);

                meetings.CancellationReason = reader["CancellationReason"].ToString();
                meetings.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                meetings.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                meetings.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);

            }

            reader.Close();
            con.Close();

            return meetings;
        }
        #endregion

        #region Meetings Delete
        public IActionResult DeleteMeetings(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Meetings_DeleteByPk";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Delete Successfully.";

                return RedirectToAction("MeetingsList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingsList");
            }
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(MeetingsModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentList = FillDepartmentDropDown();
                    return View("MeetingsAddEdit", model);
                }

                model.Modified = DateTime.UtcNow;

                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.MeetingID == 0)
                {
                    cmd.CommandText = "PR_MOM_Meetings_Insert";
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                    TempData["Success"] = "Meeting added successfully";
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Meetings_UpdateByPk";
                    cmd.Parameters.AddWithValue("@MeetingID", model.MeetingID);
                    TempData["Success"] = "Meeting updated successfully";
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter mTypeId = new SqlParameter();
                mTypeId.ParameterName = "@MeetingTypeID";
                mTypeId.SqlDbType = SqlDbType.Int;
                mTypeId.Value = model.MeetingTypeID;

                SqlParameter deptId = new SqlParameter();
                deptId.ParameterName = "@DepartmentID";
                deptId.SqlDbType = SqlDbType.Int;
                deptId.Value = model.DepartmentID;

                SqlParameter meetingsVenue = new SqlParameter();
                meetingsVenue.ParameterName = "@MeetingVenueID";
                meetingsVenue.SqlDbType = SqlDbType.Int;
                meetingsVenue.Value = model.MeetingVenueID;

                SqlParameter mDescription = new SqlParameter();
                mDescription.ParameterName = "@MeetingDescription";
                mDescription.SqlDbType = SqlDbType.NVarChar;
                mDescription.Value = model.MeetingDescription;


                SqlParameter meetingsDate = new SqlParameter();
                meetingsDate.ParameterName = "@MeetingDate";
                meetingsDate.SqlDbType = SqlDbType.DateTime;
                meetingsDate.Value = model.MeetingDate;

                SqlParameter mDocPath = new SqlParameter();
                mDocPath.ParameterName = "@DocumentPath";
                mDocPath.SqlDbType = SqlDbType.NVarChar;
                mDocPath.Value = model.DocumentPath;

                cmd.Parameters.Add(mTypeId);
                cmd.Parameters.Add(deptId);
                cmd.Parameters.Add(meetingsVenue);
                cmd.Parameters.Add(mDescription);
                cmd.Parameters.Add(meetingsDate);
                cmd.Parameters.Add(mDocPath);


                cmd.Parameters.AddWithValue("@IsCancelled", model.IsCancelled ?? false);
                cmd.Parameters.AddWithValue("@CancellationDateTime", (object?)model.CancellationDateTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CancellationReason", (object?)model.CancellationReason ?? DBNull.Value);

                con.Open();

                int noOfRows = cmd.ExecuteNonQuery();

                con.Close();

                return RedirectToAction("MeetingsList");

            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingsList");

            }

        }

        #endregion

        #region Drop Down
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

        public List<SelectListItem> FillDepartmentDropDown()
        {

            List<SelectListItem> deptList = new List<SelectListItem>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                deptList.Add(new SelectListItem(reader["DepartmentName"].ToString(), reader["DepartmentID"].ToString()));
            }

            reader.Close();
            con.Close();

            return deptList;
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
        #endregion

        #region Search 
        [HttpPost]
        public IActionResult MeetingsList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<MeetingsModel> list = GetMeetings(searchText);
            return View("MeetingsList", list);
        }

        private List<MeetingsModel> GetMeetings(string searchText)
        {
            List<MeetingsModel> list = new List<MeetingsModel>();

            SqlConnection con = new SqlConnection(
                "Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingsModel meetings = new MeetingsModel();
                meetings.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                meetings.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                meetings.MeetingDescription = reader["MeetingDescription"].ToString();
                meetings.DocumentPath = reader["DocumentPath"].ToString();
                meetings.IsCancelled = reader["IsCancelled"] != DBNull.Value
                       && Convert.ToBoolean(reader["IsCancelled"]);

                meetings.CancellationDateTime = reader["CancellationDateTime"] != DBNull.Value
                                                ? Convert.ToDateTime(reader["CancellationDateTime"])
                                                : null;

                meetings.CancellationReason = reader["CancellationReason"].ToString();

                list.Add(meetings);
            }

            reader.Close();
            con.Close();

            return list;
        }
        #endregion
    }
}

