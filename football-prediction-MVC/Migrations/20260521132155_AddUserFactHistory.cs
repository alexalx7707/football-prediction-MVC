using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace football_prediction_MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFactHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFactHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FactId = table.Column<int>(type: "int", nullable: false),
                    ShownAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFactHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFactHistories_UserId_ShownAt",
                table: "UserFactHistories",
                columns: new[] { "UserId", "ShownAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFactHistories");
        }
    }
}
