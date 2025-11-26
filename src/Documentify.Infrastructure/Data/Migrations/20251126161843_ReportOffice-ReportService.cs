using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documentify.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReportOfficeReportService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Decision = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ReporterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ResolverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportBase_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReportBase_AspNetUsers_ResolverId",
                        column: x => x.ResolverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportBase_OfficeBase_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportBase_ServiceBase_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportBase_OfficeId",
                table: "ReportBase",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportBase_ReporterId",
                table: "ReportBase",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportBase_ResolverId",
                table: "ReportBase",
                column: "ResolverId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportBase_ServiceId",
                table: "ReportBase",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportBase");
        }
    }
}
