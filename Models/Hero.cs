using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{  
     [Table("tbl_hero")]
    public class Hero
    {
        [Column("hero_id")]
        public long HeroId { get; set; }

        [Column("hero_name")]
        [Required]
        [StringLength(20)]
        public string HeroName { get; set; } = "";

        [Column("address")]
        [StringLength(50)]
        public string HeroAddress { get; set; } = "";

        [Column("hero_secret")]
        public string? HeroSecret { get; set; }
    }
}