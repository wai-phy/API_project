namespace TodoApi.Repositories
{
    public class AdminResult
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? LoginName { get; set; } = "";
        public string? Password { get; set; } 
        public bool? Inactive { get; set; }
        public string? AdminLevelName { get; set; } 
        public string? AdminPhoto { get; set; }
        public int? AdminLevelId { get; set; }
    }
}