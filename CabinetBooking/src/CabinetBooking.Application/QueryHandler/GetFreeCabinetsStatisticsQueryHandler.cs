using CabinetBooking.Application.Dtos;
using CabinetBooking.Application.Query;
using CabinetBooking.Domain.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CabinetBooking.Application.QueryHandler;

public class GetFreeCabinetsStatisticsQueryHandler(
    CabinetBookingDbContext dbContext,
    ILogger<GetFreeCabinetsStatisticsQueryHandler> _logger)
    : IRequestHandler<GetFreeCabinetsStatisticsQuery, IEnumerable<FreeCabinetStatisticDto>>
{
    private readonly CabinetBookingDbContext _dbContext = dbContext;

    public async Task<IEnumerable<FreeCabinetStatisticDto>> Handle(
        GetFreeCabinetsStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Получаем ВСЕ кабинеты (без фильтров по умолчанию)
        var cabinetsQuery = _dbContext.Cabinets.AsQueryable();

        // 2. Применяем фильтры ТОЛЬКО если они указаны в запросе
        if (request.CabinetType.HasValue)
        {
            cabinetsQuery = cabinetsQuery.Where(c => c.CabinetType == request.CabinetType.Value);
        }

        if (request.BuildingNumber.HasValue)
        {
            cabinetsQuery = cabinetsQuery.Where(c => c.Id.StartsWith($"{request.BuildingNumber}"));
        }

        var filteredCabinetIds = await cabinetsQuery.Select(c => c.Id).ToListAsync(cancellationToken);

        // 3. Получаем бронирования ТОЛЬКО если есть кабинеты для фильтрации
        if (!filteredCabinetIds.Any())
        {
            // Возвращаем все кабинеты как доступные, если нет фильтров
            if (!request.CabinetType.HasValue && !request.BuildingNumber.HasValue)
            {
                filteredCabinetIds = await _dbContext.Cabinets.Select(c => c.Id).ToListAsync(cancellationToken);
            }
        }

        // 4. Получаем бронирования для отфильтрованных кабинетов
        var bookingsQuery = _dbContext.Bookings
            .Include(b => b.Lesson)
            .Where(b => b.Date >= request.StartDate && b.Date <= request.EndDate);

        if (filteredCabinetIds.Any())
        {
            bookingsQuery = bookingsQuery.Where(b => filteredCabinetIds.Contains(b.CabinetId));
        }

        var bookings = await bookingsQuery
            .Select(b => new { b.Date, b.CabinetId, b.Lesson.LessonNumber })
            .ToListAsync(cancellationToken);

        // 5. Генерируем статистику
        var result = new List<FreeCabinetStatisticDto>();

        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            for (int lessonNumber = 1; lessonNumber <= 8; lessonNumber++)
            {
                var bookedCabinetIds = bookings
                    .Where(b => b.Date == date && b.LessonNumber == lessonNumber)
                    .Select(b => b.CabinetId)
                    .Distinct()
                    .ToList();

                var availableCount = filteredCabinetIds.Count - bookedCabinetIds.Count;

                result.Add(new FreeCabinetStatisticDto
                {
                    Date = date,
                    LessonNumber = lessonNumber,
                    AvailableCabinetsCount = Math.Max(0, availableCount)
                });
            }
        }

        return result;
    }
}