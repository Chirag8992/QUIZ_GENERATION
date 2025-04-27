using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teachers.model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.Data;

namespace Quizgeneration_Project.model
{
    public class Quiz
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Title { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Subject { get; set; }

        [Required]
        public int Duration { get; set; } // in minutes

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public int standard { get; set; }

        // Navigation properties
        [ForeignKey("TeacherId")]
        public virtual TeacherModel? Teacher { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}

