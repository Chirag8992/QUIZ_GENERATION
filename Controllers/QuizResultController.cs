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

        // POST: api/QuizResult
        [HttpPost]
        public async Task<ActionResult<QuizResult>> PostQuizResult([FromBody] QuizResultDto quizResult)
        {
            try
            {
                // Validate quiz and student exist
                var quiz = await _context.Quizzes.FindAsync(quizResult.QuizId);
                if (quiz == null)
                {
                    return BadRequest(new { message = $"Quiz with ID {quizResult.QuizId} does not exist." });
                }

                var student = await _context.student.FindAsync(quizResult.StudentId);
                if (student == null)
                {
                    return BadRequest(new { message = $"Student with ID {quizResult.StudentId} does not exist." });
                }

                // Create new quiz result
                var newQuizResult = new QuizResult
                {
                    QuizId = quizResult.QuizId,
                    StudentId = quizResult.StudentId,
                    Score = quizResult.Score,
                    CompletionDate = quizResult.CompletionDate
                };

                _context.QuizResults.Add(newQuizResult);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuizResult), new { id = newQuizResult.Id }, newQuizResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to save quiz result", error = ex.Message });
            }
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

        // GET: api/QuizResult/statistics/5
        [HttpGet("statistics/{quizId}")]
        public async Task<ActionResult<QuizStatisticsDto>> GetQuizStatistics(int quizId)
        {
            try
            {
                // Check if quiz exists
                var quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.AnswerOptions)
                    .FirstOrDefaultAsync(q => q.Id == quizId);

                if (quiz == null)
                {
                    return NotFound(new { message = $"Quiz with ID {quizId} does not exist." });
                }

                // Get all quiz results for this quiz
                var quizResults = await _context.QuizResults
                    .Where(qr => qr.QuizId == quizId)
                    .Include(qr => qr.Student)
                    .ToListAsync();

                // Get student attempts for this quiz
                var studentAttempts = await _context.StudentQuizAttempts
                    .Where(sqa => sqa.QuizId == quizId)
                    .Include(sqa => sqa.Student)
                    .Include(sqa => sqa.StudentAnswers)
                    .ToListAsync();

                // Prepare the student results list
                var studentResults = new List<StudentResultDto>();
                foreach (var result in quizResults)
                {
                    // Add student result - using quiz duration as we don't have actual time taken
                    studentResults.Add(new StudentResultDto
                    {
                        StudentId = result.StudentId,
                        StudentName = result.Student?.Name ?? "Unknown Student",
                        Score = result.Score,
                        TimeTaken = quiz.Duration * 60, // Convert minutes to seconds
                        SubmittedAt = result.CompletionDate
                    });
                }

                // If we don't have quiz results but have student attempts
                if (!quizResults.Any() && studentAttempts.Any())
                {
                    foreach (var attempt in studentAttempts)
                    {
                        // Add from attempts if not already added
                        if (!studentResults.Any(sr => sr.StudentId == attempt.StudentId))
                        {
                            studentResults.Add(new StudentResultDto
                            {
                                StudentId = attempt.StudentId,
                                StudentName = attempt.Student?.Name ?? "Unknown Student",
                                Score = attempt.Score,
                                TimeTaken = quiz.Duration * 60, // Convert minutes to seconds
                                SubmittedAt = DateTime.Now // Fallback to current time
                            });
                        }
                    }
                }

                // Calculate average score based on total possible marks
                int totalPossibleMarks = quiz.Questions.Sum(q => q.Marks);
                double averageScore = studentResults.Any() && totalPossibleMarks > 0 ?
                    studentResults.Average(sr => (double)sr.Score / totalPossibleMarks * 100) : 0;

                // Create question analysis
                var questionAnalysis = new List<QuestionAnalysisDto>();

                foreach (var question in quiz.Questions)
                {
                    // Find correct answer option (assuming the first option is correct for now)
                    // This would need to be adjusted based on your actual data model
                    var correctOption = question.AnswerOptions.FirstOrDefault();

                    if (correctOption != null)
                    {
                        // Get student answers for this question
                        var studentAnswers = await _context.StudentAnswers
                            .Where(sa => sa.QuestionId == question.Id)
                            .ToListAsync();

                        // Count correct answers (modify this logic based on how correctness is determined)
                        int correctAnswers = studentAnswers.Count(sa => sa.SelectedOptionsId == correctOption.Id);

                        // Add to question analysis
                        questionAnalysis.Add(new QuestionAnalysisDto
                        {
                            QuestionText = question.QuestionText ?? $"Question {question.Id}",
                            TotalAnswers = studentAnswers.Count,
                            CorrectAnswers = correctAnswers,
                            AverageTime = 0 // Not tracking time per question
                        });
                    }
                }

                // Create quiz statistics
                var statistics = new QuizStatisticsDto
                {
                    TotalAttempts = Math.Max(quizResults.Count, studentAttempts.Count),
                    AverageScore = averageScore,
                    AverageTime = quiz.Duration * 60, // Convert minutes to seconds
                    StudentResults = studentResults,
                    QuestionAnalysis = questionAnalysis
                };

                return statistics;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetQuizStatistics: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving quiz statistics.", error = ex.Message });
            }
        }
    }

    // Transfer DTOs
    public class QuizResultDto
    {
        [Required]
        public int QuizId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int Score { get; set; }

        public DateTime CompletionDate { get; set; } = DateTime.Now;
    }

    public class QuizStatisticsDto
    {
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public int AverageTime { get; set; } // In seconds
        public List<StudentResultDto> StudentResults { get; set; } = new List<StudentResultDto>();
        public List<QuestionAnalysisDto> QuestionAnalysis { get; set; } = new List<QuestionAnalysisDto>();
    }

    public class StudentResultDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TimeTaken { get; set; } // In seconds
        public DateTime SubmittedAt { get; set; }
    }

    public class QuestionAnalysisDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public int TotalAnswers { get; set; }
        public int CorrectAnswers { get; set; }
        public int AverageTime { get; set; } // In seconds
    }
}