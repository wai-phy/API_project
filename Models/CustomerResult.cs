namespace TodoApi.Repositories
{
    public class CustomerResult
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";

        // public string? CustomerPhoto { get; set; }
        public int? CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; } 
        public string? CustomerTypeDescription { get; set; } 
    }
}