using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class standardupadte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "standard",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "standard",
                table: "Subjects");
        }
    }
}
