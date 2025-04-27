using Microsoft.EntityFrameworkCore;
using Quizgeneration_Project.model;
using Student.model;
using Teachers.model;

namespace Quizgeneration_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<StudentModel> student { get; set; }
        public DbSet<TeacherModel> Teacher { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<AdminModel> AdminModels { get; set; }
        public DbSet<StudentQuizAttempted> StudentQuizAttempts { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Unique constraints
            modelBuilder.Entity<TeacherModel>()
        .HasIndex(t => t.Name)
        .IsUnique();

            modelBuilder.Entity<TeacherModel>()
                .HasIndex(t => t.Email)
                .IsUnique();

            modelBuilder.Entity<StudentModel>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<StudentModel>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.Name)
                .IsUnique(false);

            modelBuilder.Entity<Subject>()
                .HasIndex(s => new { s.TeacherId, s.Name, s.standard })
                .IsUnique(true);

            // ✅ Define relationships (fixing delete conflicts)

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Teacher)
                .WithMany(t => t.Quizzes)
                .HasForeignKey(q => q.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnswerOption>()
                .HasOne(a => a.Question)
                .WithMany(q => q.AnswerOptions)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // ❌ REMOVE CASCADE DELETE FROM StudentAnswer
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);  // 🔥 Prevents cascade conflict

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.SelectedOption)
                .WithMany()
                .HasForeignKey(sa => sa.SelectedOptionsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentQuizAttempted>()
                .HasOne(sqa => sqa.Quiz)
                .WithMany()
                .HasForeignKey(sqa => sqa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentQuizAttempted>()
                .HasOne(sqa => sqa.Student)
                .WithMany()
                .HasForeignKey(sqa => sqa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentAnswer>()
                .HasOne<StudentQuizAttempted>()
                .WithMany(sqa => sqa.StudentAnswers)
                .HasForeignKey("StudentQuizAttemptedId")
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Quizgeneration_Project.model.AdminModel> AdminModel { get; set; } = default!;
    }
}
