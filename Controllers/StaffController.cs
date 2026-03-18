using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers
{
    public class StaffController : Controller
    {
        #region StaffList
        public ActionResult<List<StaffModel>> StaffList()
        {
            List<StaffModel> list = new List<StaffModel>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                StaffModel s = new StaffModel();
                s.StaffID = Convert.ToInt32(reader["StaffId"]);
                s.StaffName = reader["StaffName"].ToString();
                s.MobileNo = reader["MobileNo"].ToString();
                s.EmailAddress = reader["EmailAddress"].ToString();
                s.Remarks = reader["Remarks"].ToString();

                list.Add(s);
            }

            reader.Close();

            con.Close();

            return View(list);
        }
        #endregion

        #region Add Edit
        [HttpGet]
        public IActionResult StaffAddEdit(int? id)
        {
            ViewBag.DepartmentList = FillDepartmentDropDown();

            if (id > 0)
            {
                StaffModel staff = GetStaffById(id.Value);
                return View(staff);
            }
            else
            {
                return View(new StaffModel());
            }
        }
        #endregion

        #region GetStaff
        public StaffModel GetStaffById(int id)
        {
            StaffModel staff = new StaffModel();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_SelectByPk", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StaffID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                staff.StaffID = Convert.ToInt32(reader["StaffID"]);
                staff.StaffName = reader["StaffName"].ToString();
                staff.MobileNo = reader["MobileNo"].ToString();
                staff.EmailAddress = reader["EmailAddress"].ToString();
                staff.Remarks = reader["Remarks"].ToString();
                staff.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
            }

            reader.Close();
            con.Close();

            return staff;
        }
        #endregion

        #region Delete Record
        public IActionResult DeleteStaff(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Staff_DeleteByPk";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@StaffID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Delete Successfully.";

                return RedirectToAction("StaffList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("StaffList");
            }
        }
        #endregion

        #region Save Record
        [HttpPost]
        public IActionResult Save(StaffModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.DepartmentList = FillDepartmentDropDown();
                    return View("StaffAddEdit", model);
                }

                model.Modified = DateTime.UtcNow;

                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (model.StaffID == 0)
                {
                    cmd.CommandText = "PR_MOM_Staff_Insert";
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Staff_UpdateByPk";
                    cmd.Parameters.AddWithValue("@StaffID", model.StaffID);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter deptId = new SqlParameter();
                deptId.ParameterName = "@DepartmentID";
                deptId.SqlDbType = SqlDbType.Int;
                deptId.Value = model.DepartmentID;

                SqlParameter staffName = new SqlParameter();
                staffName.ParameterName = "@StaffName";
                staffName.SqlDbType = SqlDbType.NVarChar;
                staffName.Value = model.StaffName;

                SqlParameter mobileNo = new SqlParameter();
                mobileNo.ParameterName = "@MobileNo";
                mobileNo.SqlDbType = SqlDbType.NVarChar;
                mobileNo.Value = model.MobileNo;

                SqlParameter emailAddress = new SqlParameter();
                emailAddress.ParameterName = "@EmailAddress";
                emailAddress.SqlDbType = SqlDbType.NVarChar;
                emailAddress.Value = model.EmailAddress;

                SqlParameter remarks = new SqlParameter();
                remarks.ParameterName = "@Remarks";
                remarks.SqlDbType = SqlDbType.NVarChar;
                remarks.Value = model.Remarks ?? (object)DBNull.Value;

                SqlParameter modified = new SqlParameter();
                modified.ParameterName = "@Modified";
                modified.SqlDbType = SqlDbType.DateTime;
                modified.Value = model.Modified;

                cmd.Parameters.Add(modified);
                cmd.Parameters.Add(deptId);
                cmd.Parameters.Add(staffName);
                cmd.Parameters.Add(mobileNo);
                cmd.Parameters.Add(emailAddress);
                cmd.Parameters.Add(remarks);

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if(noOfRows > 0)
                {
                    if(model.StaffID == 0)
                    {
                        TempData["Success"] = "Staff added successfully";
                    }
                    else
                    {
                        TempData["Success"] = "Staff updated successfully";

                    }
                }

                return RedirectToAction("StaffList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("StaffList");
            }
        }
        #endregion

        #region Drop Down
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
        #endregion

        #region Search 
        [HttpPost]
        public IActionResult StaffList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<StaffModel> list = GetStaff(searchText);
            return View("StaffList", list);
        }

        private List<StaffModel> GetStaff(string searchText)
        {
            List<StaffModel> list = new List<StaffModel>();

            SqlConnection con = new SqlConnection(
                "Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                StaffModel s = new StaffModel();
                s.StaffID = Convert.ToInt32(reader["StaffID"]);
                s.StaffName = reader["StaffName"].ToString();
                s.MobileNo = reader["MobileNo"].ToString();
                s.EmailAddress = reader["EmailAddress"].ToString();
                s.Remarks = reader["Remarks"].ToString();

                list.Add(s);
            }

            reader.Close();
            con.Close();

            return list;
        }
        #endregion

        #region ExportExcel
        public IActionResult ExportToExcel()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;"))
                {
                    con.Open();

                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_MOM_Staff_SelectAll";

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                        }
                    }
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Staff");

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dt.Columns[i].ColumnName;
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    for (int row = 0; row < dt.Rows.Count; row++)
                    {
                        for (int col = 0; col < dt.Columns.Count; col++)
                        {
                            worksheet.Cell(row + 2, col + 1).Value = dt.Rows[row][col]?.ToString();
                        }
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "StaffList.xlsx"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting data: " + ex.Message;
                return RedirectToAction("StaffList");
            }
        }
        #endregion

    }
}
