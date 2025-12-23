namespace Gateway.Api.Schema.Types;

public class GetAvailableCabinetsByDate
{
    public required DateTime Date { get; init; }
    public required int LessonNumber { get; init; }
    public int? BuildingNumber { get; init; }
    public CabinetTypes? CabinetTypes { get; init; }
}
