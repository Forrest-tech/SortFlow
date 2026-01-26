using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SortFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratorRatePerSecond = table.Column<double>(type: "double precision", nullable: false),
                    AddressMismatchProbability = table.Column<double>(type: "double precision", nullable: false),
                    InvalidPostalProbability = table.Column<double>(type: "double precision", nullable: false),
                    DamagedLabelProbability = table.Column<double>(type: "double precision", nullable: false),
                    DashboardWindowMinutes = table.Column<int>(type: "integer", nullable: false),
                    EnableModules = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SortingStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    StationCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortingStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SortingStations_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SortingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    StationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    Result = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SortingEvents_SortingStations_StationId",
                        column: x => x.StationId,
                        principalTable: "SortingStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SortingEvents_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SortingExceptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SortingEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExceptionType = table.Column<int>(type: "integer", nullable: false),
                    Details = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortingExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SortingExceptions_SortingEvents_SortingEventId",
                        column: x => x.SortingEventId,
                        principalTable: "SortingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SortingEvents_StationId",
                table: "SortingEvents",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_SortingEvents_Timestamp",
                table: "SortingEvents",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SortingEvents_ZoneId",
                table: "SortingEvents",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_SortingExceptions_ExceptionType",
                table: "SortingExceptions",
                column: "ExceptionType");

            migrationBuilder.CreateIndex(
                name: "IX_SortingExceptions_SortingEventId",
                table: "SortingExceptions",
                column: "SortingEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SortingExceptions_Timestamp",
                table: "SortingExceptions",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SortingStations_ZoneId",
                table: "SortingStations",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "SortingExceptions");

            migrationBuilder.DropTable(
                name: "SortingEvents");

            migrationBuilder.DropTable(
                name: "SortingStations");

            migrationBuilder.DropTable(
                name: "Zones");
        }
    }
}
