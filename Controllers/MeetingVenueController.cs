using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers
{
    public class MeetingVenueController : Controller
    {
        public ActionResult<List<MeetingVenueModel>> MeetingVenueList()
        {
            List<MeetingVenueModel> list = new List<MeetingVenueModel>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingVenueModel mv = new MeetingVenueModel();
                mv.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                mv.MeetingVenueName = reader["MeetingVenueName"].ToString();

                list.Add(mv);
            }

            reader.Close();

            con.Close();

            return View(list);
        }

        [HttpGet]
        public IActionResult MeetingVenueAddEdit(int? id)
        {
            if (id > 0)
            {
                MeetingVenueModel MeetingVenue = GetMeetingVenueById(id.Value);
                return View(MeetingVenue);
            }
            else
            {
                return View(new MeetingVenueModel());
            }
        }

        public MeetingVenueModel GetMeetingVenueById(int id)
        {
            MeetingVenueModel MeetingVenue = new MeetingVenueModel();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_MeetingVenue_SelectByPk", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MeetingVenueID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                MeetingVenue.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                MeetingVenue.MeetingVenueName = reader["MeetingVenueName"].ToString();
            }

            reader.Close();
            con.Close();

            return MeetingVenue;
        }

        [HttpPost]
        public IActionResult Save(MeetingVenueModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("MeetingVenueAddEdit", model);
                }

                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (model.MeetingVenueID == 0)
                {
                    cmd.CommandText = "PR_MOM_MeetingVenue_Insert";
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = "PR_MOM_MeetingVenue_UpdateByPk";
                    cmd.Parameters.AddWithValue("@MeetingVenueID", model.MeetingVenueID);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter mv = new SqlParameter();
                mv.ParameterName = "@MeetingVenueName";
                mv.SqlDbType = SqlDbType.NVarChar;
                mv.Value = model.MeetingVenueName;

                cmd.Parameters.Add(mv);

                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if (noOfRows > 0)
                {
                    if (model.MeetingVenueID == 0)
                    {
                        TempData["Success"] = "Meeting venue added successfully";
                    }
                    else
                    {
                        TempData["Success"] = "Meeting venue updated successfully";
                    }
                }

                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingVenueList");
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingVenue_DeleteByPk";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingVenueID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Delete Successfully.";
                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingVenueList");
            }
        }

        #region Search 
        [HttpPost]
        public IActionResult MeetingVenueList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<MeetingVenueModel> list = GetMeetingVenue(searchText);
            return View("MeetingVenueList", list);
        }

        private List<MeetingVenueModel> GetMeetingVenue(string searchText)
        {
            List<MeetingVenueModel> list = new List<MeetingVenueModel>();

            SqlConnection con = new SqlConnection(
                "Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingVenueModel mv = new MeetingVenueModel();
                mv.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                mv.MeetingVenueName = reader["MeetingVenueName"].ToString();

                list.Add(mv);
            }

            reader.Close();
            con.Close();

            return list;
        }
        #endregion

    }
}
