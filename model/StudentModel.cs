using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quizgeneration_Project.model;

namespace Student.model
{
    public class StudentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(255)]
        public string? Password { get; set; }

        [Required]
        public int Standard { get; set; }

        // Adding Role property to the student model
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Student";// "Student", "Author", or other roles

        // Navigation property
        public virtual ICollection<StudentQuizAttempted> QuizAttempts { get; set; } = new List<StudentQuizAttempted>();
    }
}
