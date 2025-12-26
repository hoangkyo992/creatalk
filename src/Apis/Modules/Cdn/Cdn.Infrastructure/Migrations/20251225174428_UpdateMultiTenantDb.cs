using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMultiTenantDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cdn",
                table: "Folder",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cdn",
                table: "FileExtension",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cdn",
                table: "File",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cdn",
                table: "AlbumFile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cdn",
                table: "Album",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cdn",
                table: "Folder");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cdn",
                table: "FileExtension");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cdn",
                table: "File");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cdn",
                table: "AlbumFile");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cdn",
                table: "Album");
        }
    }
}
