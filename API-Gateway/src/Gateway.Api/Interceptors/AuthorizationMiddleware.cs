using System.Security;
using System.Security.Authentication;
using Auth;
using HotChocolate.Resolvers;

namespace Gateway.Api.Interceptors;

public class AuthorizationMiddleware(AuthService.AuthServiceClient authClient, FieldDelegate next)
{
    public async Task InvokeAsync(IMiddlewareContext context)
    {
        if (context.Path.Length != 1)
        {
            await next(context);
            return;
        }
        
        var fieldName = context.Selection.Field.Name;
        
        if (fieldName is "register" or "login" or "logout")
        {
            await next(context);
            return;
        }

        var accessToken = context.ContextData.TryGetValue("access_token", out var tokenObj)
            ? tokenObj?.ToString()
            : null;
        
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new AuthenticationException("Invalid access_token");
        
        await next(context);
    }
}
