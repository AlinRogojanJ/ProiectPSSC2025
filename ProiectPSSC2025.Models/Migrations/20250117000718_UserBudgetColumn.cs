using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectPSSC2025.Models.Migrations
{
    /// <inheritdoc />
    public partial class UserBudgetColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Budget",
                table: "Users",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Users");
        }
    }
}
