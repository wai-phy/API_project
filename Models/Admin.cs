using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("tbl_admin")]
    public class Admin : BaseModel
    {
        [Column("admin_id")]
        [Key]
        public int AdminId { get; set; }

        [Required]
        [Column("admin_name")]
        [StringLength(50)]
        public string AdminName { get; set; }  = "";

        [Column("admin_email")]
        public string Email { get; set; } = "";

        [Column("login_name")]
        public string LoginName { get; set; } = "";

        [Column("admin_password")]
        public string? Password { get; set; } 

        [Column("inactive")]
        public bool? Inactive { get; set;} 

        [Column("salt")]
        public string Salt { get; set;} ="";

         [Column("admin_level_id")]
        public int? AdminLevelId { get; set;}

         [ForeignKey("AdminLevelId")]
        public AdminLevel? AdminLevel  { get; set; }

        [Column("admin_photo")]
        public string? AdminPhoto { get; set; }
        


    }
}