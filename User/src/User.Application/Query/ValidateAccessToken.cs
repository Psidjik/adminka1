using MediatR;

namespace User.Application.Query;

public class ValidateAccessToken : IRequest<bool>
{
    public required string AccessToken { get; init; }
}
