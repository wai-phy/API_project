using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("tbl_customertype")]
    public class CustomerType
    {
        [Column("customertype_id")]
        [Key]
        public int CustomerTypeId { get; set; }

        // [Required]
        [Column("customertype_name")]
        [StringLength(50)]
        public string CustomerTypeName { get; set; } = string.Empty;

        [Column("customertype_description")]
        public string? CustomerTypeDescription { get; set; }  = string.Empty;
    }
}