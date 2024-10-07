using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentInfoSystemApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class userAndUserRoleSeedUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0bf72365-10fe-4547-809c-df63370c89ef", null, "Owner", "OWNER" },
                    { "6b5fb7b0-1fe8-4aba-85b7-87aea797b8c7", null, "Admin", "ADMİN" },
                    { "7e2ad1b2-3dc1-4f24-b119-b7f9306468bf", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e5645657-a36d-4b49-a129-1110c2ab71c6", 0, "9b6769b9-1135-44cd-84a6-7ea151abad5e", "owner@example.com", true, null, false, null, "OWNER@EXAMPLE.COM", "OWNER", "AQAAAAIAAYagAAAAENW51Mvapu8wvMgLZagxOseqKmYHbHwEtz3azIhDyqGg+tdfhkBysSPlk/sAZCbRTw==", null, false, "e277140e-c1af-4f5c-9036-d9b71168fdc1", false, "owner" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0bf72365-10fe-4547-809c-df63370c89ef", "e5645657-a36d-4b49-a129-1110c2ab71c6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b5fb7b0-1fe8-4aba-85b7-87aea797b8c7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e2ad1b2-3dc1-4f24-b119-b7f9306468bf");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0bf72365-10fe-4547-809c-df63370c89ef", "e5645657-a36d-4b49-a129-1110c2ab71c6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bf72365-10fe-4547-809c-df63370c89ef");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5645657-a36d-4b49-a129-1110c2ab71c6");
        }
    }
}
