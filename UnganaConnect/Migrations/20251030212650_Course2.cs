using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnganaConnect.Migrations
{
    /// <inheritdoc />
    public partial class Course2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "QuizQuestions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "QuizQuestions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
