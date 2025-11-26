using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documentify.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfficeSuggestionComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommenterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OfficeSuggestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeSuggestionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeSuggestionComments_OfficeBase_OfficeSuggestionId",
                        column: x => x.OfficeSuggestionId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommenterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceComments_ServiceBase_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSuggestionComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommenterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ServiceSuggestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSuggestionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceSuggestionComments_ServiceBase_ServiceSuggestionId",
                        column: x => x.ServiceSuggestionId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeSuggestionComments_OfficeSuggestionId",
                table: "OfficeSuggestionComments",
                column: "OfficeSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceComments_ServiceId",
                table: "ServiceComments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSuggestionComments_ServiceSuggestionId",
                table: "ServiceSuggestionComments",
                column: "ServiceSuggestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeSuggestionComments");

            migrationBuilder.DropTable(
                name: "ServiceComments");

            migrationBuilder.DropTable(
                name: "ServiceSuggestionComments");
        }
    }
}
