namespace Gateway.Api.Schema.Types.Authentication;

/// <summary>
/// Токены
/// </summary>
public class Tokens
{
    /// <summary>
    /// AccessToken
    /// </summary>
    public required string AccessToken { get; init; }
    
    /// <summary>
    /// RefreshToken
    /// </summary>
    public required string RefreshToken { get; init; }
}
