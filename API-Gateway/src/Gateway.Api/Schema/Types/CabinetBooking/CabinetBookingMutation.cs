using CabinetBooking;

namespace Gateway.Api.Schema.Types.CabinetBooking;

[ExtendObjectType<Mutation>]
public class CabinetBookingMutation(
    [Service] CabinetBookingService.CabinetBookingServiceClient grpcClient,
    [Service] IMapper mapper)
{
    public async Task<BookCabinetResponse> BookCabinet(BookCabinetRequest request)
    {
        var grpcRequest = mapper.Map(request);
        return await grpcClient.BookCabinetAsync(grpcRequest);
    }
}