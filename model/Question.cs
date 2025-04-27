using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizgeneration_Project.model
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public string? QuestionText { get; set; }

        [Required]
        public int Marks { get; set; }

        // Navigation properties
        [ForeignKey("QuizId")]
        public virtual Quiz? Quiz { get; set; }

        public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}