using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTimeApplicable",
                table: "FeeRateConfigs");

            migrationBuilder.AlterColumn<double>(
                name: "CurrentReading",
                table: "UtilityReadings",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<float>(
                name: "DefaultVATRate",
                table: "FeeTypes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<double>(
                name: "ConsumptionStart",
                table: "FeeTiers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "ConsumptionEnd",
                table: "FeeTiers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FeeRateConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "FeeNotices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FeeNotices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "PreviousReading",
                table: "FeeDetails",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "CurrentReading",
                table: "FeeDetails",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Consumption",
                table: "FeeDetails",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentReadingDate",
                table: "FeeDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossCost",
                table: "FeeDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreviousReadingDate",
                table: "FeeDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Proration",
                table: "FeeDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VATCost",
                table: "FeeDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "VATRate",
                table: "FeeDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Floor",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FeeDetailTier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TierOrder = table.Column<int>(type: "int", nullable: false),
                    ConsumptionStart = table.Column<double>(type: "float", nullable: false),
                    ConsumptionEnd = table.Column<double>(type: "float", nullable: false),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consumption = table.Column<double>(type: "float", nullable: false),
                    ConsumptionStartOriginal = table.Column<double>(type: "float", nullable: false),
                    ConsumptionEndOriginal = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeDetailTier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeDetailTier_FeeDetails_FeeDetailId",
                        column: x => x.FeeDetailId,
                        principalTable: "FeeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuantityRateConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityRateConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuantityRateConfig_FeeTypes_FeeTypeId",
                        column: x => x.FeeTypeId,
                        principalTable: "FeeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeeDetailTier_FeeDetailId",
                table: "FeeDetailTier",
                column: "FeeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_QuantityRateConfig_FeeTypeId",
                table: "QuantityRateConfig",
                column: "FeeTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeeDetailTier");

            migrationBuilder.DropTable(
                name: "QuantityRateConfig");

            migrationBuilder.DropColumn(
                name: "DefaultVATRate",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FeeRateConfigs");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "FeeNotices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FeeNotices");

            migrationBuilder.DropColumn(
                name: "CurrentReadingDate",
                table: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "GrossCost",
                table: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "PreviousReadingDate",
                table: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "Proration",
                table: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "VATCost",
                table: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "VATRate",
                table: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Apartments");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentReading",
                table: "UtilityReadings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "ConsumptionStart",
                table: "FeeTiers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "ConsumptionEnd",
                table: "FeeTiers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTimeApplicable",
                table: "FeeRateConfigs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReading",
                table: "FeeDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrentReading",
                table: "FeeDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Consumption",
                table: "FeeDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
