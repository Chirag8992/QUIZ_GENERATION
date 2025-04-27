using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePathss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Teacher",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teacher",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teacher",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "StudentQuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    StudentModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentQuizAttempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentQuizAttempts_student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentQuizAttempts_student_StudentModelId",
                        column: x => x.StudentModelId,
                        principalTable: "student",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOptionsId = table.Column<int>(type: "int", nullable: false),
                    StudentQuizAttemptedId = table.Column<int>(type: "int", nullable: false),
                    StudentQuizAttemptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_AnswerOptions_SelectedOptionsId",
                        column: x => x.SelectedOptionsId,
                        principalTable: "AnswerOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptId",
                        column: x => x.StudentQuizAttemptId,
                        principalTable: "StudentQuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_StudentQuizAttempts_StudentQuizAttemptedId",
                        column: x => x.StudentQuizAttemptedId,
                        principalTable: "StudentQuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_SelectedOptionsId",
                table: "StudentAnswers",
                column: "SelectedOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptedId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptedId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizAttemptId",
                table: "StudentAnswers",
                column: "StudentQuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizAttempts_QuizId",
                table: "StudentQuizAttempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizAttempts_StudentId",
                table: "StudentQuizAttempts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizAttempts_StudentModelId",
                table: "StudentQuizAttempts",
                column: "StudentModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "StudentQuizAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teacher",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teacher",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
