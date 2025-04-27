using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizgeneration_Project.model
{
    public class AnswerOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        [StringLength(255)]
        public string? OptionText { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        // Navigation properties
        [ForeignKey("QuestionId")]
        public virtual Question? Question { get; set; }
    }
}