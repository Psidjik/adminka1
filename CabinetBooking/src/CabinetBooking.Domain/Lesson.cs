using CabinetBooking.Domain.Common.Domain;

namespace CabinetBooking.Domain;

public class Lesson
{
    private Lesson() {}

    public int LessonNumber { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
}