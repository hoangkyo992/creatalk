using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdn.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFileExtraProperties : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Properties",
            schema: "cdn",
            table: "File",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "TypeId",
            schema: "cdn",
            table: "File",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Properties",
            schema: "cdn",
            table: "File");

        migrationBuilder.DropColumn(
            name: "TypeId",
            schema: "cdn",
            table: "File");
    }
}
