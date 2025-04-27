using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserConnections.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionEventsOutbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IpAddress = table.Column<string>(type: "varchar", maxLength: 45, nullable: false),
                    ConnectionTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionEventsOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserConnections",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IpAddress = table.Column<string>(type: "varchar", maxLength: 45, nullable: false),
                    LastConnectionUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnections", x => new { x.UserId, x.IpAddress });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionEventsOutbox_ProcessedAtUtc",
                table: "ConnectionEventsOutbox",
                column: "ProcessedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_IpAddress",
                table: "UserConnections",
                column: "IpAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionEventsOutbox");

            migrationBuilder.DropTable(
                name: "UserConnections");
        }
    }
}
