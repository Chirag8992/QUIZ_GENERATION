using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Student.model;

namespace Quizgeneration_Project.model
{
    public class StudentQuizAttempted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int Score { get; set; } = 0;

        // Navigation properties
        [ForeignKey("QuizId")]
        public virtual Quiz Quiz { get; set; } = null!;

        [ForeignKey("StudentId")]
        public virtual StudentModel Student { get; set; } = null!;

        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}