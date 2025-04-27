using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;

namespace Quizgeneration_Project.Controllers
{
    public class AdminLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AdminModelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminModelsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminModel>>> GetAdminModel()
        {
            return await _context.AdminModel.ToListAsync();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginDto loginModel)
        {
            var user = _context.AdminModel
                .FirstOrDefault(x => x.Email == loginModel.Email && x.Password == loginModel.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new
            {
                message = "Login successful",
                token = "mock-token", // Replace with real JWT later
                authorId = user.Id,
                role = user.Role

            });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AdminModel>> GetAdminModel(int id)
        {
            var adminModel = await _context.AdminModel.FindAsync(id);

            if (adminModel == null)
            {
                return NotFound();
            }

            return adminModel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdminModel(int id, AdminModel adminModel)
        {
            if (id != adminModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(adminModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminModelExists(id))
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

        // POST: api/AdminModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AdminModel>> PostAdminModel(AdminModel adminModel)
        {
            _context.AdminModel.Add(adminModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdminModel", new { id = adminModel.Id }, adminModel);
        }

        // DELETE: api/AdminModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdminModel(int id)
        {
            var adminModel = await _context.AdminModel.FindAsync(id);
            if (adminModel == null)
            {
                return NotFound();
            }

            _context.AdminModel.Remove(adminModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdminModelExists(int id)
        {
            return _context.AdminModel.Any(e => e.Id == id);
        }
    }
}
