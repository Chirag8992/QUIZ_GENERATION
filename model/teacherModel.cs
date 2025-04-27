using System.ComponentModel.DataAnnotations;
using Quizgeneration_Project.model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teachers.model
{
    public class TeacherModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty; // Storing hashed password

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Teacher";

        // Navigation properties
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();

        // JWT Refresh Token
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Constructor to set default role
        
    }
}
