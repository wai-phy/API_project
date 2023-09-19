namespace TodoApi.Models
{
    public class HeroSearchPayLoad
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? HeroNameTerm { get; set; }

        public string? AddressTerm { get; set; }
    }
}