using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class CustomerRequest
    {
        public int CustomerId { get; set; }
        [Required]
        [StringLength(10)]
        public string CustomerName { get; set; } = string.Empty;
        [StringLength(100)]
        public string CustomerAddress { get; set; } = string.Empty;

        public DateTime? RegisterDate { get; set; }
        public int? CustomerTypeId { get; set; }

        public string? CustomerPhoto { get; set;}


    }
}