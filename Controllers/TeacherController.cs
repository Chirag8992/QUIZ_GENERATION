using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quizgeneration_Project.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Teachers.model;
using Student.model;
using BC = BCrypt.Net.BCrypt;


//using Org.BouncyCastle.Crypto.Generators;

namespace Quizgeneration_Project.Controllers
{
    public class CreateTeacherDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
    }

    public class TokenModel
    {
        public string? RefreshToken { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration config;

        public TeacherController(AppDbContext context, IConfiguration config)
        {
            this.context = context;
            this.config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var user = await context.Teacher.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null || !BC.Verify(login.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await context.SaveChangesAsync();

            return Ok(new
            {
                token = accessToken,
                refreshToken,
                teacherId = user.Id,
                name = user.Name,
                role = user.Role
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            var user = await context.Teacher.FirstOrDefaultAsync(u => u.RefreshToken == tokenModel.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid refresh token" });

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherModel>>> GetTeachers()
        {
            return await context.Teacher.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherModel>> GetTeacher(int id)
        {
            var teacher = await context.Teacher.FindAsync(id);
            if (teacher == null)
                return NotFound(new { message = "Teacher not found" });
            return Ok(teacher);
        }


        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody] CreateTeacherDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("All fields are required");

            // Check if email already exists
            var exists = await context.Teacher.AnyAsync(t => t.Email == dto.Email);
            if (exists)
                return Conflict("Email already exists");

            // Hash the password (use a proper hashing method in production)
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var teacher = new TeacherModel
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = "Teacher",
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            };

            context.Teacher.Add(teacher);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await context.Teacher.FindAsync(id);
            if (teacher == null)
                return NotFound(new { message = "Teacher not found" });
            context.Teacher.Remove(teacher);
            await context.SaveChangesAsync();
            return Ok(new { message = "Deleted Successfully" });
        }

        private string GenerateJwtToken(TeacherModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
