namespace TodoApi.Repositories
{
    public class HeroSearchPayload
    {
         public long Id { get; set; }
        public string? Name { get; set; }
        public string? HeroNameTerm { get; set; }

        public string? AddressTerm { get; set; }
    }
}