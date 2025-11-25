using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Documentify.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfficeBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Workdays = table.Column<byte>(type: "tinyint", nullable: true),
                    OpenFrom = table.Column<TimeOnly>(type: "time", nullable: true),
                    OpenTo = table.Column<TimeOnly>(type: "time", nullable: true),
                    EveningOpenFrom = table.Column<TimeOnly>(type: "time", nullable: true),
                    EveningOpenTo = table.Column<TimeOnly>(type: "time", nullable: true),
                    LocationLat = table.Column<double>(type: "float", nullable: true),
                    LocationLng = table.Column<double>(type: "float", nullable: true),
                    LocationText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Is24_7 = table.Column<bool>(type: "bit", nullable: false),
                    Phones = table.Column<string>(type: "nvarchar(165)", maxLength: 165, nullable: true),
                    WriterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ApproverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SuggestionType = table.Column<int>(type: "int", nullable: true),
                    Change = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeBase_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OfficeBase_AspNetUsers_WriterId",
                        column: x => x.WriterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfficeBase_OfficeBase_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsAvailableOnline = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstimatedTime = table.Column<int>(type: "int", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(11,2)", precision: 11, scale: 2, nullable: false),
                    WriterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ApproverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SuggestionType = table.Column<int>(type: "int", nullable: true),
                    Change = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceBase_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServiceBase_AspNetUsers_WriterId",
                        column: x => x.WriterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceBase_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceBase_ServiceBase_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OfficeStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeStatus_OfficeBase_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOfficeSuggestionUpvotes",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpvotedOfficeSuggestionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOfficeSuggestionUpvotes", x => new { x.ApplicationUserId, x.UpvotedOfficeSuggestionsId });
                    table.ForeignKey(
                        name: "FK_UserOfficeSuggestionUpvotes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOfficeSuggestionUpvotes_OfficeBase_UpvotedOfficeSuggestionsId",
                        column: x => x.UpvotedOfficeSuggestionsId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRateOffice",
                columns: table => new
                {
                    OfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RaterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRateOffice", x => new { x.OfficeId, x.RaterId });
                    table.ForeignKey(
                        name: "FK_UserRateOffice_AspNetUsers_RaterId",
                        column: x => x.RaterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRateOffice_OfficeBase_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOfficeAvailability",
                columns: table => new
                {
                    OfficesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOfficeAvailability", x => new { x.OfficesId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_ServiceOfficeAvailability_OfficeBase_OfficesId",
                        column: x => x.OfficesId,
                        principalTable: "OfficeBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOfficeAvailability_ServiceBase_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Order = table.Column<int>(type: "int", nullable: false),
                    AssociatedServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeRequired = table.Column<int>(type: "int", nullable: false),
                    CostRequired = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InnerServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InnerServiceCountOriginals = table.Column<int>(type: "int", nullable: true),
                    InnerServiceCountCopies = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => new { x.AssociatedServiceId, x.Order });
                    table.ForeignKey(
                        name: "FK_Steps_ServiceBase_AssociatedServiceId",
                        column: x => x.AssociatedServiceId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Steps_ServiceBase_InnerServiceId",
                        column: x => x.InnerServiceId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserServiceSuggestionUpvotes",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpvotedServiceSuggestionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServiceSuggestionUpvotes", x => new { x.ApplicationUserId, x.UpvotedServiceSuggestionsId });
                    table.ForeignKey(
                        name: "FK_UserServiceSuggestionUpvotes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServiceSuggestionUpvotes_ServiceBase_UpvotedServiceSuggestionsId",
                        column: x => x.UpvotedServiceSuggestionsId,
                        principalTable: "ServiceBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeBase_ApproverId",
                table: "OfficeBase",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeBase_OfficeId",
                table: "OfficeBase",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeBase_WriterId",
                table: "OfficeBase",
                column: "WriterId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeStatus_OfficeId",
                table: "OfficeStatus",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBase_ApproverId",
                table: "ServiceBase",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBase_CategoryId",
                table: "ServiceBase",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBase_ServiceId",
                table: "ServiceBase",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBase_WriterId",
                table: "ServiceBase",
                column: "WriterId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOfficeAvailability_ServicesId",
                table: "ServiceOfficeAvailability",
                column: "ServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_InnerServiceId",
                table: "Steps",
                column: "InnerServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOfficeSuggestionUpvotes_UpvotedOfficeSuggestionsId",
                table: "UserOfficeSuggestionUpvotes",
                column: "UpvotedOfficeSuggestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRateOffice_RaterId",
                table: "UserRateOffice",
                column: "RaterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServiceSuggestionUpvotes_UpvotedServiceSuggestionsId",
                table: "UserServiceSuggestionUpvotes",
                column: "UpvotedServiceSuggestionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "OfficeStatus");

            migrationBuilder.DropTable(
                name: "ServiceOfficeAvailability");

            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "UserOfficeSuggestionUpvotes");

            migrationBuilder.DropTable(
                name: "UserRateOffice");

            migrationBuilder.DropTable(
                name: "UserServiceSuggestionUpvotes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "OfficeBase");

            migrationBuilder.DropTable(
                name: "ServiceBase");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
