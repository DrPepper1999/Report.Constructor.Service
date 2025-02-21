using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Report.Constructor.DAL.ReportOrders.Migrations
{
    /// <inheritdoc />
    public partial class AddReportFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "ReportOrder");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "ReportOrder");

            migrationBuilder.AddColumn<string>(
                name: "ReportFilter",
                table: "ReportOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportFilter",
                table: "ReportOrder");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "ReportOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                table: "ReportOrder",
                type: "datetime2",
                nullable: true);
        }
    }
}
