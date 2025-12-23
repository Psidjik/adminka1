using CabinetBooking.Domain;
using MediatR;

namespace CabinetBooking.Application.Query;

public class GetAvailableCabinetsByDate : IRequest<List<string>>
{
    public DateOnly Date { get; init; }
    public int LessonNumber { get; init; }
    public int? BuildingNumber { get; init; }
    public CabinetType? CabinetType { get; init; }
}
