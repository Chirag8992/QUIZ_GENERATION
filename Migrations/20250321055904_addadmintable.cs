using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class addadmintable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Teacher",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "student",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "student");
        }
    }
}
