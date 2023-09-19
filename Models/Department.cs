using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("tbl_department")]
    public partial class Department
    {
        [Column("dept_id")]
        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        [Required]
        [Column("dept_name")]
        public string DeptName { get; set; } = string.Empty;

    }
}