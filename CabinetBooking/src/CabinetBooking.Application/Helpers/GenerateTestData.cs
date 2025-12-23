using CabinetBooking.Domain;
using CabinetBooking.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace CabinetBooking.Application.Helpers;

public class GenerateTestData(CabinetBookingDbContext dbContext) : IGenerateTestData
{
    private readonly Random _random = new();
    private readonly DateOnly _startDate = new(2025, 4, 28);
    private readonly DateOnly _endDate = new(2025, 6, 1);

    public async Task GenerateBookingTestData()
    {
        await GenerateCabinets();
        await GenerateBookings();
    }

    private async Task GenerateCabinets()
    {
        if (await dbContext.Cabinets.AnyAsync())
        {
            return;
        }
        
        var cabinets = new List<Cabinet>();
        var usedNumbers = new HashSet<string>(); 

        for (var building = 1; building <= 4; building++)
        {
            for (var floor = 1; floor <= 5; floor++)
            {
                var cabinetsOnFloor = _random.Next(10, 20);

                var attempts = 0; 

                while (cabinetsOnFloor > 0 && attempts < 1000)
                {
                    attempts++;

                    var roomNumber = _random.Next(1, 100).ToString("D2");
                    var cabinetNumber = $"{building}{floor}{roomNumber}";

                    if (usedNumbers.Contains(cabinetNumber))
                    {
                        continue; 
                    }

                    usedNumbers.Add(cabinetNumber);

                    var cabinetType = GetRandomCabinetType(floor);
                    var isTechnical = _random.Next(10) < 3;
                    var isProjector = _random.Next(10) < 4;

                    cabinets.Add(new Cabinet(cabinetNumber, isTechnical, isProjector, cabinetType));
                    cabinetsOnFloor--; 
                }
            }
        }

        await dbContext.Cabinets.AddRangeAsync(cabinets);
        await dbContext.SaveChangesAsync();
    }

    private CabinetType GetRandomCabinetType(int floor)
    {
        return floor switch
        {
            1 => _random.Next(10) < 6 ? CabinetType.PracticalClassroom :
                _random.Next(10) < 7 ? CabinetType.LabRoom : CabinetType.LectureHall,
            5 => _random.Next(10) < 6 ? CabinetType.LectureHall :
                _random.Next(10) < 7 ? CabinetType.LabRoom : CabinetType.PracticalClassroom,
            _ => (CabinetType)_random.Next(0, 3)
        };
    }

    private async Task GenerateBookings()
    {
        if (await dbContext.Bookings.AnyAsync())
            return;

        var cabinets = await dbContext.Cabinets.AsNoTracking().ToListAsync();
        var lessons = await dbContext.Lessons.ToListAsync();
        var bookings = new List<Booking>();

        const double usageProbability = 0.8;

        for (var currentDate = _startDate; currentDate <= _endDate; currentDate = currentDate.AddDays(1))
        {
            foreach (var lesson in lessons)
            {
                foreach (var cabinet in cabinets)
                {
                    if (_random.NextDouble() < usageProbability)
                    {
                        if (!bookings.Any(b =>
                                b.CabinetId == cabinet.Id &&
                                b.Date == currentDate &&
                                b.LessonId == lesson.LessonNumber))
                        {
                            bookings.Add(new Booking(cabinet.Id, lesson, currentDate));
                        }
                    }
                }
            }
        }

        await dbContext.Bookings.AddRangeAsync(bookings);
        await dbContext.SaveChangesAsync();
    }
}
