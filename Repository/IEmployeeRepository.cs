using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IEmployeeRepository : IRepositoryBase<Employee>
    {
        Task<IEnumerable<Employee>> SearchEmployee(string searchTerm);
        Task<IEnumerable<EmployeeResult>> ListEmployee();
         bool IsExists(long id);
        new Task DeleteAsync(object employee, bool v);
    }
}