
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Controllers.Quizgeneration_Project.Dto;
using Quizgeneration_Project.Data;
using Quizgeneration_Project.model;

namespace Quizgeneration_Project.Controllers
{
    namespace Quizgeneration_Project.Dto


    {

        public class StudentQuizSubmissionDto
        {
            [Required]
            public int QuizId { get; set; }

            [Required]
            public int StudentId { get; set; }

            [Required]
            public int Score { get; set; }

            // New field for time tracking
            public int TimeSpent { get; set; } // Time spent in seconds

            [Required]
            public List<QuestionAnswerDto> Answers { get; set; }
        }
        public class QuizSubmissionDto
        {
            public int QuizId { get; set; }
            public int StudentId { get; set; }
            public int Score { get; set; }
            public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();
        }

        public class QuestionAnswerDto
        {
            public int QuestionId { get; set; }
            public int SelectedOptionId { get; set; }
        }
    }

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

        // Add this to the StudentQuizAttemptedsController class
        [HttpPost("SubmitQuiz")]
public async Task<ActionResult<StudentQuizAttempted>> SubmitQuiz(QuizSubmissionDto submission)
{
    try
    {
        // Log the submission data
        Console.WriteLine($"Received quiz submission: QuizId={submission.QuizId}, StudentId={submission.StudentId}, Score={submission.Score}, Answers={submission.Answers?.Count ?? 0}");

        // Validate input
        if (submission.QuizId <= 0 || submission.StudentId <= 0)
        {
            return BadRequest("Invalid Quiz ID or Student ID");
        }

        // Check if the quiz exists
        var quiz = await _context.Quizzes.FindAsync(submission.QuizId);
        if (quiz == null)
        {
            return NotFound($"Quiz with ID {submission.QuizId} not found");
        }

        // Check if the student exists
        var student = await _context.student.FindAsync(submission.StudentId);
        if (student == null)
        {
            return NotFound($"Student with ID {submission.StudentId} not found");
        }

        // Create the quiz attempt record
        var quizAttempt = new StudentQuizAttempted
        {
            QuizId = submission.QuizId,
            StudentId = submission.StudentId,
            Score = submission.Score
        };

        // Add the quiz attempt to the database
        _context.StudentQuizAttempts.Add(quizAttempt);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Created quiz attempt with ID: {quizAttempt.Id}");

        // Now add all the student answers
        if (submission.Answers != null && submission.Answers.Any())
        {
            foreach (var answer in submission.Answers)
            {
                try
                {
                    // Validate question existence
                    var question = await _context.Questions.FindAsync(answer.QuestionId);
                    if (question == null)
                    {
                        Console.WriteLine($"Warning: Question with ID {answer.QuestionId} not found");
                        continue;
                    }

                    // Validate if the option exists for the question
                    var option = await _context.AnswerOptions
                        .Where(o => o.Id == answer.SelectedOptionId && o.QuestionId == answer.QuestionId)
                        .FirstOrDefaultAsync();

                    // If option not found, try to find by index (position) for the question
                    if (option == null)
                    {
                        Console.WriteLine($"Option with ID {answer.SelectedOptionId} not found for question {answer.QuestionId}. Attempting to find by index...");
                        
                        // Get all options for this question sorted by ID (assuming this is the order they were created)
                        var questionOptions = await _context.AnswerOptions
                            .Where(o => o.QuestionId == answer.QuestionId)
                            .OrderBy(o => o.Id)
                            .ToListAsync();
                        
                        // Find the option by index if possible (with 0-based index)
                        int optionIndex = answer.SelectedOptionId - 1; // Convert to 0-based index if needed
                        if (optionIndex >= 0 && optionIndex < questionOptions.Count())
                        {
                            option = questionOptions[optionIndex];
                            Console.WriteLine($"Found option by index position: {option.Id}");
                        }
                    }

                    // If we still couldn't find an option, log a warning and continue
                    if (option == null)
                    {
                        Console.WriteLine($"Warning: Could not find a valid option for QuestionId={answer.QuestionId}, SelectedOptionId={answer.SelectedOptionId}");
                        // Continue with the provided ID anyway as a fallback
                    }

                            var studentAnswer = new StudentAnswer
                            {
                                QuestionId = answer.QuestionId,
                                SelectedOptionsId = option?.Id ?? answer.SelectedOptionId,
                                StudentQuizAttemptedId = quizAttempt.Id  // Match your model property name (with 'ed')
                            };

                            _context.StudentAnswers.Add(studentAnswer);
                    Console.WriteLine($"Added answer: QuestionId={answer.QuestionId}, SelectedOptionId={studentAnswer.SelectedOptionsId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing answer for question {answer.QuestionId}: {ex.Message}");
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Saved all student answers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving student answers: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw; // Re-throw to be caught by the outer try-catch
            }
        }
        else
        {
            Console.WriteLine("No answers were provided in the submission");
        }

        return CreatedAtAction("GetStudentQuizAttempted", new { id = quizAttempt.Id }, quizAttempt);
    }
    catch (DbUpdateException dbEx)
    {
        Console.WriteLine($"Database update error: {dbEx.Message}");
        Console.WriteLine($"Inner exception: {dbEx.InnerException?.Message}");
        return StatusCode(500, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing quiz submission: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
        private bool StudentQuizAttemptedExists(int id)
        {
            return _context.StudentQuizAttempts.Any(e => e.Id == id);
        }
    }
}
