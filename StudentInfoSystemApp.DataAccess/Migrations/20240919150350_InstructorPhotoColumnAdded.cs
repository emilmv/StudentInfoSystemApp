using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentInfoSystemApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InstructorPhotoColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Instructors");
        }
    }
}
