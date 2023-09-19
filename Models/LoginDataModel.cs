namespace TodoApi.Models
{
    public class LoginDataModel
    {
        public string? GrantType { get; set; }
        public string? LoginType { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}