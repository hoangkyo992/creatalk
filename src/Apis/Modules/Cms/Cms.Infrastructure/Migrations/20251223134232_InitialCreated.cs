using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cms");

            migrationBuilder.CreateTable(
                name: "Attendee",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "character varying(201)", maxLength: 201, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TicketNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TicketId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageProvider",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Settings = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_MessageProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sequences",
                schema: "cms",
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
                name: "AttendeeMessage",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    RequestPayload = table.Column<string>(type: "text", nullable: false),
                    ResponsePayload = table.Column<string>(type: "text", nullable: false),
                    MessageId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AttendeeId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendeeMessage_Attendee_AttendeeId",
                        column: x => x.AttendeeId,
                        principalSchema: "cms",
                        principalTable: "Attendee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendeeMessage_MessageProvider_ProviderId",
                        column: x => x.ProviderId,
                        principalSchema: "cms",
                        principalTable: "MessageProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeMessage_AttendeeId",
                schema: "cms",
                table: "AttendeeMessage",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeMessage_ProviderId",
                schema: "cms",
                table: "AttendeeMessage",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendeeMessage",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Sequences",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Attendee",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "MessageProvider",
                schema: "cms");
        }
    }
}
