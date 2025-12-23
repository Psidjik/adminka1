using MediatR;
using Microsoft.IdentityModel.Tokens;
using User.Application.Query;

namespace User.Application.QueryHandler;

public class ValidateAccessTokenHandler(IJwtService jwtService) : IRequestHandler<ValidateAccessToken, bool>
{
    public Task<bool> Handle(ValidateAccessToken request, CancellationToken cancellationToken)
    {
        try
        {
            jwtService.ValidateAccessToken(request.AccessToken);
            return Task.FromResult(true);
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(false);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
}
