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
    public class AdminController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public AdminController(IRepositoryWrapper RW)
        {
            _repositoryWrapper = RW;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminResult>>> GetAdmin()
        {
            var AdminItems = await _repositoryWrapper.Admin.ListAdmin();
            return Ok(AdminItems);
        }

        // GET: api/Admin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _repositoryWrapper.Admin.FindByIDAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return admin;
        }

        // PUT: api/Admin/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, AdminRequest adminRequest)
        {
            if (id != adminRequest.AdminId)
            {
                return BadRequest();
            }

            Admin? objAdmin;
            try
            {
                objAdmin = await _repositoryWrapper.Admin.FindByIDAsync(id);
                if (objAdmin == null)
                    throw new Exception("Invalid Admin ID");
                objAdmin.AdminName = adminRequest.AdminName;
                objAdmin.Email = adminRequest.Email;
                objAdmin.Password = adminRequest.Password;
                objAdmin.Inactive = false;
                objAdmin.LoginName = adminRequest.LoginName;

                await _repositoryWrapper.Admin.UpdateAsync(objAdmin);
                await _repositoryWrapper.EventLog.Update(objAdmin);

                if (adminRequest.AdminPhoto != null || adminRequest.AdminPhoto != "")
                {
                    FileService.DeleteFileNameOnly("AdminPhoto", id.ToString());
                    FileService.MoveTempFile("AdminPhoto", adminRequest.AdminId.ToString(), adminRequest.AdminPhoto);

                }

            }
            catch (DbUpdateConcurrencyException) when (!AdminExists(id))
            {

                {
                    return NotFound();
                }

            }
            return NoContent();
        }

        // POST: api/Admin
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(AdminRequest adminRequest)
        {
            int _minPasswordLength = 0;
            var localDate = DateOnly.FromDateTime(DateTime.Now).ToString("dd-MM-yyyy");
            var admin = new Admin
            {
                AdminId = adminRequest.AdminId,
                AdminName = adminRequest.AdminName,
                Password = adminRequest.Password,
                Email = adminRequest.Email,
                Inactive = false,
                AdminLevelId = adminRequest.AdminLevelId,
                AdminPhoto = adminRequest.AdminPhoto
            };
            var password = admin.Password;
            if (password.ToString().Length < _minPasswordLength)
            {
                throw new ValidationException("Invalid Password");

            }
            else
            {
                string salt = Util.SaltedHash.GenerateSalt();
                password = Util.SaltedHash.ComputeHash(salt, password.ToString());
                admin.Password = password;
                admin.Salt = salt;
                Validator.ValidateObject(admin, new ValidationContext(admin), true);
                await _repositoryWrapper.Admin.CreateAsync(admin);
                await _repositoryWrapper.EventLog.Insert(admin);
                // AppDomainUnloadedException = admin.AdminId;
                if (admin.AdminPhoto != null && admin.AdminPhoto != "")
                {
                    FileService.MoveTempFile("AdminPhoto", admin.AdminId.ToString(), admin.AdminPhoto);
                }
                // return new { data = AdminId }
                        // return CreatedAtAction(nameof(GetAdmin), new { id = admin.AdminId }, admin);
            }
            // Validator.ValidateObject(adminRequest, new ValidationContext(adminRequest), true); //server side validation by using

            await _repositoryWrapper.Admin.CreateAsync(admin, true);
            // int AdminId = admin.AdminId;

            return CreatedAtAction(nameof(GetAdmin), new { id = admin.AdminId }, admin);


        }

        // DELETE: api/Admin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _repositoryWrapper.Admin.FindByIDAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            FileService.DeleteFileNameOnly("AdminPhoto", id.ToString());

            await _repositoryWrapper.Admin.DeleteAsync(admin, true);
            await _repositoryWrapper.EventLog.Delete(admin);

            return NoContent();
        }


        [HttpPost("search/{term}")]
        public async Task<ActionResult<IEnumerable<Admin>>> SearchAdmin(string searchTerm)
        {
            var empList = await _repositoryWrapper.Admin.SearchAdmin(searchTerm);
            return Ok(empList);
        }


        private bool AdminExists(int id)
        {
            return _repositoryWrapper.Admin.IsExists(id);
        }
    }
}
