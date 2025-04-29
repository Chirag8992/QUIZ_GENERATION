using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class xyz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "StudentQuizAttemptId",
                table: "StudentAnswers",
                newName: "StudentQuizAttemptedId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_StudentQuizAttemptedId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptedId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptedId",
                principalTable: "StudentQuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptedId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "StudentQuizAttemptedId",
                table: "StudentAnswers",
                newName: "StudentQuizAttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptedId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_StudentQuizAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptId",
                principalTable: "StudentQuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
