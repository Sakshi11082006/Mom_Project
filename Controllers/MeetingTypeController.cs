using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers
{
    public class MeetingTypeController : Controller
    {
        public ActionResult<List<MeetingTypeModel>> MeetingTypeList()
        {
            List <MeetingTypeModel> list = new List <MeetingTypeModel> ();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                MeetingTypeModel mt = new MeetingTypeModel ();  
                mt.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                mt.MeetingTypeName = reader["MeetingTypeName"].ToString();
                mt.Remarks = reader["Remarks"].ToString();

                list.Add (mt);
            }

            reader.Close();
            con.Close();

            return View(list);
        }

        #region Search 
        [HttpPost]

        public IActionResult MeetingTypeList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<MeetingTypeModel> list = GetMeetingType(searchText);
            return View("MeetingTypeList", list);
        }

        private List<MeetingTypeModel> GetMeetingType(string searchText)
        {
            List<MeetingTypeModel> list = new List<MeetingTypeModel>();

            SqlConnection con = new SqlConnection(
                "Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingTypeModel mt = new MeetingTypeModel();
                mt.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                mt.MeetingTypeName = reader["MeetingTypeName"].ToString();
                mt.Remarks = reader["Remarks"].ToString();

                list.Add(mt);
            }

            reader.Close();
            con.Close();

            return list;
        }
        #endregion

        [HttpGet]
        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (id > 0)
            {
                MeetingTypeModel mType = GetMeetingTypeById(id.Value);
                return View(mType);
            }
            else
            {
                return View(new MeetingTypeModel());
            }
        }

        public MeetingTypeModel GetMeetingTypeById(int id)
        {
            MeetingTypeModel mType = new MeetingTypeModel();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MeetingType_SelectByPk", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MeetingTypeID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                mType.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                mType.MeetingTypeName = reader["MeetingTypeName"].ToString();
                mType.Remarks = reader["Remarks"].ToString();
            }

            reader.Close();
            con.Close();

            return mType;
        }

        [HttpPost]
        public IActionResult Save(MeetingTypeModel model)
        {
            ModelState.Remove("MeetingTypeName");
            if (string.IsNullOrEmpty(model.MeetingTypeName))
            {
                ModelState.AddModelError("MeetingTypeName", "Meeting Type name can not be null or empty.");
            }
            try
            {     
                if (!ModelState.IsValid)
                {
                    return View("MeetingTypeAddEdit", model);
                }

                model.Modified = DateTime.UtcNow;

                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (model.MeetingTypeID == 0)
                {
                    cmd.CommandText = "PR_MOM_MeetingType_Insert";
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = "PR_MOM_MeetingType_UpdateByPk";
                    cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeID);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter MeetingTypeName = new SqlParameter();
                MeetingTypeName.ParameterName = "@MeetingTypeName";
                MeetingTypeName.SqlDbType = SqlDbType.NVarChar;
                MeetingTypeName.Value = model.MeetingTypeName;

                SqlParameter remark = new SqlParameter();
                remark.ParameterName = "@Remarks";
                remark.SqlDbType = SqlDbType.NVarChar;
                remark.Value = model.Remarks;

                SqlParameter modified = new SqlParameter();
                modified.ParameterName = "@Modified";
                modified.SqlDbType = SqlDbType.DateTime;
                modified.Value = DateTime.Now;

                cmd.Parameters.Add(MeetingTypeName);
                cmd.Parameters.Add(remark);
                cmd.Parameters.Add(modified);

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if(noOfRows > 0)
                {
                    if(model.MeetingTypeID == 0)
                    {
                        TempData["Success"] = "Meeting type added successfully";
                    }
                    else
                    {
                        TempData["Success"] = "Meeting type updated successfully";
                    }
                }

                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("MeetingTypeList");
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingType_DeleteByPk";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingTypeID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Delete Successfully.";

                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("MeetingTypeList");
            }
        }
    }
}
