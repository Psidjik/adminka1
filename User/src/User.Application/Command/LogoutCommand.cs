using MediatR;

namespace User.Application.Command;

public class LogoutCommand : IRequest
{
    public required string RefreshToken { get; init; } 
}
