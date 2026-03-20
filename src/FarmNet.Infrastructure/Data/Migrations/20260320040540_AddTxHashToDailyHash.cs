using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmNet.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTxHashToDailyHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaXacNhan",
                table: "DailyHashes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TxHash",
                table: "DailyHashes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaXacNhan",
                table: "DailyHashes");

            migrationBuilder.DropColumn(
                name: "TxHash",
                table: "DailyHashes");
        }
    }
}
