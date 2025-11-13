using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApartmentBuildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentBuildingImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Buildings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentBuildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAll = table.Column<bool>(type: "bit", nullable: false),
                    UserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Announcements_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<double>(type: "float", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apartments_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingCycleSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosingDayOfMonth = table.Column<int>(type: "int", nullable: false),
                    PaymentDueDate = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingCycleSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingCycleSettings_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalculationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsVATApplicable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DefaultVATRate = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeTypes_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrithDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Residents_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserReadStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReadStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReadStatus_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeNotices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillingCycle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeNotices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeNotices_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParkingRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingRegistrations_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeRateConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VATRate = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeRateConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeRateConfigs_FeeTypes_FeeTypeId",
                        column: x => x.FeeTypeId,
                        principalTable: "FeeTypes",
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

            migrationBuilder.CreateTable(
                name: "UtilityReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentReading = table.Column<double>(type: "float", nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilityReadings_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilityReadings_FeeTypes_FeeTypeId",
                        column: x => x.FeeTypeId,
                        principalTable: "FeeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedback_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApartmentResidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentResidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApartmentResidents_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApartmentResidents_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FeeDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeNoticeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Consumption = table.Column<double>(type: "float", nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VATRate = table.Column<float>(type: "real", nullable: false),
                    VATCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousReadingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreviousReading = table.Column<double>(type: "float", nullable: true),
                    CurrentReadingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentReading = table.Column<double>(type: "float", nullable: true),
                    Proration = table.Column<double>(type: "float", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeDetails_FeeNotices_FeeNoticeId",
                        column: x => x.FeeNoticeId,
                        principalTable: "FeeNotices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeeDetails_FeeTypes_FeeTypeId",
                        column: x => x.FeeTypeId,
                        principalTable: "FeeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeeTiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeRateConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TierOrder = table.Column<int>(type: "int", nullable: false),
                    ConsumptionStart = table.Column<double>(type: "float", nullable: false),
                    ConsumptionEnd = table.Column<double>(type: "float", nullable: false),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeTiers_FeeRateConfigs_FeeRateConfigId",
                        column: x => x.FeeRateConfigId,
                        principalTable: "FeeRateConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Src = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApartmentBuildingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AnnouncementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FeedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAttachments_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAttachments_ApartmentBuildings_ApartmentBuildingId",
                        column: x => x.ApartmentBuildingId,
                        principalTable: "ApartmentBuildings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAttachments_Feedback_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedback",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAttachments_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_ApartmentBuildingId",
                table: "Announcements",
                column: "ApartmentBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentResidents_ApartmentId",
                table: "ApartmentResidents",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentResidents_ResidentId",
                table: "ApartmentResidents",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ApartmentBuildingId",
                table: "Apartments",
                column: "ApartmentBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingCycleSettings_ApartmentBuildingId",
                table: "BillingCycleSettings",
                column: "ApartmentBuildingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_RequestId",
                table: "Feedback",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeDetails_FeeNoticeId",
                table: "FeeDetails",
                column: "FeeNoticeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeDetails_FeeTypeId",
                table: "FeeDetails",
                column: "FeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeDetailTier_FeeDetailId",
                table: "FeeDetailTier",
                column: "FeeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeNotices_ApartmentId",
                table: "FeeNotices",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeRateConfigs_FeeTypeId",
                table: "FeeRateConfigs",
                column: "FeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeTiers_FeeRateConfigId",
                table: "FeeTiers",
                column: "FeeRateConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeTypes_ApartmentBuildingId",
                table: "FeeTypes",
                column: "ApartmentBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_AnnouncementId",
                table: "FileAttachments",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_ApartmentBuildingId",
                table: "FileAttachments",
                column: "ApartmentBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_FeedbackId",
                table: "FileAttachments",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_RequestId",
                table: "FileAttachments",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingRegistrations_ApartmentId",
                table: "ParkingRegistrations",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuantityRateConfig_FeeTypeId",
                table: "QuantityRateConfig",
                column: "FeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ApartmentBuildingId",
                table: "Requests",
                column: "ApartmentBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Residents_ApartmentBuildingId",
                table: "Residents",
                column: "ApartmentBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReadStatus_AnnouncementId",
                table: "UserReadStatus",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReadStatus_UserId_AnnouncementId",
                table: "UserReadStatus",
                columns: new[] { "UserId", "AnnouncementId" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityReadings_ApartmentId",
                table: "UtilityReadings",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilityReadings_FeeTypeId",
                table: "UtilityReadings",
                column: "FeeTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApartmentResidents");

            migrationBuilder.DropTable(
                name: "BillingCycleSettings");

            migrationBuilder.DropTable(
                name: "FeeDetailTier");

            migrationBuilder.DropTable(
                name: "FeeTiers");

            migrationBuilder.DropTable(
                name: "FileAttachments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ParkingRegistrations");

            migrationBuilder.DropTable(
                name: "QuantityRateConfig");

            migrationBuilder.DropTable(
                name: "UserReadStatus");

            migrationBuilder.DropTable(
                name: "UtilityReadings");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.DropTable(
                name: "FeeDetails");

            migrationBuilder.DropTable(
                name: "FeeRateConfigs");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "FeeNotices");

            migrationBuilder.DropTable(
                name: "FeeTypes");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.DropTable(
                name: "ApartmentBuildings");
        }
    }
}
