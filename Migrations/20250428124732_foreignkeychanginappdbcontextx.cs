using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class foreignkeychanginappdbcontextx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "StudentQuizAttemptId",
                table: "StudentAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentQuizAttemptId",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptId",
                principalTable: "StudentQuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
