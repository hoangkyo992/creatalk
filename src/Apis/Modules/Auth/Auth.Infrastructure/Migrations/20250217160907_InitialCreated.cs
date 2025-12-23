using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreated : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "auth");

        migrationBuilder.CreateTable(
            name: "LogActivity",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                MethodName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Action = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                RequestId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LogActivity", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LogEntity",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                ActivityId = table.Column<long>(type: "bigint", nullable: false),
                EntityName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Pk = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                CRUD = table.Column<char>(type: "character(1)", nullable: false),
                Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                OldValue = table.Column<string>(type: "text", nullable: false),
                NewValue = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LogEntity", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LogRelatedEntity",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                ActivityId = table.Column<long>(type: "bigint", nullable: false),
                EntityName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Pk = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LogRelatedEntity", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Role",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Role", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Sequences",
            schema: "auth",
            columns: table => new
            {
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Value = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sequences", x => x.Name);
            });

        migrationBuilder.CreateTable(
            name: "Setting",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Value = table.Column<string>(type: "text", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Setting", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserSession",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                RefreshToken = table.Column<string>(type: "text", nullable: false),
                StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                EndBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Navigator = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserSession", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RoleFeature",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                RoleId = table.Column<long>(type: "bigint", nullable: false),
                Feature = table.Column<string>(type: "text", nullable: false),
                Action = table.Column<string>(type: "text", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleFeature", x => x.Id);
                table.ForeignKey(
                    name: "FK_RoleFeature_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "auth",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "User",
            schema: "auth",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                DisplayName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                RoleId = table.Column<long>(type: "bigint", nullable: false),
                Password = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                StatusId = table.Column<int>(type: "integer", nullable: false),
                Avatar = table.Column<string>(type: "text", nullable: true),
                PasswordChanged = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_User", x => x.Id);
                table.ForeignKey(
                    name: "FK_User_Role_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "auth",
                    principalTable: "Role",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_RoleFeature_RoleId",
            schema: "auth",
            table: "RoleFeature",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_User_RoleId",
            schema: "auth",
            table: "User",
            column: "RoleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LogActivity",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "LogEntity",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "LogRelatedEntity",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "RoleFeature",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "Sequences",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "Setting",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "User",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "UserSession",
            schema: "auth");

        migrationBuilder.DropTable(
            name: "Role",
            schema: "auth");
    }
}
