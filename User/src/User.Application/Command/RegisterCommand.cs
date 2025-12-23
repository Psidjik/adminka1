using MediatR;
using User.Application.Models;
using User.Domain.ValueObject;

namespace User.Application.Command;

public class RegisterCommand : IRequest<RegisterResult>
{
    public required string Password { get; init; }
    public required FullName FullName { get; init; }
    public required string? PersonalNumber { get; init; }
}
