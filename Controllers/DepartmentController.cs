using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mom_Project.Models;
using System.Data;

namespace Mom_Project.Controllers 
{
    public class DepartmentController : Controller
    {
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

                list.Add(d);
            }

            reader.Close();
            con.Close();

            return View("DepartmentList", list);
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
                    cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                }
                else
                {
                    cmd.CommandText = "PR_MOM_Department_UpdateByPk";
                    cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter deptName = new SqlParameter();
                deptName.ParameterName = "@DepartmentName";
                deptName.SqlDbType = SqlDbType.NVarChar;
                deptName.Value = model.DepartmentName;

                cmd.Parameters.Add(deptName);

                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);

                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();
                con.Close();

                if (noOfRows > 0)
                {
                    if (model.DepartmentID == 0)
                    {
                        TempData["Success"] = "Department added successfully";
                    }
                    else
                    {
                        TempData["Success"] = "Department updated successfully";
                    }
                }

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

    }
}
