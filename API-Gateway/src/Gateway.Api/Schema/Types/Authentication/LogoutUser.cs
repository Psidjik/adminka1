namespace Gateway.Api.Schema.Types.Authentication;

/// <summary>
/// Запрос на выход
/// </summary>
public class LogoutUser
{
    /// <summary>
    /// RefreshToken
    /// </summary>
    public required string RefreshToken { get; init; } 
}
