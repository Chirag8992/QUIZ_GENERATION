using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Teachers.model;

namespace Quizgeneration_Project.model
{
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        public int standard {  get; set; }

        // Navigation properties
        [ForeignKey("TeacherId")]
        public virtual TeacherModel? Teacher { get; set; }
    }
}