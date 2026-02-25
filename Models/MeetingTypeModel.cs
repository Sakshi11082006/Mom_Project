using System.ComponentModel.DataAnnotations;

namespace Mom_Project.Models
{
    public class MeetingTypeModel
    {
        [Key]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Meeting Type name must be required")]
        [StringLength(100)]
        public string MeetingTypeName { get; set; }

        [Required(ErrorMessage = "Remark must be entered")]
        [StringLength(100)]
        public string Remarks { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime Modified { get; set; }
    }
}
