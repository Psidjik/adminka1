using Auth;
using Grpc.Core;
using MediatR;

using User.Application.Command;
using User.Application.Query;

namespace User.Api.Services;

public class UserGrpcService(IMediator mediator) : AuthService.AuthServiceBase
{
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var registerCommand = new RegisterCommand
        {
            Password = request.Teacher.Password,
            FullName = new Domain.ValueObject.FullName(request.Teacher.FullName.Name, request.Teacher.FullName.Surname,
                request.Teacher.FullName.Patronymic),
            PersonalNumber = request.Teacher.PersonalNumber,
        };

        await mediator.Send(registerCommand);

        return new RegisterResponse();
    }
    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var loginCommand = new LoginCommand
        {
            PersonalNumber = request.PersonalNumber,
            Password = request.Password
        };
        var result = await mediator.Send(loginCommand);

        return new LoginResponse
        {
            Tokens = new TokenResponse
            {
                AccessToken = result.Tokens.AccessToken,
                RefreshToken = result.Tokens.RefreshToken
            }
        };
    }
    public override async Task<RefreshAccessTokenResponse> RefreshAccessToken(RefreshAccessTokenRequest request, ServerCallContext context)
    {
        var command = new RefreshAccessTokenCommand { RefreshToken = request.RefreshToken };

        var accessToken = await mediator.Send(command);

        if (accessToken is null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid or expired refresh token"));

        return new RefreshAccessTokenResponse { AccessToken = accessToken };
    }
    public override async Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
    {
        var logoutCommand = new LogoutCommand { RefreshToken = request.RefreshToken };

        await mediator.Send(logoutCommand);

        return new LogoutResponse();
    }
    public override async Task<ValidateAccessTokenResponse> ValidateAccessToken(ValidateAccessTokenRequest request,
        ServerCallContext context)
    {
        var isValid = await mediator.Send(
            new ValidateAccessToken
            {
                AccessToken = request.AccessToken
            }
        );
        
        return new ValidateAccessTokenResponse() { IsValid = isValid };
    }
}