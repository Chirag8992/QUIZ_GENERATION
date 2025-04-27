using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;

namespace Quizgeneration_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        public class SubjectAssignmentDto
        {
            public int TeacherId { get; set; }
            public string Subject { get; set; } = string.Empty;
            public List<string> Standards { get; set; } = new();
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> PostMultipleSubjects([FromBody] SubjectAssignmentDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Subject) || dto.Standards == null || !dto.Standards.Any())
                return BadRequest(new { message = "Invalid subject assignment data." });

            var subjects = dto.Standards.Select(std => new Subject
            {
                TeacherId = dto.TeacherId,
                Name = dto.Subject,
                standard = int.Parse(std)
            }).ToList();

            _context.Subjects.AddRange(subjects);
            await _context.SaveChangesAsync();

            return Ok(subjects);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            return subject;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, Subject subject)
        {
            if (id != subject.Id)
                return BadRequest();

            _context.Entry(subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> PostSubject(Subject subject)
        {
            // Check if a subject with the same TeacherId, Name, and standard already exists
            var existingSubject = await _context.Subjects
                .FirstOrDefaultAsync(s =>
                    s.TeacherId == subject.TeacherId &&
                    s.Name == subject.Name &&
                    s.standard == subject.standard);

            if (existingSubject != null)
            {
                // Subject already exists, return it instead of creating a duplicate
                return CreatedAtAction("GetSubject", new { id = existingSubject.Id }, existingSubject);
            }

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubject", new { id = subject.Id }, subject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}