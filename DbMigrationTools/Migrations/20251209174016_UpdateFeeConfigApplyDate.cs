using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeeConfigApplyDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "QuantityRateConfig");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "QuantityRateConfig",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
