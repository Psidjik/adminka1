using Auth;
using Google.Protobuf.Collections;

namespace Gateway.Api.Schema.Types.Authentication;

[ExtendObjectType<Mutation>]
public class AuthenticationMutation
{
#pragma warning disable CS1573
     /// <summary>
     /// Регистрация
     /// </summary>
     /// <param name="request">Модель запроса для регистрации</param>
     public async Task<string> Register([Service] IMapper mapper,
         [Service] AuthService.AuthServiceClient grpcClient, RegisterUser request)
     {
         await grpcClient.RegisterAsync(mapper.Map(request));

         return "Success!";
     }
     
     /// <summary>
     /// Вход
     /// </summary>
     /// <param name="request">Модель запроса для login</param>
     public async Task<string> Login([Service] IMapper mapper,
         [Service] AuthService.AuthServiceClient grpcClient,
         [Service] IHttpContextAccessor contextAccessor, LoginUser request)
     {
          var response = await grpcClient.LoginAsync(mapper.Map(request));

          var http = contextAccessor.HttpContext ?? throw new Exception("No HTTP context");
          
          http.Response.Headers.Append("Set-Cookie",
              $"accessToken={response.Tokens.AccessToken}; Path=/; Expires={DateTime.UtcNow.AddSeconds(15):R}; HttpOnly; Secure; SameSite=None; Partitioned");

          http.Response.Headers.Append("Set-Cookie",
              $"refreshToken={response.Tokens.RefreshToken}; Path=/; Expires={DateTime.UtcNow.AddSeconds(40):R}; HttpOnly; Secure; SameSite=None; Partitioned");

          return "Success!";
     }
     
     /// <summary>
     /// Выход
     /// </summary>
     /// <param name="request">Модель запроса для logout</param>
     public async Task<string> Logout([Service] IHttpContextAccessor contextAccessor,
         [Service] AuthService.AuthServiceClient grpcClient, LogoutUser request)
     {
         var http = contextAccessor.HttpContext ?? throw new Exception("No HTTP context");

         var refreshToken = http.Request.Cookies["refreshToken"];
         
         http.Response.Headers.Append("Set-Cookie",
             "accessToken=; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT; HttpOnly; Secure; SameSite=None; Partitioned");

         http.Response.Headers.Append("Set-Cookie",
             "refreshToken=; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT; HttpOnly; Secure; SameSite=None; Partitioned");

         
         if (!string.IsNullOrEmpty(refreshToken))
                await grpcClient.LogoutAsync(new LogoutRequest { RefreshToken = refreshToken });
         
         return "Logged out";
     }
     
     /// <summary>
     /// Обновление access токена по refresh токену
     /// </summary>
     public async Task<string> RefreshAccessToken(
         [Service] AuthService.AuthServiceClient grpcClient,
         [Service] IHttpContextAccessor contextAccessor)
     {
         var http = contextAccessor.HttpContext ?? throw new Exception("No HTTP context");

         var refreshToken = http.Request.Cookies["refreshToken"];
         if (string.IsNullOrEmpty(refreshToken))
             throw new UnauthorizedAccessException("No refresh token");

         var response = await grpcClient.RefreshAccessTokenAsync(new RefreshAccessTokenRequest
         {
             RefreshToken = refreshToken
         });

         http.Response.Cookies.Append("accessToken", response.AccessToken, new CookieOptions
         {
             HttpOnly = true,
             Secure = true,
             SameSite = SameSiteMode.None,
             Expires = DateTimeOffset.UtcNow.AddSeconds(30)
         });

         return "Token refreshed";
     }
}
