using MediatR;
using User.Application.Models;

namespace User.Application.Command;

public class LoginCommand : IRequest<LoginResponse>
{
    public required string PersonalNumber { get; init; }
    public required string Password { get; init; }
}
