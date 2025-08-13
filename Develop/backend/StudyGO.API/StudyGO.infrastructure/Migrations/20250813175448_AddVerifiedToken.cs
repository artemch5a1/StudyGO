using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGO.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifiedToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerifiedToken",
                table: "UsersEntity",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedToken",
                table: "UsersEntity");
        }
    }
}
