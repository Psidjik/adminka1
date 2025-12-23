using CabinetBooking.Domain;
using MediatR;

namespace CabinetBooking.Application.Commands;

public class AddCabinet : IRequest<string>
{
    public required string Number { get; init; }
    public required bool IsProjector { get; init; }
    public required bool IsTechnical { get; init; }
    public required CabinetType CabinetType { get; init; }
}
