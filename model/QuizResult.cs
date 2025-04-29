using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizgeneration_Project.model
{
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

        public DateTime CompletionDate { get; set; }

        // New field for time tracking
        public int TimeSpent { get; set; } // Time spent in seconds

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        [ForeignKey("StudentId")]
        public Student.model.StudentModel Student { get; set; }
    }
}