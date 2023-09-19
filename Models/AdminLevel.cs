using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("tbl_adminlevel")]
    public class AdminLevel
    {
        [Column("admin_level_id")]
        [Key]
        public int AdminLevelId { get; set; }

        [Required]
        [Column("adminlevel_name")]
        [StringLength(50)]
        public string AdminLevelName { get; set; } = string.Empty;

      
    }
}