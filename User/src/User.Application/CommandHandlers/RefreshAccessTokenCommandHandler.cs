using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Application.Command;
using User.Domain.Data;

namespace User.Application.CommandHandlers;

public class RefreshAccessTokenCommandHandler(UserDbContext context,
    IJwtService jwtService) : IRequestHandler<RefreshAccessTokenCommand, string?>
{
    public async Task<string?> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                    rt.Token == request.RefreshToken &&
                    rt.ExpiresAt > DateTimeOffset.UtcNow,
                cancellationToken);

        if (existingToken is null)
            return null;

        var userAccount = await context.Teachers
            .FirstOrDefaultAsync(u => u.Id == existingToken.TeacherId, cancellationToken);

        return userAccount is null
            ? null
            : jwtService.GenerateAccessToken(userAccount);
    }
}
