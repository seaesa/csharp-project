using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmNet.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHinhAnhToSensorData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "HinhAnh",
                table: "SensorData",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "SensorData");
        }
    }
}
