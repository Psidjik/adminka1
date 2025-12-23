using MediatR;

namespace CabinetBooking.Application.Commands;

public record BookCabinetCommand : IRequest<Unit>
{
    public string CabinetId { get; init; }
    public DateOnly Date { get; init; }
    public int LessonNumber { get; init; }
}