using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TgBotFrame.Example.Migrations
{
    /// <inheritdoc />
    public partial class BansAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bans_UserId",
                table: "Bans");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "RoleMembers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoleMembers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Bans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Until",
                table: "Bans",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Bans",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMembers_UserId",
                table: "RoleMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId_Until",
                table: "Bans",
                columns: new[] { "UserId", "Until" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bans_Users_UserId",
                table: "Bans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMembers_Users_UserId",
                table: "RoleMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bans_Users_UserId",
                table: "Bans");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMembers_Users_UserId",
                table: "RoleMembers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_RoleMembers_UserId",
                table: "RoleMembers");

            migrationBuilder.DropIndex(
                name: "IX_Bans_UserId_Until",
                table: "Bans");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "RoleMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "RoleMembers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Until",
                table: "Bans",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Bans",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Bans",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "Bans",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId",
                table: "Bans",
                column: "UserId");
        }
    }
}
