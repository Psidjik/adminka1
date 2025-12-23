using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using User.Application.Command;
using User.Domain.Data;

namespace User.Application.CommandHandlers;

public class LogoutCommandHandler(UserDbContext authServiceDbContext) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await authServiceDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null)
            throw new AuthenticationFailureException("Invalid refresh token");
        
        authServiceDbContext.RefreshTokens.Remove(refreshToken);
        await authServiceDbContext.SaveChangesAsync(cancellationToken);
    }
}
