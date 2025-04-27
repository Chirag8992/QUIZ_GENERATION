using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;

namespace Quizgeneration_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAnswersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentAnswersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StudentAnswers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentAnswer>>> GetStudentAnswers()
        {
            return await _context.StudentAnswers.ToListAsync();
        }

        // GET: api/StudentAnswers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentAnswer>> GetStudentAnswer(int id)
        {
            var studentAnswer = await _context.StudentAnswers.FindAsync(id);

            if (studentAnswer == null)
            {
                return NotFound();
            }

            return studentAnswer;
        }

        // PUT: api/StudentAnswers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentAnswer(int id, StudentAnswer studentAnswer)
        {
            if (id != studentAnswer.Id)
            {
                return BadRequest();
            }

            _context.Entry(studentAnswer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentAnswerExists(id))
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

        // POST: api/StudentAnswers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentAnswer>> PostStudentAnswer(StudentAnswer studentAnswer)
        {
            _context.StudentAnswers.Add(studentAnswer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentAnswer", new { id = studentAnswer.Id }, studentAnswer);
        }

        // DELETE: api/StudentAnswers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAnswer(int id)
        {
            var studentAnswer = await _context.StudentAnswers.FindAsync(id);
            if (studentAnswer == null)
            {
                return NotFound();
            }

            _context.StudentAnswers.Remove(studentAnswer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentAnswerExists(int id)
        {
            return _context.StudentAnswers.Any(e => e.Id == id);
        }
    }
}
