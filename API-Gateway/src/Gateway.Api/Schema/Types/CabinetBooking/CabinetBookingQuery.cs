using CabinetBooking;

namespace Gateway.Api.Schema.Types.CabinetBooking;

[ExtendObjectType<Query>]
public class CabinetBookingQuery(
    [Service] CabinetBookingService.CabinetBookingServiceClient grpcClient,
    [Service] IMapper mapper)
{
    public async Task<List<string>> GetAvailableCabinets(GetAvailableCabinetsByDate request)
    {
        var grpcRequest = mapper.Map(request);
        var response = await grpcClient.GetAvailableCabinetsAsync(grpcRequest);
        return response.CabinetIds.ToList();
    }


    public async Task<FreeCabinetsResponse> GetFreeCabinets(GetFreeCabinetsRequest request)
    {
        var grpcRequest = mapper.Map(request);
        return await grpcClient.GetFreeCabinetsAsync(grpcRequest);
    }
}