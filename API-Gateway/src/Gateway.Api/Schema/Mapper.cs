using Gateway.Api.Schema.Types;
using Gateway.Api.Schema.Types.Authentication;
using Google.Protobuf.WellKnownTypes;
using CabinetTypes = Gateway.Api.Schema.Types.CabinetTypes;

namespace Gateway.Api.Schema;

public class Mapper : IMapper
{
    private CabinetBooking.CabinetTypes MapCabinetType(CabinetTypes? cabinetType)
    {
        if (!cabinetType.HasValue)
            return default;

        return (CabinetBooking.CabinetTypes)cabinetType.Value;
    }

    public CabinetBooking.AvailableCabinetsRequest Map(GetAvailableCabinetsByDate request)
    {
        return new CabinetBooking.AvailableCabinetsRequest
        {
            Date = ToTimestamp(request.Date),
            LessonNumber = request.LessonNumber,
            BuildingNumber = request.BuildingNumber ?? 0,
            CabinetType = MapCabinetType(request.CabinetTypes)
        };
    }

    public CabinetBooking.FreeCabinetsRequest Map(GetFreeCabinetsRequest request)
    {
        var grpcRequest = new CabinetBooking.FreeCabinetsRequest
        {
            StartDate = ToTimestamp(request.StartDate),
            EndDate = ToTimestamp(request.EndDate)
        };
        
        if (request.BuildingNumber.HasValue)
        {
            grpcRequest.BuildingNumber = request.BuildingNumber.Value;
        }

        if (request.CabinetType.HasValue)
        {
            grpcRequest.CabinetType = Map(request.CabinetType.Value);
        }

        return grpcRequest;
    }

    public CabinetBooking.BookCabinetRequest Map(BookCabinetRequest request)
    {
        return new CabinetBooking.BookCabinetRequest
        {
            CabinetId = request.CabinetId,
            Date = ToTimestamp(request.Date),
            LessonNumber = request.LessonNumber
        };
    }

    private static Timestamp ToTimestamp(DateTime dateTime)
    {
        return Timestamp.FromDateTime(dateTime.ToUniversalTime());
    }

    private static CabinetBooking.CabinetTypes Map(CabinetTypes cabinetTypes)
    {
        return cabinetTypes switch
        {
            CabinetTypes.LectureHall => CabinetBooking.CabinetTypes.LectureHall,
            CabinetTypes.LabRoom => CabinetBooking.CabinetTypes.LabRoom,
            CabinetTypes.PracticalClassroom => CabinetBooking.CabinetTypes.PracticalClassroom,
            _ => throw new ArgumentOutOfRangeException(nameof(cabinetTypes), 
                $"Неизвестный тип кабинета: {cabinetTypes}")
        };
    }
    public Auth.RegisterRequest Map(RegisterUser request) =>
        new()
        {
            Teacher = new Auth.Teacher
            {
                PersonalNumber = request.PersonalNumber,
                Password = request.Password,
                FullName = new Auth.FullName
                {
                    Name = request.FullName.Name,
                    Surname = request.FullName.Surname,
                    Patronymic = request.FullName.Patronymic ?? string.Empty
                }
            }
        };

    public Auth.LoginRequest Map(LoginUser request) =>
        new()
        {
            PersonalNumber = request.PersonalNubmer,
            Password = request.Password
        };

    public Auth.LogoutRequest Map(LogoutUser request) =>
        new()
        {
            RefreshToken = request.RefreshToken
        };
}