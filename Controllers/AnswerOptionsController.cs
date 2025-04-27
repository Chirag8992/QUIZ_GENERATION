using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;

namespace Quizgeneration_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerOptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnswerOptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AnswerOptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerOption>>> GetAnswerOptions()
        {
            return await _context.AnswerOptions.ToListAsync();
        }

        // GET: api/AnswerOptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerOption>> GetAnswerOption(int id)
        {
            var answerOption = await _context.AnswerOptions.FindAsync(id);

            if (answerOption == null)
            {
                return NotFound();
            }

            return answerOption;
        }

        // GET: api/AnswerOptions/Question/5
        [HttpGet("Question/{questionId}")]
        public async Task<ActionResult<IEnumerable<AnswerOption>>> GetAnswerOptionsByQuestion(int questionId)
        {
            return await _context.AnswerOptions
                .Where(ao => ao.QuestionId == questionId)
                .ToListAsync();
        }

        // PUT: api/AnswerOptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnswerOption(int id, AnswerOption answerOption)
        {
            if (id != answerOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(answerOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerOptionExists(id))
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

        // POST: api/AnswerOptions
        [HttpPost]
        public async Task<ActionResult<AnswerOption>> PostAnswerOption(AnswerOption answerOption)
        {
            // Validate QuestionId exists
            var questionExists = await _context.Questions.AnyAsync(q => q.Id == answerOption.QuestionId);
            if (!questionExists)
            {
                return BadRequest(new { message = $"Question with ID {answerOption.QuestionId} does not exist." });
            }

            _context.AnswerOptions.Add(answerOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnswerOption", new { id = answerOption.Id }, answerOption);
        }

        // POST: api/AnswerOptions/Bulk
        [HttpPost("Bulk")]
        public async Task<ActionResult<IEnumerable<AnswerOption>>> PostBulkAnswerOptions(
            [FromBody] List<AnswerOption> options)
        {
            if (options == null || !options.Any())
            {
                return BadRequest(new { message = "No answer options provided" });
            }

            // Get the question ID from the first option
            int questionId = options.First().QuestionId;

            // Validate all options have the same QuestionId
            if (options.Any(o => o.QuestionId != questionId))
            {
                return BadRequest(new { message = "All options must belong to the same question" });
            }

            // Validate QuestionId exists
            var questionExists = await _context.Questions.AnyAsync(q => q.Id == questionId);
            if (!questionExists)
            {
                return BadRequest(new { message = $"Question with ID {questionId} does not exist." });
            }

            // Ensure only one option is marked as correct
            if (options.Count(o => o.IsCorrect) != 1)
            {
                return BadRequest(new { message = "Exactly one option must be marked as correct" });
            }

            _context.AnswerOptions.AddRange(options);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnswerOptionsByQuestion", new { questionId = questionId }, options);
        }

        // DELETE: api/AnswerOptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswerOption(int id)
        {
            var answerOption = await _context.AnswerOptions.FindAsync(id);
            if (answerOption == null)
            {
                return NotFound();
            }

            _context.AnswerOptions.Remove(answerOption);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnswerOptionExists(int id)
        {
            return _context.AnswerOptions.Any(e => e.Id == id);
        }
    }
}