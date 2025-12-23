namespace Gateway.Api.Schema.Types;

public class BookCabinetRequest
{
    public required string CabinetId { get; init; }
    public required DateTime Date { get; init; }
    public required int LessonNumber { get; init; }
}