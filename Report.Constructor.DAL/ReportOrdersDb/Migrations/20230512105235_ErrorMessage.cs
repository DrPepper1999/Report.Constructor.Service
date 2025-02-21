using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Report.Constructor.DAL.ReportOrders.Migrations
{
    /// <inheritdoc />
    public partial class ErrorMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "ReportOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "ReportOrder");
        }
    }
}
