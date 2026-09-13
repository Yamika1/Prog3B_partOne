using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InteractiveDashboard.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class create4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SensorValue",
                table: "SensorPayload",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SensorValue",
                table: "SensorPayload");
        }
    }
}
