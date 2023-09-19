using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class AdminRequest
    {
        public int AdminId { get; set; }
        [Required]
        [StringLength(10)]
        public string AdminName { get; set; } = string.Empty;
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public bool? Inactive { get; set; }
        public string? LoginName { get; set; } = string.Empty;
        public int? AdminLevelId { get; set; }

        public string? AdminPhoto { get; set;}


    }
}