using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGlobalRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMembers_Users_UserId",
                table: "BoardMembers");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_UserId",
                table: "BoardMembers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_UserId",
                table: "BoardMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMembers_Users_UserId",
                table: "BoardMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
