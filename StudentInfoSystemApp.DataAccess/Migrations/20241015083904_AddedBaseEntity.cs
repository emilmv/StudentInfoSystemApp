using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentInfoSystemApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Students",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Schedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Schedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Programs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Programs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Programs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Instructors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Instructors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Instructors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Departments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Attendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Attendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Attendances",
                type: "datetime2",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Attendances");

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
    }
}
