using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CabinetBooking.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLessons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Вставляем данные в таблицу lessons
            migrationBuilder.InsertData(
                schema: "cabinet_booking", // Указываем схему
                table: "lessons",          // Указываем таблицу
                columns: new[] { "lesson_number", "start_time", "end_time" }, // Указываем колонки
                values: new object[,] // Указываем данные
                {
                    { 1, new TimeOnly(8, 30), new TimeOnly(9, 50) },
                    { 2, new TimeOnly(10, 5), new TimeOnly(11, 25) },
                    { 3, new TimeOnly(11, 40), new TimeOnly(13, 0) },
                    { 4, new TimeOnly(13, 45), new TimeOnly(15, 5) },
                    { 5, new TimeOnly(15, 20), new TimeOnly(16, 40) },
                    { 6, new TimeOnly(16, 55), new TimeOnly(18, 15) },
                    { 7, new TimeOnly(18, 30), new TimeOnly(19, 50) },
                    { 8, new TimeOnly(20, 00), new TimeOnly(21, 20) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем данные из таблицы lessons
            migrationBuilder.DeleteData(
                schema: "cabinet_booking", // Указываем схему
                table: "lessons",          // Указываем таблицу
                keyColumn: "lesson_number", // Указываем колонку для поиска
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8 }); // Указываем значения для удаления
        }
    }
}