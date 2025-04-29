using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizgeneration_Project.model
{
    public class StudentAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int SelectedOptionsId { get; set; }

        [Required]
        public int StudentQuizAttemptedId { get; set; }  // This is the correct property name in your model

        [ForeignKey("StudentQuizAttemptedId")]
        public virtual StudentQuizAttempted StudentQuizAttempt { get; set; } = null!;
        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        [ForeignKey("SelectedOptionsId")]
        public virtual AnswerOption SelectedOption { get; set; } = null!;

        
    }
}