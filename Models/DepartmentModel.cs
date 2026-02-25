using System.ComponentModel.DataAnnotations;

namespace Mom_Project.Models
{
    public class DepartmentModel
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }
    }
}
