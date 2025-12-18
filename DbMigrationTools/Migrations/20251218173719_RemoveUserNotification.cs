using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbMigrationTools.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserIds",
                table: "Announcements",
                newName: "ApartmentIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApartmentIds",
                table: "Announcements",
                newName: "UserIds");
        }
    }
}
