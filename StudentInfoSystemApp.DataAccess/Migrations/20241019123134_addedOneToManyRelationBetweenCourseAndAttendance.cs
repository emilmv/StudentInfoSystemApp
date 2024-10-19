using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentInfoSystemApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedOneToManyRelationBetweenCourseAndAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "60e106e0-c8f1-4b07-9d13-3515eef0094c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90cbd3b6-5bae-4953-aa4f-3c94e943ede6");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "ddb505f7-fda6-4a49-8b48-8da9aeef3e8c", "0ec064df-9a0c-4107-955b-7bfd9b5c594b" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ddb505f7-fda6-4a49-8b48-8da9aeef3e8c");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0ec064df-9a0c-4107-955b-7bfd9b5c594b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1698901d-9e9f-43ac-85d6-3a305fd4a6a4", null, "User", "USER" },
                    { "24ae2106-0cc7-47aa-bdb1-312cc443e36a", null, "Admin", "ADMIN" },
                    { "87368eaf-2067-4ddd-86e7-0e3207bc3e20", null, "Owner", "OWNER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0196dce8-109e-47e1-8b0e-d04f8ec59f4e", 0, "54333b15-523a-498d-841b-31dfd9030b0f", "owner@example.com", true, "ExampleName ExampleSurname", false, null, "OWNER@EXAMPLE.COM", "OWNER", "AQAAAAIAAYagAAAAEGwgwKLO+kUD31wl4vZRCn9ce4Z4KddzHYmbQ1+w2vDNSkRyUyG1kQ8uOh3/TY46gg==", null, false, "9224c701-8038-4b95-aa80-245b8a932041", false, "owner" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "87368eaf-2067-4ddd-86e7-0e3207bc3e20", "0196dce8-109e-47e1-8b0e-d04f8ec59f4e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1698901d-9e9f-43ac-85d6-3a305fd4a6a4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "24ae2106-0cc7-47aa-bdb1-312cc443e36a");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "87368eaf-2067-4ddd-86e7-0e3207bc3e20", "0196dce8-109e-47e1-8b0e-d04f8ec59f4e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87368eaf-2067-4ddd-86e7-0e3207bc3e20");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0196dce8-109e-47e1-8b0e-d04f8ec59f4e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "60e106e0-c8f1-4b07-9d13-3515eef0094c", null, "Admin", "ADMIN" },
                    { "90cbd3b6-5bae-4953-aa4f-3c94e943ede6", null, "User", "USER" },
                    { "ddb505f7-fda6-4a49-8b48-8da9aeef3e8c", null, "Owner", "OWNER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0ec064df-9a0c-4107-955b-7bfd9b5c594b", 0, "6aa96fff-d61f-4caa-97f4-4d292f9fabf1", "owner@example.com", true, "ExampleName ExampleSurname", false, null, "OWNER@EXAMPLE.COM", "OWNER", "AQAAAAIAAYagAAAAENmDX/5M9oCj9h04noyqW5MsnwOQTNIaseYmL/iCxgeXBG7POoLOF1/OYDQ+hr5sCw==", null, false, "d0a3e907-ac6d-45dd-be79-5fdf32a04828", false, "owner" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "ddb505f7-fda6-4a49-8b48-8da9aeef3e8c", "0ec064df-9a0c-4107-955b-7bfd9b5c594b" });
        }
    }
}
