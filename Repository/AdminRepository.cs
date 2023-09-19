using System.Data;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace TodoApi.Repositories
{
    public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
    {
        public AdminRepository(TodoContext repositoryContext) : base(repositoryContext) { }

        public async Task<IEnumerable<Admin>> SearchAdmin(string searchTerm)
        {
            return await RepositoryContext.Admins
                        .Where(s => s.AdminName.Contains(searchTerm))
                        .OrderBy(s => s.AdminId).ToListAsync();
        }

        public async Task<IEnumerable<AdminResult>> ListAdmin()
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
            return await RepositoryContext.Admins
                        .Select(e => new AdminResult{
                            AdminId = e.AdminId,
                            AdminName = e.AdminName,
                            Email = e.Email,
                            LoginName = e.LoginName,
                            Password = e.Password,
                            AdminPhoto = e.AdminPhoto,
                            AdminLevelId = e.AdminLevelId,
                            AdminLevelName = e.AdminLevel!.AdminLevelName,
                        })
                        .OrderBy(s => s.AdminId).ToListAsync();
        }


        public bool IsExists(long id)
        {
            return RepositoryContext.Admins.Any(e => e.AdminId == id);
        }

       
    }

}