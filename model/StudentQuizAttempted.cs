using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizgeneration_Project.model
{
    public class StudentQuizAttempted
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int Score { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.Now;

        // New field for time tracking
        public int TimeSpent { get; set; } // Time spent in seconds

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        [ForeignKey("StudentId")]
        public Student.model.StudentModel Student { get; set; }

        public List<StudentAnswer> StudentAnswers { get; set; }
    }
}