using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentInfoSystemApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedFullNameofOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fbac687-dcfa-439f-bb09-344ae701105e", null, "User", "USER" },
                    { "a90bb2e5-6095-4e28-aa6c-7b9a0faecf32", null, "Admin", "ADMIN" },
                    { "dfd5894b-322f-4470-8b99-52726d76b22e", null, "Owner", "OWNER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "08fadacd-087c-4079-aa84-325ac9eff01a", 0, "ae79fb2a-7d26-4576-bbd4-89b6aaa1d9b1", "owner@example.com", true, "ExampleName ExampleSurname", false, null, "OWNER@EXAMPLE.COM", "OWNER", "AQAAAAIAAYagAAAAEErwtTR99PhVI2/a0Pw8ft+1PmY/FavPZ+PyAR92bAIVXE2A3KFF07Wc2nu+/fqy4w==", null, false, "69e9aef5-0f28-4c1c-8332-b5a23d1c0eb4", false, "owner" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "dfd5894b-322f-4470-8b99-52726d76b22e", "08fadacd-087c-4079-aa84-325ac9eff01a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fbac687-dcfa-439f-bb09-344ae701105e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a90bb2e5-6095-4e28-aa6c-7b9a0faecf32");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "dfd5894b-322f-4470-8b99-52726d76b22e", "08fadacd-087c-4079-aa84-325ac9eff01a" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dfd5894b-322f-4470-8b99-52726d76b22e");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "08fadacd-087c-4079-aa84-325ac9eff01a");

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
    }
}
