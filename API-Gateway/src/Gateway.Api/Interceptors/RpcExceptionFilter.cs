using System.Security;
using System.Security.Authentication;
using Grpc.Core;

namespace Gateway.Api.Interceptors;

public class RpcExceptionFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        switch (error.Exception)
        {
            case RpcException ex:
            {
                var reasonEntry = ex.Trailers.FirstOrDefault(t => t.Key == "reason");
                var codeEntry = ex.Trailers.FirstOrDefault(t => t.Key == "code");

                if (reasonEntry != null && codeEntry != null)
                {
                    var newError = ErrorBuilder.New()
                        .SetCode(codeEntry.Value)
                        .SetMessage(reasonEntry.Value)
                        .Build();

                    return newError;
                }
                else
                {
                    var newError = ErrorBuilder.New()
                        .SetCode(ex.StatusCode.ToString())
                        .SetMessage(ex.StatusCode.ToString())
                        .Build();
                    
                    return newError;
                }
            }
            case AuthenticationException argEx:
            {
                var newError = ErrorBuilder.New()
                    .SetCode("401")
                    .SetMessage(argEx.Message)
                    .Build();
                
                return newError;
            }
            case SecurityException secEx:
            {
                var newError = ErrorBuilder.New()
                    .SetCode("403")
                    .SetMessage(secEx.Message)
                    .Build();
                
                return newError;
            }
            default:
                return error;
        }
    }
}


