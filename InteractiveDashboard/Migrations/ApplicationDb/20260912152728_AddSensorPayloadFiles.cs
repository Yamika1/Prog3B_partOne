using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InteractiveDashboard.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddSensorPayloadFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensorPayloadFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SensorPayloadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorPayloadFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorPayloadFiles_SensorPayload_SensorPayloadId",
                        column: x => x.SensorPayloadId,
                        principalTable: "SensorPayload",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorPayloadFiles_SensorPayloadId",
                table: "SensorPayloadFiles",
                column: "SensorPayloadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorPayloadFiles");
        }
    }
}
