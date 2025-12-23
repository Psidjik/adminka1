using CabinetBooking.Application.Query;
using CabinetBooking.Domain.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabinetBooking.Application.QueryHandler;

public class GetAvailableCabinetsByDateHandler(CabinetBookingDbContext dbContext)
    : IRequestHandler<GetAvailableCabinetsByDate, List<string>>
{
    public async Task<List<string>> Handle(GetAvailableCabinetsByDate request, CancellationToken cancellationToken)
    {
        var availableCabinetsQuery = dbContext.Cabinets
            .Where(c => !dbContext.Bookings
                .Any(b =>
                    b.CabinetId == c.Id &&
                    b.Date == request.Date &&
                    b.LessonId == request.LessonNumber));
        
        if (request.BuildingNumber.HasValue && request.BuildingNumber != 0)
        {
            var buildingStr = request.BuildingNumber.Value.ToString();
            availableCabinetsQuery = availableCabinetsQuery
                .Where(c => c.Id.StartsWith(buildingStr));
        }
    
        if (request.CabinetType.HasValue)
        {
            availableCabinetsQuery = availableCabinetsQuery
                .Where(c => c.CabinetType == request.CabinetType.Value);
        }

        return await availableCabinetsQuery
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);
    }
}