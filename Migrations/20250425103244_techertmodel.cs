using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizgeneration_Project.Migrations
{
    /// <inheritdoc />
    public partial class techertmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Teacher",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Teacher",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Teacher");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Teacher",
                newName: "Password");
        }
    }
}
