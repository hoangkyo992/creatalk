using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMultiTenantDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "auth",
                table: "LogRelatedEntity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "auth",
                table: "LogEntity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "auth",
                table: "LogActivity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "auth",
                table: "LogRelatedEntity");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "auth",
                table: "LogEntity");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "auth",
                table: "LogActivity");
        }
    }
}
