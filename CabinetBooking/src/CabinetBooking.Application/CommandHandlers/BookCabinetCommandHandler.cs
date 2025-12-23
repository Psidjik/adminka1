using CabinetBooking.Application.Commands;
using CabinetBooking.Domain;
using CabinetBooking.Domain.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabinetBooking.Application.CommandHandlers;

public class BookCabinetCommandHandler(CabinetBookingDbContext dbContext) : IRequestHandler<BookCabinetCommand, Unit>
{
    public async Task<Unit> Handle(BookCabinetCommand request, CancellationToken cancellationToken)
    {
        var cabinetExists = await dbContext.Cabinets
            .AnyAsync(c => c.Id == request.CabinetId, cancellationToken);
    
        if (!cabinetExists)
            throw new ArgumentException($"Аудитория {request.CabinetId} не найдена");
        
        var lesson = await dbContext.Lessons
            .FirstOrDefaultAsync(l => l.LessonNumber == request.LessonNumber, cancellationToken);

        if (lesson == null)
            throw new ArgumentException($"Аудитория {request.LessonNumber} не найден");
        
        var isAlreadyBooked = await dbContext.Bookings
            .AnyAsync(b => 
                    b.CabinetId == request.CabinetId &&
                    b.Date == request.Date &&
                    b.LessonId == request.LessonNumber,
                cancellationToken);

        if (isAlreadyBooked)
            throw new InvalidOperationException($"Аудитория {request.CabinetId} уже забронирована на это время");
        
        var booking = new Booking(
            cabinetId: request.CabinetId,
            lesson: lesson,
            date: request.Date);

        await dbContext.Bookings.AddAsync(booking, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}