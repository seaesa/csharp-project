using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmNet.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyHashes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    DataHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoNhatKy = table.Column<int>(type: "int", nullable: false),
                    SoCamBien = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyHashes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyHashes_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyHashes_BatchId_Ngay",
                table: "DailyHashes",
                columns: new[] { "BatchId", "Ngay" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyHashes");
        }
    }
}
