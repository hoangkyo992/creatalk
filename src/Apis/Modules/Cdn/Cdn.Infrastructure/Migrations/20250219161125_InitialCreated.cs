using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdn.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreated : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "cdn");

        migrationBuilder.CreateTable(
            name: "FileExtension",
            schema: "cdn",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                MineType = table.Column<string>(type: "text", nullable: false),
                TypeId = table.Column<int>(type: "integer", nullable: false),
                IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FileExtension", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Folder",
            schema: "cdn",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                IsEmpty = table.Column<bool>(type: "boolean", nullable: false),
                StatusId = table.Column<int>(type: "integer", nullable: false),
                ParentId = table.Column<long>(type: "bigint", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Folder", x => x.Id);
                table.ForeignKey(
                    name: "FK_Folder_Folder_ParentId",
                    column: x => x.ParentId,
                    principalSchema: "cdn",
                    principalTable: "Folder",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Sequences",
            schema: "cdn",
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
            name: "File",
            schema: "cdn",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Extension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Content = table.Column<byte[]>(type: "bytea", nullable: false),
                StorageUrl = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                Size = table.Column<int>(type: "integer", nullable: false),
                StatusId = table.Column<int>(type: "integer", nullable: false),
                FolderId = table.Column<long>(type: "bigint", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_File", x => x.Id);
                table.ForeignKey(
                    name: "FK_File_Folder_FolderId",
                    column: x => x.FolderId,
                    principalSchema: "cdn",
                    principalTable: "Folder",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_File_FolderId",
            schema: "cdn",
            table: "File",
            column: "FolderId");

        migrationBuilder.CreateIndex(
            name: "IX_Folder_ParentId",
            schema: "cdn",
            table: "Folder",
            column: "ParentId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "File",
            schema: "cdn");

        migrationBuilder.DropTable(
            name: "FileExtension",
            schema: "cdn");

        migrationBuilder.DropTable(
            name: "Sequences",
            schema: "cdn");

        migrationBuilder.DropTable(
            name: "Folder",
            schema: "cdn");
    }
}