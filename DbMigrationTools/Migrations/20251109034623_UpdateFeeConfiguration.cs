using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeeConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VehicleDescription",
                table: "ParkingRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleNumber",
                table: "ParkingRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "FeeRateConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleDescription",
                table: "ParkingRegistrations");

            migrationBuilder.DropColumn(
                name: "VehicleNumber",
                table: "ParkingRegistrations");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "FeeRateConfigs");
        }
    }
}
