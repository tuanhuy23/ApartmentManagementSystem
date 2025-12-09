using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "FeeTiers");

            migrationBuilder.AddColumn<float>(
                name: "OtherRate",
                table: "FeeRateConfigs",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherRate",
                table: "FeeRateConfigs");

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "FeeTiers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
