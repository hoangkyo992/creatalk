using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateUserRoles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_User_Role_RoleId",
            schema: "auth",
            table: "User");

        migrationBuilder.AlterColumn<long>(
            name: "RoleId",
            schema: "auth",
            table: "User",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.CreateTable(
            name: "UserRole",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                RoleId = table.Column<long>(type: "bigint", nullable: false),
                RevokedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRole", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserRole_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "auth",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserRole_User_UserId",
                    column: x => x.UserId,
                    principalSchema: "auth",
                    principalTable: "User",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserRole_RoleId",
            schema: "auth",
            table: "UserRole",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_UserRole_UserId",
            schema: "auth",
            table: "UserRole",
            column: "UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_User_Role_RoleId",
            schema: "auth",
            table: "User",
            column: "RoleId",
            principalSchema: "auth",
            principalTable: "Role",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_User_Role_RoleId",
            schema: "auth",
            table: "User");

        migrationBuilder.DropTable(
            name: "UserRole",
            schema: "auth");

        migrationBuilder.AlterColumn<long>(
            name: "RoleId",
            schema: "auth",
            table: "User",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_User_Role_RoleId",
            schema: "auth",
            table: "User",
            column: "RoleId",
            principalSchema: "auth",
            principalTable: "Role",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
