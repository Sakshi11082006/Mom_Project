using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers 
{
    public class DepartmentController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public DepartmentController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        #region Department List
        public ActionResult<List<DepartmentModel>> Index()
        {
            List<DepartmentModel> list = new List<DepartmentModel>();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DepartmentModel d = new DepartmentModel();
                d.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                d.DepartmentName = reader["DepartmentName"].ToString();
                d.DepartmentLogo = reader["DepartmentLogo"].ToString();

                list.Add(d);
            }

            reader.Close();
            con.Close();

            return View("DepartmentList", list);
        }
        #endregion

        #region Search 
        [HttpPost]

        public IActionResult Index(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<DepartmentModel> list = GetDepartments(searchText);
            return View("DepartmentList", list);
        }

        private List<DepartmentModel> GetDepartments(string searchText)
        {
            List<DepartmentModel> list = new List<DepartmentModel>();

            SqlConnection con = new SqlConnection(
                "Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DepartmentModel d = new DepartmentModel();
                d.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                d.DepartmentName = reader["DepartmentName"].ToString();
                d.DepartmentLogo = reader["DepartmentLogo"].ToString();

                list.Add(d);
            }

            reader.Close();
            con.Close();

            return list;
        }
        #endregion

        #region Department Add Edit
        public IActionResult DepartmentAddEdit(int? id)
        {
            if (id > 0)
            {
                DepartmentModel department = GetDepartmentById(id.Value);
                return View(department);
            }
            else
            {
                return View(new DepartmentModel());
            }
        }
        #endregion

        #region Get Department By Id
        public DepartmentModel GetDepartmentById(int id)
        {
            DepartmentModel department = new DepartmentModel();

            SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand("PR_MOM_Department_SelectByPk", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentID", id);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                department.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                department.DepartmentName = reader["DepartmentName"].ToString();
                department.DepartmentLogo = reader["DepartmentLogo"].ToString();
            }

            reader.Close();
            con.Close();

            return department;
        }
        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(DepartmentModel model)
        {
            ModelState.Remove("DepartmentName");
            ModelState.Remove("DepartmentLogo");
            if (string.IsNullOrEmpty(model.DepartmentName))

            {
                ModelState.AddModelError("DepartmentName", "Department name can not be null or empty.");
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("DepartmentAddEdit", model);
                }

                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");
                SqlCommand cmd = new SqlCommand(); 
                cmd.Connection = con;

                if (model.DepartmentID == 0)
                {
                    cmd.CommandText = "PR_MOM_Department_Insert";
                    TempData["Success"] = "Department added successfully";
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Department_UpdateByPk";
                    cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                    TempData["Success"] = "Department updated successfully";
                }

                cmd.CommandType = CommandType.StoredProcedure;

                string filePath = model.DepartmentLogo;

                if (model.LogoFile != null)
                {
                    string folder = Path.Combine(_env.WebRootPath, "uploads");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(model.LogoFile.FileName);

                    string fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.LogoFile.CopyTo(stream);
                    }

                    filePath = "/uploads/" + fileName;
                }

                else if (model.DepartmentID == 0)
                {
                    // only for NEW record
                    filePath = "/uploads/default.png";
                }

                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                cmd.Parameters.AddWithValue("@DepartmentLogo", filePath);


                if (model.DepartmentID == 0) // INSERT ONLY
                {
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);  // ✅ correct place
                }

                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Department Delete
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection("Server=SAKSHISANTOKI\\SQLEXPRESS;Database=MOM_DOTNET;Trusted_Connection=True;TrustServerCertificate=True;");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Department_DeleteByPk";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@DepartmentId";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Delete Successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] = "Foreign Key Constraint Violated.";
                return RedirectToAction("Index");
            }
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
                        cmd.CommandText = "PR_MOM_Department_SelectAll";

                        using (SqlDataReader dr  = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                        }
                    }
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Department");

                    for(int i=0; i < dt.Columns.Count; i++)
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
                            "DepartmentList.xlsx"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting data: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}
