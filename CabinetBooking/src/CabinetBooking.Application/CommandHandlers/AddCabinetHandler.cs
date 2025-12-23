using CabinetBooking.Application.Commands;
using CabinetBooking.Domain;
using CabinetBooking.Domain.Data;
using MediatR;

namespace CabinetBooking.Application.CommandHandlers;

public class AddCabinetHandler(CabinetBookingDbContext context) : IRequestHandler<AddCabinet, string>
{
    public async Task<string> Handle(AddCabinet command, CancellationToken cancellationToken)
    {
        var cabinet = new Cabinet(command.Number, command.IsTechnical, command.IsProjector, command.CabinetType);
        await context.Cabinets.AddAsync(cabinet, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return cabinet.Id; 
    }
}
