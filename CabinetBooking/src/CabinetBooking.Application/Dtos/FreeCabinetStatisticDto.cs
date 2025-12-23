using MediatR;

namespace CabinetBooking.Application.Dtos;

public record FreeCabinetStatisticDto
{
    public DateOnly Date { get; init; }
    public int LessonNumber { get; init; }
    public int AvailableCabinetsCount { get; init; }
}