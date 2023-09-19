using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public EmployeeController(IRepositoryWrapper RW)
        {
            _repositoryWrapper = RW;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employeeItems =  await _repositoryWrapper.Employees.FindAllAsync();
            return Ok(employeeItems);
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _repositoryWrapper.Employees.FindByIDAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            Employee? objEmployee;
            try
            {
                objEmployee = await _repositoryWrapper.Employees.FindByIDAsync(id);
                if (objEmployee == null) 
                    throw new Exception("Invalid Employee ID");
                
                objEmployee.EmployeeName = employee.EmployeeName;
                
                await _repositoryWrapper.Hero.UpdateAsync(objEmployee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            await _repositoryWrapper.Employees.CreateAsync(employee, true);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var employee = await _repositoryWrapper.Employees.FindByIDAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            await _repositoryWrapper.Employees.DeleteAsync(employee, true);
            
            return NoContent();
        }

        
        [HttpPost("search/{term}")]
        public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployee(string term)
        {
            var empList = await _repositoryWrapper.Employees.SearchEmployee(term);
            return Ok(empList);           
        }


        private bool EmployeeExists(long id)
        {
            return _repositoryWrapper.Employees.IsExists(id);
        }
    }
}
