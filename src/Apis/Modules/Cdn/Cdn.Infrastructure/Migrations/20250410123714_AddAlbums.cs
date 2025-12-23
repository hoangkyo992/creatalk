using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdn.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAlbums : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Album",
            schema: "cdn",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                IsEmpty = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Album", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AlbumFile",
            schema: "cdn",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                AlbumId = table.Column<long>(type: "bigint", nullable: false),
                FileId = table.Column<long>(type: "bigint", nullable: false),
                Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                RowVersion = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AlbumFile", x => x.Id);
                table.ForeignKey(
                    name: "FK_AlbumFile_Album_AlbumId",
                    column: x => x.AlbumId,
                    principalSchema: "cdn",
                    principalTable: "Album",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AlbumFile_File_FileId",
                    column: x => x.FileId,
                    principalSchema: "cdn",
                    principalTable: "File",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AlbumFile_AlbumId",
            schema: "cdn",
            table: "AlbumFile",
            column: "AlbumId");

        migrationBuilder.CreateIndex(
            name: "IX_AlbumFile_FileId",
            schema: "cdn",
            table: "AlbumFile",
            column: "FileId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AlbumFile",
            schema: "cdn");

        migrationBuilder.DropTable(
            name: "Album",
            schema: "cdn");
    }
}
