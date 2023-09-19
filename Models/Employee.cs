using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("tbl_employee")]
    public class Employee
    {
        [Column("employee_id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("employee_name")]
        [StringLength(50)]
        public string EmployeeName { get; set; } = string.Empty;

        [Column("employee_address")]
        [StringLength(100)]
        public string EmployeeAddress { get; set; } = string.Empty;

        [Column("employee_department_id")]
        public int? EmpDepartmentId { get; set; }
        
        [ForeignKey("EmpDepartmentId")]
        public Department? EmpDepartment { get; set; }
    }
}