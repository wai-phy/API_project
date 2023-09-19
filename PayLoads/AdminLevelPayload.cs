using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Payloads
{
    public class AdminLevelPayload
    {
        public int? AdminLevelID { get; set; }
        public string AdminLevelName { get; set; } = string.Empty;
        public string? RestrictIPList { get; set; }
        public string? Description { get; set; }
        public string? Remark { get; set; }
    }
}
