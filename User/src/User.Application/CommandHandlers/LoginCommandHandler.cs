using System.Security.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Application.Command;
using User.Application.Models;
using User.Domain.Data;

namespace User.Application.CommandHandlers;

public class LoginCommandHandler(UserDbContext userDbContext, IJwtService jwtService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {

        var userAccount = await userDbContext.Teachers
            .FirstOrDefaultAsync(u => u.PersonalNumber == request.PersonalNumber, cancellationToken);

        if (userAccount is null || !BCrypt.Net.BCrypt.Verify(request.Password, userAccount.Password))
            throw new AuthenticationException("Неверный email или пароль");

        var accessToken = jwtService.GenerateAccessToken(userAccount);
        var refreshToken = jwtService.GenerateRefreshToken();

        var tokens = new Tokens
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        
        await jwtService.SaveRefreshToken(userAccount.Id, refreshToken);

        return new LoginResponse
        {
            Tokens = tokens,
        };
    }
}
