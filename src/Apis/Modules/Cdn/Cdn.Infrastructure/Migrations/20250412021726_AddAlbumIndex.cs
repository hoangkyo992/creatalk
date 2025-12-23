using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdn.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAlbumIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Index",
            schema: "cdn",
            table: "AlbumFile",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Index",
            schema: "cdn",
            table: "AlbumFile");
    }
}
