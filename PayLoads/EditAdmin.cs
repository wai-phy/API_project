using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TodoApi.Models;

namespace TodoApi.Payloads
{
    public class EditAdmin
    {
        public int AdminID { get; set; }
        public int AdminLevelID { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;

        [ForeignKey("AdminLevelID")]
        public AdminLevel? AdminLevel { get; set; }

        [NotMapped]
        public string? AdminPhoto { get; set; }  //Not Mapped attribute, not to store in actual database colulmn
    }
}