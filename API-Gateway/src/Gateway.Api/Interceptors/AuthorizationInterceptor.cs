using System.Security.Authentication;
using Auth;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;

namespace Gateway.Api.Interceptors;

public class AuthorizationInterceptor(AuthService.AuthServiceClient authClient) : IHttpRequestInterceptor
{
    public async ValueTask OnCreateAsync(HttpContext context,
        IRequestExecutor requestExecutor,
        OperationRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        var accessToken = context.Request.Cookies["accessToken"];
        var refreshToken = context.Request.Cookies["refreshToken"];

        if (accessToken is null && refreshToken is null)
            return;
        
        if (accessToken is not null)
        {
            var validationAccessTokenResponse = await authClient.ValidateAccessTokenAsync(new ValidateAccessTokenRequest
            {
                AccessToken = accessToken
            }, cancellationToken: cancellationToken);

            if (!validationAccessTokenResponse.IsValid)
                throw new AuthenticationException("Invalid token");
        }


        if (accessToken is null)
        {
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var refreshResponse = await authClient.RefreshAccessTokenAsync(new RefreshAccessTokenRequest
                {
                    RefreshToken = refreshToken
                }, cancellationToken: cancellationToken);

                if (!string.IsNullOrWhiteSpace(refreshResponse.AccessToken))
                {
                    accessToken = refreshResponse.AccessToken;

                    context.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/; samesite=None; Partitioned",
                        Expires = DateTimeOffset.UtcNow.AddMinutes(1)
                    });
                }
                else throw new AuthenticationException("Invalid token: Refresh token expired or invalid");
            }
            else throw new AuthenticationException("Invalid token: Missing refresh token");
        }

        requestBuilder.SetGlobalState("access_token", accessToken);
        requestBuilder.SetGlobalState("refresh_token", refreshToken);
    }
}