using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algorithms_programm.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Algorithms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperimentSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlgorithmId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NMax = table.Column<int>(type: "INTEGER", nullable: false),
                    Step = table.Column<int>(type: "INTEGER", nullable: false),
                    RunsPerPoint = table.Column<int>(type: "INTEGER", nullable: false),
                    MMax = table.Column<int>(type: "INTEGER", nullable: true),
                    MStep = table.Column<int>(type: "INTEGER", nullable: true),
                    ConfigHash = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Label = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperimentSessions_Algorithms_AlgorithmId",
                        column: x => x.AlgorithmId,
                        principalTable: "Algorithms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperimentRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    N = table.Column<int>(type: "INTEGER", nullable: false),
                    M = table.Column<int>(type: "INTEGER", nullable: true),
                    RunIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    ElapsedMilliseconds = table.Column<double>(type: "REAL", nullable: true),
                    StepCount = table.Column<long>(type: "INTEGER", nullable: true),
                    MeasuredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperimentRuns_ExperimentSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ExperimentSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Algorithms_Name",
                table: "Algorithms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExperimentRuns_SessionId_N_M",
                table: "ExperimentRuns",
                columns: new[] { "SessionId", "N", "M" });

            migrationBuilder.CreateIndex(
                name: "IX_ExperimentSessions_AlgorithmId_ConfigHash",
                table: "ExperimentSessions",
                columns: new[] { "AlgorithmId", "ConfigHash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperimentRuns");

            migrationBuilder.DropTable(
                name: "ExperimentSessions");

            migrationBuilder.DropTable(
                name: "Algorithms");
        }
    }
}
