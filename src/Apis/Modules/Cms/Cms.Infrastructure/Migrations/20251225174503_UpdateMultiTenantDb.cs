using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMultiTenantDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cms",
                table: "MessageProvider",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "EventPayload",
                schema: "cms",
                table: "AttendeeMessage",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                schema: "cms",
                table: "AttendeeMessage",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cms",
                table: "AttendeeMessage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserReceivedAt",
                schema: "cms",
                table: "AttendeeMessage",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "cms",
                table: "Attendee",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TicketZone",
                schema: "cms",
                table: "Attendee",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cms",
                table: "MessageProvider");

            migrationBuilder.DropColumn(
                name: "EventPayload",
                schema: "cms",
                table: "AttendeeMessage");

            migrationBuilder.DropColumn(
                name: "SentAt",
                schema: "cms",
                table: "AttendeeMessage");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cms",
                table: "AttendeeMessage");

            migrationBuilder.DropColumn(
                name: "UserReceivedAt",
                schema: "cms",
                table: "AttendeeMessage");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "cms",
                table: "Attendee");

            migrationBuilder.DropColumn(
                name: "TicketZone",
                schema: "cms",
                table: "Attendee");
        }
    }
}
