using System.Data;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using Serilog;

namespace TodoApi.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(TodoContext repositoryContext) : base(repositoryContext) { }

        public async Task<IEnumerable<Customer>> SearchCustomer(string filter)
        {
            try{
                ExpandoObject queryFilter = new();
                queryFilter.TryAdd("@filter","%" + filter + "%");
                queryFilter.TryAdd("@filterid", filter);

                var SelectQuery = @"SELECT c.customer_id as Id, c.customer_name as CustomerName
                                 FROM tbl_customer c WHERE c.customer_name LIKE @filter 
                                 OR c.customer_id = @filterid ORDER BY c.customer_name LIMIT 0, 5";
                List<Customer> custResult = await RepositoryContext.RunExecuteSelectQuery<Customer>(SelectQuery,queryFilter);
                return custResult;
            }
            catch(Exception ex){
                Log.Error("GetCustomer fail" + ex.Message);
                return new List<Customer>();
            }
            return await RepositoryContext.Customers
                        .Where(s => s.CustomerName.Contains(filter))
                        .OrderBy(s => s.CustomerId).ToListAsync();
        }

        public async Task<IEnumerable<CustomerResult>> ListCustomer()
        {
            // return await RepositoryContext.Employees
            //             .OrderBy(s => s.Id).ToListAsync();
            // return await RepositoryContext.Employees
            //             .Include(e => e.EmpDepartment)
            //             .OrderBy(s => s.Id).ToListAsync();
            // return await RepositoryContext.Employees
            //             .Select(e => new EmployeeResult{
            //                 Id = e.Id,
            //                 EmployeeName = e.EmployeeName,
            //                 EmployeeAddress = e.EmployeeAddress,
            //                 EmpDepartmentId = e.EmpDepartmentId
            //             })
                        // .OrderBy(s => s.Id).ToListAsync();
            return await RepositoryContext.Customers
                        .Select(e => new CustomerResult{
                            Id = e.CustomerId,
                            CustomerName = e.CustomerName,
                            CustomerAddress = e.CustomerAddress,
                            // CustomerPhoto = e.CustomerPhoto,
                            CustomerTypeId = e.CustomerTypeId,
                            CustomerTypeName = e.CustomerType!.CustomerTypeName,
                            CustomerTypeDescription = e.CustomerType!.CustomerTypeDescription
                        })
                        .OrderBy(s => s.Id).ToListAsync();
        }


        public bool IsExists(long id)
        {
            return RepositoryContext.Customers.Any(e => e.CustomerId == id);
        }

       
    }

}