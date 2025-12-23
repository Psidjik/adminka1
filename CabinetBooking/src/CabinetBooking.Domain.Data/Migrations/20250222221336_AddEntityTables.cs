using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CabinetBooking.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cabinet_booking");

            migrationBuilder.CreateTable(
                name: "cabinets",
                schema: "cabinet_booking",
                columns: table => new
                {
                    Number = table.Column<string>(type: "text", nullable: false),
                    is_technical = table.Column<bool>(type: "boolean", nullable: false),
                    is_projector = table.Column<bool>(type: "boolean", nullable: false),
                    cabinet_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cabinets", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                schema: "cabinet_booking",
                columns: table => new
                {
                    lesson_number = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lessons", x => x.lesson_number);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                schema: "cabinet_booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cabinet_id = table.Column<string>(type: "text", nullable: false),
                    lesson_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.id);
                    table.ForeignKey(
                        name: "fk_bookings_cabinets_cabinet_id",
                        column: x => x.cabinet_id,
                        principalSchema: "cabinet_booking",
                        principalTable: "cabinets",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bookings_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalSchema: "cabinet_booking",
                        principalTable: "lessons",
                        principalColumn: "lesson_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bookings_cabinet_id",
                schema: "cabinet_booking",
                table: "bookings",
                column: "cabinet_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_lesson_id",
                schema: "cabinet_booking",
                table: "bookings",
                column: "lesson_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookings",
                schema: "cabinet_booking");

            migrationBuilder.DropTable(
                name: "cabinets",
                schema: "cabinet_booking");

            migrationBuilder.DropTable(
                name: "lessons",
                schema: "cabinet_booking");
        }
    }
}
