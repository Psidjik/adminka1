using CabinetBooking.Application.Dtos;
using MediatR;

namespace CabinetBooking.Application.Query;

public record GetFreeCabinetsStatisticsQuery : IRequest<IEnumerable<FreeCabinetStatisticDto>>
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public Domain.CabinetType? CabinetType { get; init; }
    public int? BuildingNumber { get; init; }
}