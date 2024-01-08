using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSS.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LevelOfWork",
                table: "Estimates",
                newName: "LevelOfWorks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LevelOfWorks",
                table: "Estimates",
                newName: "LevelOfWork");
        }
    }
}
