using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;
using System.ComponentModel.DataAnnotations;
using Student.model;

namespace Quizgeneration_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizResultController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuizResultController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/QuizResult
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizResult>>> GetQuizResults()
        {
            return await _context.QuizResults.ToListAsync();
        }

        // GET: api/QuizResult/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizResult>> GetQuizResult(int id)
        {
            var quizResult = await _context.QuizResults.FindAsync(id);

            if (quizResult == null)
            {
                return NotFound();
            }

            return quizResult;
        }

        // GET: api/QuizResult/Student/5
        [HttpGet("Student/{studentId}")]
        public async Task<ActionResult<IEnumerable<QuizResult>>> GetStudentResults(int studentId)
        {
            return await _context.QuizResults
                .Where(qr => qr.StudentId == studentId)
                .ToListAsync();
        }

        public class CreateQuizResultDto
        {
            [Required]
            public int QuizId { get; set; }

            [Required]
            public int StudentId { get; set; }

            [Required]
            public int Score { get; set; }

            [Required]
            public DateTime CompletionDate { get; set; }
        }

        // POST: api/QuizResult
        [HttpPost]
        public async Task<ActionResult<QuizResult>> PostQuizResult(CreateQuizResultDto resultDto)
        {
            // Validate that the quiz exists
            var quizExists = await _context.Quizzes.AnyAsync(q => q.Id == resultDto.QuizId);
            if (!quizExists)
            {
                return BadRequest(new { message = $"Quiz with ID {resultDto.QuizId} does not exist." });
            }

            // Validate that the student exists
            var studentExists = await _context.student.AnyAsync(s => s.Id == resultDto.StudentId);
            if (!studentExists)
            {
                return BadRequest(new { message = $"Student with ID {resultDto.StudentId} does not exist." });
            }

            // Check if the student has already completed this quiz
            var existingResult = await _context.QuizResults
                .FirstOrDefaultAsync(qr => qr.QuizId == resultDto.QuizId && qr.StudentId == resultDto.StudentId);

            if (existingResult != null)
            {
                // Update existing result
                existingResult.Score = resultDto.Score;
                existingResult.CompletionDate = resultDto.CompletionDate;

                _context.Entry(existingResult).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(existingResult);
            }
            else
            {
                // Create new result
                var quizResult = new QuizResult
                {
                    QuizId = resultDto.QuizId,
                    StudentId = resultDto.StudentId,
                    Score = resultDto.Score,
                    CompletionDate = resultDto.CompletionDate
                };

                _context.QuizResults.Add(quizResult);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetQuizResult", new { id = quizResult.Id }, quizResult);
            }
        }

        // DELETE: api/QuizResult/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizResult(int id)
        {
            var quizResult = await _context.QuizResults.FindAsync(id);
            if (quizResult == null)
            {
                return NotFound();
            }

            _context.QuizResults.Remove(quizResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}