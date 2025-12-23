using CabinetBooking.Domain.Common.Domain;
using CommonLibrary.Common.Domain;

namespace CabinetBooking.Domain;

public class Cabinet : Entity<string>
{
    private Cabinet() {}

    public Cabinet(string cabinetId, bool isTechnical, bool isProjector, CabinetType cabinetType)
    {
        Id = cabinetId;
        IsTechnical = isTechnical;
        IsProjector = isProjector;
        CabinetType = cabinetType;
    }

    public bool IsTechnical { get; private set; }
    public bool IsProjector { get; private set; }
    public CabinetType CabinetType { get; private set; }
    private List<Booking> _bookings = []; 
    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    public void AddBooking(Booking booking)
    {
        _bookings.Add(booking);
    }
}