using System.Data;
using System.Linq;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Repositories
{
    public class HeroRepository : RepositoryBase<Hero>, IHeroRepository
    {
        public HeroRepository(TodoContext repositoryContext) : base(repositoryContext) { }

        public async Task<IEnumerable<Hero>> SearchHero(string searchTerm)
        {
            return await RepositoryContext.Heroes
                        .Where(s => s.HeroName.Contains(searchTerm))
                        .OrderBy(s => s.HeroId).ToListAsync();
        }

        public async Task<IEnumerable<Hero>> SearchHeroMultiple(HeroSearchPayload SearchObj)
        {
            return await RepositoryContext.Heroes
                        .Where(s => s.HeroName.Contains(SearchObj.HeroNameTerm ?? "") || s.HeroAddress.Contains(SearchObj.AddressTerm ?? ""))
                        .OrderBy(s => s.HeroId).ToListAsync();
        }

        public bool IsExists(long id)
        {
            return RepositoryContext.Heroes.Any(e => e.HeroId == id);
        }
    }

}