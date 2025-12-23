using Gateway.Api.Schema.Types;
using Gateway.Api.Schema.Types.Authentication;

namespace Gateway.Api.Schema;

public interface IMapper
{
    CabinetBooking.AvailableCabinetsRequest Map(GetAvailableCabinetsByDate request);
    CabinetBooking.FreeCabinetsRequest Map(GetFreeCabinetsRequest request);
    CabinetBooking.BookCabinetRequest Map(BookCabinetRequest request);
    
    Auth.RegisterRequest Map(RegisterUser request);
    
    Auth.LoginRequest Map(LoginUser request);
    
    Auth.LogoutRequest Map(LogoutUser request);
}