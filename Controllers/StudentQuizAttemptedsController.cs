
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;

namespace Quizgeneration_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentQuizAttemptedsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentQuizAttemptedsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StudentQuizAttempteds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentQuizAttempted>>> GetStudentQuizAttempts()
        {
            return await _context.StudentQuizAttempts.ToListAsync();
        }

        // GET: api/StudentQuizAttempteds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentQuizAttempted>> GetStudentQuizAttempted(int id)
        {
            var studentQuizAttempted = await _context.StudentQuizAttempts.FindAsync(id);

            if (studentQuizAttempted == null)
            {
                return NotFound();
            }

            return studentQuizAttempted;
        }

        // PUT: api/StudentQuizAttempteds/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentQuizAttempted(int id, StudentQuizAttempted studentQuizAttempted)
        {
            if (id != studentQuizAttempted.Id)
            {
                return BadRequest();
            }

            _context.Entry(studentQuizAttempted).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentQuizAttemptedExists(id))
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

        // POST: api/StudentQuizAttempteds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentQuizAttempted>> PostStudentQuizAttempted(StudentQuizAttempted studentQuizAttempted)
        {
            _context.StudentQuizAttempts.Add(studentQuizAttempted);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentQuizAttempted", new { id = studentQuizAttempted.Id }, studentQuizAttempted);
        }

        // DELETE: api/StudentQuizAttempteds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentQuizAttempted(int id)
        {
            var studentQuizAttempted = await _context.StudentQuizAttempts.FindAsync(id);
            if (studentQuizAttempted == null)
            {
                return NotFound();
            }

            _context.StudentQuizAttempts.Remove(studentQuizAttempted);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentQuizAttemptedExists(int id)
        {
            return _context.StudentQuizAttempts.Any(e => e.Id == id);
        }
    }
}
