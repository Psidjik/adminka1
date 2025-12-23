namespace Gateway.Api.Schema.Types;

public class GetFreeCabinetsRequest
{
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public int? BuildingNumber { get; init; }
    public CabinetTypes? CabinetType { get; init; }
}
