using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserConnections.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeIpAddressIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing index
            migrationBuilder.DropIndex(
                name: "IX_UserConnections_IpAddress",
                table: "UserConnections");

            // Create a btree index for exact matches (equality searches)
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_UserConnections_IpAddress_Exact"" 
                ON ""UserConnections"" USING btree (""IpAddress"");
            ");

            // Create an index optimized for prefix searches using text_pattern_ops operator class
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_UserConnections_IpAddress_Prefix"" 
                ON ""UserConnections"" USING btree (""IpAddress"" text_pattern_ops);
            ");

            // Add similar indexes to ConnectionEventsOutbox
            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_ConnectionEventsOutbox_IpAddress_Exact"" 
                ON ""ConnectionEventsOutbox"" USING btree (""IpAddress"");
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX ""IX_ConnectionEventsOutbox_IpAddress_Prefix"" 
                ON ""ConnectionEventsOutbox"" USING btree (""IpAddress"" text_pattern_ops);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the specialized indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_UserConnections_IpAddress_Exact"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_UserConnections_IpAddress_Prefix"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_ConnectionEventsOutbox_IpAddress_Exact"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_ConnectionEventsOutbox_IpAddress_Prefix"";");

            // Recreate the original index
            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_IpAddress",
                table: "UserConnections",
                column: "IpAddress");
        }
    }
}
