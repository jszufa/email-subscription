using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailSubscription.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Name",
                table: "Users",
                columns: new[] { "Email", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Name",
                table: "Users");
        }
    }
}
