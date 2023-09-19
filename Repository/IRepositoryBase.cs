using System.Linq.Expressions;

namespace TodoApi.Repositories
{
    public interface IRepositoryBase<T>
    {
        T? FindByID(long ID);
        T? FindByID(int ID);
        Task<T?> FindByIDAsync(long ID);
        Task<T?> FindByIDAsync(int ID);
        IEnumerable<T> FindAll();
        Task<IEnumerable<T>> FindAllAsync();
        T? FindByCompositeID(long ID1, long ID2);
        T? FindByCompositeID(int ID1, int ID2);
        Task<T?> FindByCompositeIDAsync(long ID1, long ID2);
        Task<T?> FindByCompositeIDAsync(int ID1, int ID2);
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        bool AnyByCondition(Expression<Func<T, bool>> expression);
        Task<bool> AnyByConditionAsync(Expression<Func<T, bool>> expression);
        void Create(dynamic entity, bool flush = true);
        void CreateRange(dynamic entity, bool flush = true);
        void Update(dynamic entity, bool flush = true);
        void UpdateRange(dynamic entity, bool flush = true);
        void Delete(dynamic entity, bool flush = true);
        void DeleteRange(dynamic entity, bool flush = true);
        void DiscardChanges();
        void Save();
        Task CreateAsync(dynamic entity, bool flush = true);
        Task UpdateAsync(dynamic entity, bool flush = true);
        Task DeleteAsync(dynamic entity, bool flush = true);
        Task SaveAsync();
    }
}
