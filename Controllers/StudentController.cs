using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quizgeneration_Project.Data;
using Student.model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Quizgeneration_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase

    {
        private readonly AppDbContext context;
        private readonly IConfiguration _config; 
        private readonly PasswordHasher<StudentModel> _passwordHasher;

        public StudentController(AppDbContext context, IConfiguration config)
        {
            this.context = context;
            _config = config;
            _passwordHasher = new PasswordHasher<StudentModel>(); 
            Console.WriteLine($"JWT Key: {_config["Jwt:Key"]}");
            Console.WriteLine($"JWT Issuer: {_config["Jwt:Issuer"]}");
            Console.WriteLine($"JWT Audience: {_config["Jwt:Audience"]}");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentModel>>> GetStudents()
        {
            return await context.student.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentModel>> GetStudent(int id)
        {
            var student = await context.student.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(student);
        }

        [HttpGet("standards")]
        public IActionResult GetStandards()
        {
            var standards = new List<string>
        {
            "1st Standard", "2nd Standard", "3rd Standard", "4th Standard",
            "5th Standard", "6th Standard", "7th Standard", "8th Standard",
            "9th Standard", "10th Standard", "11th Standard", "12th Standard"
        };

            return Ok(standards);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var student = await context.student.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Teacher not found" });

            context.student.Remove(student);
            await context.SaveChangesAsync();
            return Ok(new { message = "Deleted Successfully" });
        }

        [HttpPost("register")]
        public async Task<ActionResult<StudentModel>> Register([FromBody] StudentModel student)
        {
            if (student == null)
                return BadRequest(new { message = "Invalid data received." });

            student.Password = _passwordHasher.HashPassword(student, student.Password);

            context.student.Add(student);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Student.model.LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null)
                    return BadRequest(new { message = "Invalid request format" });

                Console.WriteLine($"Received login request for email: {loginRequest.Email}");

                if (context == null)
                {
                    Console.WriteLine("Database context is null");
                    return StatusCode(500, new { message = "Database context not available" });
                }

                if (context.student == null)
                {
                    Console.WriteLine("Student DbSet is null");
                    return StatusCode(500, new { message = "Student DbSet not available" });
                }

                var student = await context.student.FirstOrDefaultAsync(s => s.Email == loginRequest.Email);

                if (student == null)
                {
                    Console.WriteLine($"No student found with email: {loginRequest.Email}");
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                Console.WriteLine($"Found student: {student.Id}, {student.Name}");

                var result = _passwordHasher.VerifyHashedPassword(student, student.Password, loginRequest.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    Console.WriteLine("Password verification failed");
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                Console.WriteLine("Password verified successfully");

                if (string.IsNullOrEmpty(_config["Jwt:Key"]) ||
                    string.IsNullOrEmpty(_config["Jwt:Issuer"]) ||
                    string.IsNullOrEmpty(_config["Jwt:Audience"]))
                {
                    Console.WriteLine("JWT configuration is missing");
                    return StatusCode(500, new { message = "JWT configuration is missing" });
                }

                var token = GenerateJwtToken(student);

                Console.WriteLine("JWT token generated successfully");

                return Ok(new { token, studentId = student.Id });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }

                return StatusCode(500, new
                {
                    message = "An error occurred during login",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        //[HttpGet("test-jwt")]
        //public IActionResult TestJwt()
        //{
        //    try
        //    {
        //        var dummyStudent = new StudentModel
        //        {
        //            Id = 1,
        //            Email = "test@example.com",
        //            Name = "Test User",
        //            Password = "dummy",
        //            Standard = 10
        //        };

        //        var token = GenerateJwtToken(dummyStudent);
        //        return Ok(new { token });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            message = "JWT generation error",
        //            error = ex.Message,
        //            innerError = ex.InnerException?.Message
        //        });
        //    }
        //}

        private string GenerateJwtToken(StudentModel student)
        {
            try
            {
                string? configKey = _config["Jwt:Key"];
                Console.WriteLine($"Using JWT key from config: {configKey}");

                byte[] keyBytes;
                using (var sha = SHA256.Create())
                {
                    keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(configKey));
                }

                var key = new SymmetricSecurityKey(keyBytes);
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, student.Email),
            new Claim("studentId", student.Id.ToString()),
            new Claim("Role", student.Role) // Ensure Role is passed here from the model
        };

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating JWT token: {ex.Message}");
                throw;
            }
        }

    }


}

