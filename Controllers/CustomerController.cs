using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Util;
using TodoApi.Repositories;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public CustomerController(IRepositoryWrapper RW)
        {
            _repositoryWrapper = RW;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResult>>> GetCustomer()
        {
            var customerItems =  await _repositoryWrapper.Customer.ListCustomer();
            return Ok(customerItems);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _repositoryWrapper.Customer.FindByIDAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerRequest customerRequest)
        {
            if (id != customerRequest.CustomerId)
            {
                return BadRequest();
            }

            Customer? objCustomer;
            try
            {   
                objCustomer = await _repositoryWrapper.Customer.FindByIDAsync(id);
                if (objCustomer == null) 
                    throw new Exception("Invalid Customer ID");
                objCustomer.CustomerName = customerRequest.CustomerName;
                objCustomer.CustomerAddress = customerRequest.CustomerAddress;
                objCustomer.RegisterDate = customerRequest.RegisterDate;
                objCustomer.CustomerPhoto = customerRequest.CustomerPhoto;

                await _repositoryWrapper.Customer.UpdateAsync(objCustomer);

                if(customerRequest.CustomerPhoto != null || customerRequest.CustomerPhoto != "")
                {
                     FileService.DeleteFileNameOnly("CustomerPhoto", id.ToString());
                    FileService.MoveTempFile("CustomerPhoto", customerRequest.CustomerId.ToString(), customerRequest.CustomerPhoto);

                }
                
            }
            catch (DbUpdateConcurrencyException) when  (!CustomerExists(id))
            {
                
                {
                    return NotFound();
                }
                
            }
            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerRequest customerRequest)
        {   
            var localDate = DateOnly.FromDateTime(DateTime.Now).ToString("dd-MM-yyyy");
            var customer = new Customer
            {
                CustomerId = customerRequest.CustomerId,
                CustomerName = customerRequest.CustomerName,
                RegisterDate = customerRequest.RegisterDate,
                CustomerAddress = customerRequest.CustomerAddress,
                CustomerTypeId = customerRequest.CustomerTypeId,
                CustomerPhoto = customerRequest.CustomerPhoto
            };
            Validator.ValidateObject(customerRequest, new ValidationContext(customerRequest), true); //server side validation by using

            await _repositoryWrapper.Customer.CreateAsync(customer, true);
           
            if(customer.CustomerPhoto != null && customer.CustomerPhoto !=""){
                FileService.MoveTempFile("CustomerPhoto",customer.CustomerId.ToString(), customer.CustomerPhoto);
            }
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _repositoryWrapper.Customer.FindByIDAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            FileService.DeleteFileNameOnly("CustomerPhoto", id.ToString());

            await _repositoryWrapper.Customer.DeleteAsync(customer, true);
            
            return NoContent();
        }

        
        [HttpPost("search/{term}")]
        public async Task<ActionResult<IEnumerable<Customer>>>  SearchCustomer(string searchTerm)
        {
            var empList = await _repositoryWrapper.Customer.SearchCustomer(searchTerm);
            return Ok(empList);           
        }


        private bool CustomerExists(int id)
        {
            return _repositoryWrapper.Customer.IsExists(id);
        }
    }
}
