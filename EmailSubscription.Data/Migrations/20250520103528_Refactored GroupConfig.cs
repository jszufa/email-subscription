using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSubscription.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredGroupConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");
        }
    }
}
