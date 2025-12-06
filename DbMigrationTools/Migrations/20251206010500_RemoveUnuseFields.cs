using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnuseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Building",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Buildings",
                table: "ApartmentBuildings");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ApartmentBuildings");

            migrationBuilder.RenameColumn(
                name: "UserIds",
                table: "Announcements",
                newName: "ApartmentIds");

            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "Requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "QuantityRateConfig",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FileAttachments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FileAttachments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "FeeRateConfigs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "QuantityRateConfig");

            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "FeeRateConfigs");

            migrationBuilder.RenameColumn(
                name: "ApartmentIds",
                table: "Announcements",
                newName: "UserIds");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FileAttachments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FileAttachments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "Apartments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Buildings",
                table: "ApartmentBuildings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ApartmentBuildings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
