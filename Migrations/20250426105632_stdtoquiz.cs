using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class stdtoquiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subjects_TeacherId",
                table: "Subjects");

            migrationBuilder.AddColumn<int>(
                name: "standard",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TeacherId_Name_standard",
                table: "Subjects",
                columns: new[] { "TeacherId", "Name", "standard" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subjects_TeacherId_Name_standard",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "standard",
                table: "Quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TeacherId",
                table: "Subjects",
                column: "TeacherId");
        }
    }
}
