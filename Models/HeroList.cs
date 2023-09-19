using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TodoApi.Models
{
    public class HeroList
    {
        public long HeroListId { get; set; }
        public string? HeroListName { get; set; }
        public string? HeroListAge { get; set; }
        public string? HeroListAddress { get; set; }
    }
}