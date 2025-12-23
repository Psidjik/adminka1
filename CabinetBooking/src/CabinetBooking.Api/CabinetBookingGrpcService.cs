using CabinetBooking.Application.Commands;
using CabinetBooking.Application.Query;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;

namespace CabinetBooking.Api;

public class CabinetBookingGrpcService(IMediator mediator) : CabinetBookingService.CabinetBookingServiceBase
{
    public override async Task<AddCabinetResponse> AddCabinet(AddCabinetRequest request, ServerCallContext context)
    {
        var command = new AddCabinet
        {
            Number = request.Cabinet.Number,
            IsProjector = request.Cabinet.IsProjector,
            IsTechnical = request.Cabinet.IsTechnical,
            CabinetType = (Domain.CabinetType)request.Cabinet.CabinetType
        };
        
        var response = await mediator.Send(command, CancellationToken.None);
        return new AddCabinetResponse{Number = response};
    }
    
    public override async Task<AvailableCabinetsResponse> GetAvailableCabinets(
        AvailableCabinetsRequest request, ServerCallContext context)
    {
        if (request.LessonNumber is > 8 or < 1)
            throw new ArgumentException("Неверный номер пары");
        
        var date = request.Date.ToDateTime().Date;
        
        Domain.CabinetType? cabinetType = null;
        if (request.HasCabinetType)
        {
            cabinetType = request.CabinetType switch
            {
                CabinetType.LectureHall => Domain.CabinetType.LectureHall,
                CabinetType.LabRoom => Domain.CabinetType.LabRoom,
                CabinetType.PracticalClassroom => Domain.CabinetType.PracticalClassroom,
                _ => throw new ArgumentException($"Unknown cabinet type: {request.CabinetType}")
            };
        }
        
        var result = await mediator.Send(new GetAvailableCabinetsByDate
        {
            Date = DateOnly.FromDateTime(date),
            LessonNumber = request.LessonNumber,
            BuildingNumber = request.BuildingNumber,
            CabinetType = cabinetType
        });

        var response = new AvailableCabinetsResponse();
        response.CabinetIds.AddRange(result);
        return response;
    }
    
    public override async Task<FreeCabinetsResponse> GetFreeCabinets(
        FreeCabinetsRequest request, ServerCallContext context)
    {
        var startDate = request.StartDate.ToDateTime().Date;
        var endDate = request.EndDate.ToDateTime().Date;
        
        Domain.CabinetType? cabinetType = null;
        if (request.HasCabinetType)
        {
            cabinetType = request.CabinetType switch
            {
                CabinetType.LectureHall => Domain.CabinetType.LectureHall,
                CabinetType.LabRoom => Domain.CabinetType.LabRoom,
                CabinetType.PracticalClassroom => Domain.CabinetType.PracticalClassroom,
                _ => throw new ArgumentException($"Unknown cabinet type: {request.CabinetType}")
            };
        }
        
        var query = new GetFreeCabinetsStatisticsQuery
        {
            StartDate = DateOnly.FromDateTime(startDate),
            EndDate = DateOnly.FromDateTime(endDate),
            CabinetType = cabinetType,
            BuildingNumber = request.HasBuildingNumber ? request.BuildingNumber : null
        };

        var result = await mediator.Send(query, context.CancellationToken);
        
        var response = new FreeCabinetsResponse();
        response.Statistics.AddRange(result.Select(x => new FreeCabinets
        {
            Date = Timestamp.FromDateTime(DateTime.SpecifyKind(
                x.Date.ToDateTime(TimeOnly.MinValue), 
                DateTimeKind.Utc)),
            LessonNumber = x.LessonNumber,
            AvailableCabinetsCount = x.AvailableCabinetsCount
        }));

        return response;
    }
    
    public override async Task<BookCabinetResponse> BookCabinet(
        BookCabinetRequest request, 
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.CabinetId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Не указан номер кабинета"));
    
        if (request.LessonNumber is < 1 or > 8)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Неверный номер пары (должен быть 1-8)"));

        var date = request.Date.ToDateTime().Date;
        
        var command = new BookCabinetCommand
        {
            CabinetId = request.CabinetId,
            Date = DateOnly.FromDateTime(date),
            LessonNumber = request.LessonNumber
        };

        await mediator.Send(command, context.CancellationToken);

        return new BookCabinetResponse 
        {
            Success = true,
            Message = $"Аудитория {request.CabinetId} успешно забронирована"
        };
    }
    
}