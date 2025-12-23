using MediatR;

namespace User.Application.Command;

public class RefreshAccessTokenCommand : IRequest<string?>
{
    public required string RefreshToken { get; init; }
}
