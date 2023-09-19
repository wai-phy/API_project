using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("tbl_customer")]
    public class Customer
    {
        [Column("customer_id")]
        [Key]
        public int CustomerId { get; set; }

        // [Required]
        [Column("customer_name")]
        [StringLength(50)]
        public string CustomerName { get; set; } = "";

        [Column("register_date")]
        public DateTime? RegisterDate { get; set; } 

        [Column("customer_address")]
        public string CustomerAddress { get; set; } = "";

        [Column("customertype_id")]
        public int? CustomerTypeId { get; set; }

         [ForeignKey("CustomerTypeId")]
        public CustomerType? CustomerType { get; set; }
        
        [Column("customer_photo")]
        public string? CustomerPhoto { get; set;}

    }
}