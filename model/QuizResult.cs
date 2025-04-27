
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Student.model;
namespace Quizgeneration_Project.model
{
    // Add this to the models folder
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public DateTime CompletionDate { get; set; }

        // Navigation properties
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        [ForeignKey("StudentId")]
        public StudentModel Student { get; set; }
    }
}
