using CabinetBooking.Domain.Common.Domain;
using CommonLibrary.Common.Domain;

namespace CabinetBooking.Domain;

public class Booking : Entity<Guid>
{
    private Booking() {}

    public Booking(string cabinetId, Lesson lesson, DateOnly date)
    {
        CabinetId = cabinetId;
        Lesson = lesson;
        Date = date;
    }


    public string CabinetId { get; private set; }
    public Lesson Lesson { get; private set; }
    public int LessonId { get; private set; }
    public DateOnly Date { get; private set; }
}