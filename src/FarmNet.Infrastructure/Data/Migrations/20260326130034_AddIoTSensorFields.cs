using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmNet.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIoTSensorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BomBat",
                table: "SensorData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CoMua",
                table: "SensorData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "KhiGas",
                table: "SensorData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BomBat",
                table: "SensorData");

            migrationBuilder.DropColumn(
                name: "CoMua",
                table: "SensorData");

            migrationBuilder.DropColumn(
                name: "KhiGas",
                table: "SensorData");
        }
    }
}
