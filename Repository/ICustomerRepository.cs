using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        Task<IEnumerable<Customer>> SearchCustomer(string searchTerm);
        Task<IEnumerable<CustomerResult>> ListCustomer();
         bool IsExists(long id);
        
    }
}