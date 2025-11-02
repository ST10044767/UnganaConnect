using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnganaConnect.Migrations
{
    /// <inheritdoc />
    public partial class Forum2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Posts",
                table: "ForumCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Posts",
                table: "ForumCategories");
        }
    }
}
