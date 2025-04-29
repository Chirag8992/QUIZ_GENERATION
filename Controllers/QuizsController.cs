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
    public class QuizController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Quiz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .ToListAsync();
        }

        // GET: api/Quiz/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return quiz;
        }

        // GET: api/Quiz/Teacher/5
        [HttpGet("Teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzesByTeacher(int teacherId)
        {
            return await _context.Quizzes
                .Where(q => q.TeacherId == teacherId)
                .ToListAsync();
        }

        // PUT: api/Quiz/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuiz(int id, Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return BadRequest();
            }

            _context.Entry(quiz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizExists(id))
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

        public class CreateQuizDto
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Subject { get; set; }

            [Required]
            public int Standard { get; set; }

            [Required]
            public int TeacherId { get; set; }

            [Required]
            public int Duration { get; set; } // in minutes

            [Required]
            public DateTime StartTime { get; set; }

            public int TotalMarks { get; set; }
        }

        // POST: api/Quiz
        [HttpPost]
        public async Task<ActionResult<Quiz>> PostQuiz(CreateQuizDto quizDto)
        {
            // Validate TeacherId exists
            var teacherExists = await _context.Teacher.AnyAsync(t => t.Id == quizDto.TeacherId);
            if (!teacherExists)
            {
                return BadRequest(new { message = $"Teacher with ID {quizDto.TeacherId} does not exist." });
            }

            // Create new quiz
            var quiz = new Quiz
            {
                Title = quizDto.Title,
                Subject = quizDto.Subject,
                TeacherId = quizDto.TeacherId,
                Duration = quizDto.Duration,
                StartTime = quizDto.StartTime,
                DateTime = DateTime.Now // Current date/time for creation timestamp
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuiz", new { id = quiz.Id }, quiz);
        }

        public class QuizWithQuestionsDto
        {
            [Required]
            public CreateQuizDto Quiz { get; set; }

            [Required]
            public List<QuestionWithOptionsDto> Questions { get; set; }

            public class QuestionWithOptionsDto
            {
                [Required]
                public string QuestionText { get; set; }

                [Required]
                public int Marks { get; set; }

                [Required]
                public List<AnswerOptionDto> Options { get; set; }

                [Required]
                public int CorrectOptionIndex { get; set; }

                [Required]
                public int standard { get; set; }
            }

            public class AnswerOptionDto
            {
                [Required]
                public string OptionText { get; set; }
            }
        }

        // GET: api/Quiz/{quizId}/Details
        [HttpGet("{quizId}/Details")]
        public async Task<IActionResult> GetQuizDetails(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            // Explicitly load questions if they're not loading
            if (quiz.Questions == null || !quiz.Questions.Any())
            {
                await _context.Entry(quiz)
                    .Collection(q => q.Questions)
                    .LoadAsync();

                foreach (var question in quiz.Questions)
                {
                    await _context.Entry(question)
                        .Collection(q => q.AnswerOptions)
                        .LoadAsync();
                }
            }

            var result = new
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Subject = quiz.Subject,
                Standard = quiz.standard,
                Questions = quiz.Questions.Select(q => new
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Marks = q.Marks,
                    Options = q.AnswerOptions.Select(o => new
                    {
                        Id = o.Id,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/Quiz/WithQuestions
        [HttpPost("WithQuestions")]
        public async Task<ActionResult<Quiz>> PostQuizWithQuestions(QuizWithQuestionsDto quizDto)
        {
            // Validate TeacherId exists
            var teacherExists = await _context.Teacher.AnyAsync(t => t.Id == quizDto.Quiz.TeacherId);
            if (!teacherExists)
            {
                return BadRequest(new { message = $"Teacher with ID {quizDto.Quiz.TeacherId} does not exist." });
            }

            // Start transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create quiz
                var quiz = new Quiz
                {
                    Title = quizDto.Quiz.Title,
                    Subject = quizDto.Quiz.Subject,
                    TeacherId = quizDto.Quiz.TeacherId,
                    Duration = quizDto.Quiz.Duration,
                    StartTime = quizDto.Quiz.StartTime,
                    DateTime = DateTime.Now,
                    standard = quizDto.Quiz.Standard
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                // Add questions and options
                foreach (var questionDto in quizDto.Questions)
                {
                    var question = new Question
                    {
                        QuizId = quiz.Id,
                        QuestionText = questionDto.QuestionText,
                        Marks = questionDto.Marks
                    };

                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();

                    // Add options for this question
                    for (int i = 0; i < questionDto.Options.Count; i++)
                    {
                        var option = new AnswerOption
                        {
                            QuestionId = question.Id,
                            OptionText = questionDto.Options[i].OptionText,
                            IsCorrect = i == questionDto.CorrectOptionIndex
                        };

                        _context.AnswerOptions.Add(option);
                    }

                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                // Return the created quiz with questions and options
                var createdQuiz = await _context.Quizzes
                    .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.Id == quiz.Id);

                return CreatedAtAction("GetQuiz", new { id = quiz.Id }, createdQuiz);
            }
            catch (Exception ex)
            {
                // Roll back transaction in case of any exception
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Failed to create quiz with questions", error = ex.Message });
            }
        }

        // DELETE: api/Quiz/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Student/{studentId}")]
        public async Task<ActionResult<dynamic>> GetStudentQuizzes(int studentId, [FromQuery] int? standard)
        {
            try
            {
                // Get student's standard if not provided
                if (!standard.HasValue)
                {
                    var student = await _context.student.FindAsync(studentId);
                    if (student != null)
                    {
                        standard = student.Standard;
                    }
                }

                var now = DateTime.UtcNow;

                // Get all quizzes (filter by standard if provided)
                var allQuizzes = await _context.Quizzes
                    .Where(q => !standard.HasValue || q.standard == standard.Value)
                    .Include(q => q.Questions)
                    .ToListAsync();

                // Get student's completed quiz IDs
                var completedQuizResults = await _context.QuizResults
                    .Where(qr => qr.StudentId == studentId)
                    .ToListAsync();

                var completedQuizIds = completedQuizResults.Select(qr => qr.QuizId).ToList();

                var upcomingQuizzes = allQuizzes
                    .Where(q => !completedQuizIds.Contains(q.Id))
                    .Select(q => new
                    {
                        q.Id,
                        q.Title,
                        q.Subject,
                        q.StartTime,
                        q.Duration,
                        q.standard,
                        QuestionCount = q.Questions.Count
                    })
                    .ToList();

                var completedQuizzes = await _context.QuizResults
                    .Where(qr => qr.StudentId == studentId)
                    .Include(qr => qr.Quiz)
                    .Select(qr => new
                    {
                        Quiz = new
                        {
                            qr.Quiz.Id,
                            qr.Quiz.Title,
                            qr.Quiz.Subject,
                            qr.Quiz.standard,
                            qr.Quiz.StartTime
                        },
                        qr.Score,
                        CompletedDate = qr.CompletionDate,
                        TotalQuestions = qr.Quiz.Questions.Count,
                        Percentage = (qr.Score * 100) / (qr.Quiz.Questions.Count > 0 ? qr.Quiz.Questions.Count : 1)
                    })
                    .ToListAsync();

                return Ok(new
                {
                    upcomingQuizzes,
                    completedQuizzes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.Id == id);
        }
    }
}