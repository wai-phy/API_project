using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IHeroRepository : IRepositoryBase<Hero>
    {
        Task<IEnumerable<Hero>> SearchHero(string searchName);
        Task<IEnumerable<Hero>> SearchHeroMultiple(HeroSearchPayload SearchObj);
        bool IsExists(long id);
        new Task DeleteAsync(object employee, bool v);
    }
}